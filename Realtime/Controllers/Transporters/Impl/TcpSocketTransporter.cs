using System.Buffers;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MemoryPack;
using Realtime.Controllers.Filters.Interfaces;
using Realtime.Controllers.Transporters.Interfaces;
using Realtime.Data;
using Realtime.Networks;
using Realtime.Utils;

namespace Realtime.Controllers.Transporters.Impl;

public class MessageEncoder
{
    public byte[] Encode<TPlayerIndex, TData>(in NetworkMessage<TPlayerIndex, TData> msg)
        where TData : unmanaged, INetworkPayload
    {
        Span<byte> idSpan = stackalloc byte[sizeof(ushort)];
        BitConverter.TryWriteBytes(idSpan, msg.Opcode);
        return ListExtensions.SerializeWith(msg, idSpan);
    }
}

public static class MessageHelper
{
    // Generate length depends on opcode
    public static int GetPayloadLength(in ushort id);
}

// Decode into segment
public partial class MessageDecoder<TPlayerIndex>
{
    public IEnumerable<NetworkMessage<TPlayerIndex, INetworkPayload>> Decode(ReadOnlySpan<byte> buffer)
    {
        var bufferLength = buffer.Length;
        for (var start = 0; start < bufferLength;)
        {
            var id = BitConverter.ToUInt16(buffer[..2]);
            var length = MessageHelper.GetPayloadLength(id);
            var message = MemoryPackSerializer.Deserialize<NetworkMessage<TPlayerIndex, INetworkPayload>>(buffer[..length]);
            start += length;
            yield return message;
        }
    }
}

public interface IBufferStorage<TWrappedData>
{
    ValueTask AddToBuffer(Memory<TWrappedData> data, CancellationToken token);
    ValueTask AddToBuffer(TWrappedData data, CancellationToken token);
    void Clear();
    ReadOnlySpan<TWrappedData> Data { get; }
}

public sealed class PoolableWrapper<T> : IDisposable
{
    private readonly IPool<PoolableWrapper<T>> _pool;
    public T Value { get; }
    public PoolableWrapper(IPool<PoolableWrapper<T>> pool, T wrappedValue)
    {
        _pool = pool;
        Value = wrappedValue;
    }
    public static implicit operator T(PoolableWrapper<T> entry) => entry.Value;
    public void Dispose()
    {
        _pool.Dispose(this);
    }
}
public interface IPool<T> : IFactory<T>
{
    void Dispose(T obj);
}
public interface IFactory<out T>
{
    T Create();
}
public class SimpleObjectPooling<T> : IPool<T>
{
    private readonly Queue<T> _pool;
    private readonly IFactory<T> _factory;
    public SimpleObjectPooling(int capacity, IFactory<T> factory)
    {
        _pool = new Queue<T>(capacity);
        _factory = factory;
    }
    public T Create()
    {
        if (_pool.Count > 0)
            return _pool.Dequeue();
        var newObj = _factory.Create();
        _pool.Enqueue(newObj);
        return newObj;
    }
    public void Dispose(T obj)
    {
        _pool.Enqueue(obj);
    }
}

public class BufferWrapper<TWrappedData> : IDisposable, IBufferStorage<TWrappedData>
{
    private readonly List<TWrappedData> _buffer = new(10000);
    private readonly SemaphoreSlim _semaphoreSlim = new(0, 1);
    public async ValueTask AddToBuffer(TWrappedData data, CancellationToken token)
    {
        try
        {
            await _semaphoreSlim.WaitAsync(token).ConfigureAwait(false);
            _buffer.Add(data);
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }

    public void Clear()
    {
        _buffer.Clear();
    }

    public ReadOnlySpan<TWrappedData> Data => CollectionsMarshal.AsSpan(_buffer);

    public async ValueTask AddToBuffer(Memory<TWrappedData> data, CancellationToken token)
    {
        try
        {
            await _semaphoreSlim.WaitAsync(token).ConfigureAwait(false);
            _buffer.Add(data);
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }

    public void Dispose()
    {
        _semaphoreSlim.Dispose();
    }
}

public class TcpSocketTransporter<TPlayerIndex, TPlayer> : ITransporter<TPlayerIndex, TPlayer>
    where TPlayerIndex : notnull
    where TPlayer : PlayerData<TPlayerIndex>, INetworkPayload
{
    private readonly MessageDecoder<TPlayerIndex> _messageDecoder;
    private readonly MessageEncoder _messageEncoder;
    private readonly IBufferStorage<byte> _receivedBufferWrapper;
    private readonly IBufferStorage<SentMessage<TPlayerIndex>> _sentBufferWrapper;
    private readonly Dictionary<TPlayerIndex, Socket> _sockets = new();
    private readonly IPlayerAuthenticator<TPlayerIndex, TPlayer> _authenticator;
    private readonly IPool<PoolableWrapper<byte[]>> _bufferPool;
    private readonly Queue<Task> _queueTasks = new();

    public NetworkMessage<TPlayerIndex, T>? ExtractMessage<T>(byte[] data, in int length) where T : INetworkPayload
    {
        var serializeData = data.AsSpan(0, length);
        var deserializedData = MemoryPackSerializer.Deserialize<NetworkMessage<TPlayerIndex, T>>(serializeData);
        return deserializedData;
    }
    public async ValueTask<NetworkMessage<TPlayerIndex, T>?> GetMessageAsync<T>(Socket socket, uint opcode,
        CancellationToken cancellationToken = default) where T : INetworkPayload
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            using var buffer = _bufferPool.Create();
            var dataCount = await socket.ReceiveAsync(buffer.Value, cancellationToken).ConfigureAwait(false);
            if (cancellationToken.IsCancellationRequested)
                return null;
            var data = ExtractMessage<T>(buffer, dataCount);
            if (data.HasValue && data.Value.Opcode == opcode)
                return data;
        }

        return null;
    }

    public async ValueTask RemovePlayerAsync(TPlayerIndex target)
    {
        if (!_sockets.TryGetValue(target, out var socket))
            return;
        await socket.DisconnectAsync(false).ConfigureAwait(false);
    }

    public ValueTask<TPlayer> AddPlayerAsync(TPlayer playerData)
    {
        throw new NotImplementedException();
    }

    private void AddPlayer(TPlayerIndex playerIndex, Socket socket)
    {
        _sockets.Add(playerIndex, socket);
    }

    public async ValueTask InitMatchPlayersAsync(CancellationToken initMatchToken)
    {
        using Socket listener = new(
            AddressFamily.InterNetwork,
            SocketType.Stream,
            ProtocolType.Tcp);
        while (!initMatchToken.IsCancellationRequested)
        {
            var accept = await listener.AcceptAsync(initMatchToken).ConfigureAwait(false);
            var player = await HandShake(accept);
            if (player is not null)
                AddPlayer(player.PlayerId, accept);
        }
    }

    private async ValueTask<TPlayer?> HandShake(Socket socket)
    {
        var data = await GetMessageAsync<TPlayer>(socket, 0);
        return data.HasValue ? await _authenticator.Authenticate(data.Value.Payload) : null;
    }

    public ValueTask SendMessage<T>(in NetworkMessage<TPlayerIndex, T> msg,
        CancellationToken cancellationToken) where T : unmanaged, INetworkPayload
    {
        if (!_sockets.ContainsKey(msg.Target))
            return ValueTask.CompletedTask;
        var data = _messageEncoder.Encode(msg);
        return _sentBufferWrapper.AddToBuffer(new SentMessage<TPlayerIndex>
        {
            Data =  data,
            MessageType = msg.MessageType,
            Target = msg.Target
        }, cancellationToken);
    }

    public ValueTask FlushSentMessage(CancellationToken cancellationToken)
    {
        _queueTasks.Clear();
        var messages = _sentBufferWrapper.Data;
        foreach (var message in messages)
        {
            if (cancellationToken.IsCancellationRequested)
                break;
            if (message.MessageType == MessageType.Broadcast)
                Broadcast(message.Data);
            else
                SentMessageToTarget(message.Target, message.Data);
        }
        return new ValueTask(Task.WhenAll(_queueTasks));
    }
    private void Broadcast(in byte[] msg)
    {
        foreach (var socket in _sockets.Values)
            _queueTasks.Enqueue(socket.ReceiveAsync(msg));
    }
    private void SentMessageToTarget(in TPlayerIndex target, in byte[] msg)
    {
        if (!_sockets.TryGetValue(target, out var socket))
            return;
        _queueTasks.Enqueue(socket.SendAsync(msg));
    }

    public void Clear()
    {
        _receivedBufferWrapper.Clear();
        _sentBufferWrapper.Clear();
        _queueTasks.Clear();
    }
    public IEnumerable<NetworkMessage<TPlayerIndex, INetworkPayload>> FlushReceivedMessages() =>  
        _messageDecoder.Decode(_receivedBufferWrapper.Data);

    public ValueTask StartFlushingReceivedMessages(CancellationToken cancellationToken)
    {
        _queueTasks.Clear();
        foreach (var socket in _sockets.Values)
            _queueTasks.Enqueue(FlushTaskRunner(socket, cancellationToken));
        return new ValueTask(Task.WhenAll(_queueTasks));
    }
    private async Task FlushTaskRunner(Socket socket, CancellationToken cancellationToken)
    {
        using var bufferWrapper = _bufferPool.Create();
        await socket.FlushReceivedData(bufferWrapper.Value, _receivedBufferWrapper, cancellationToken);
    }
}