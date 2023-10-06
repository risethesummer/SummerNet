using System.Net.Sockets;
using System.Runtime.CompilerServices;
using MemoryPack;
using Realtime.Controllers.Filters.Interfaces;
using Realtime.Controllers.Transporters.Interfaces;
using Realtime.Data;
using Realtime.Networks;
using Realtime.Utils.Buffers;
using Realtime.Utils.Extensions;
using Realtime.Utils.Factory;

namespace Realtime.Controllers.Transporters.Impl;

// Decode into segment

public class TcpSocketTransporter<TPlayerIndex, TPlayer> : ITransporter<TPlayerIndex, TPlayer>
    where TPlayerIndex : unmanaged, INetworkIndex
    where TPlayer : PlayerData<TPlayerIndex>, INetworkPayload
{
    private readonly MessageDecoder<TPlayerIndex> _messageDecoder;
    private readonly MessageEncoder _messageEncoder;
    private readonly IParallelBuffer<byte> _receivedParallelBufferWrapper;
    private readonly IParallelBuffer<SentMessage<TPlayerIndex>> _sentParallelBufferWrapper;
    private readonly Dictionary<TPlayerIndex, Socket> _sockets = new();
    private readonly IPlayerAuthenticator<TPlayerIndex, TPlayer> _authenticator;
    private readonly IFactory<uint, PoolableWrapper<uint, AutoSizeBuffer<byte>>> _bufferPool;
    private readonly IFactory<PoolableWrapper<DisposableQueue<Task>>> _taskPool;

    private readonly IFactory<DangerousBuffer<byte>,
        PoolableWrapper<DangerousBuffer<byte>, UnmanagedMemoryManager<byte>>> _memoryManagerPool;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
            var data = ExtractMessage<T>(buffer.Value, dataCount);
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
        var data = _messageEncoder.EncodeNonAlloc(msg);
        if (msg.MessageType != MessageType.Broadcast)
            return new ValueTask(SentMessageToTarget(msg.Target, data, cancellationToken));
        return new ValueTask(Broadcast(data, cancellationToken));
    }

    private async Task SentMessageToTarget(TPlayerIndex target, AutoDisposableData<ReadOnlyMemory<byte>,
        UnmanagedMemoryManager<byte>> msg, CancellationToken token)
    {
        using (msg)
        {
            if (!_sockets.TryGetValue(target, out var socket))
                return;
            await socket.SendAsync(msg.Data, token).ConfigureAwait(false);
        }
    }

    private async Task Broadcast(AutoDisposableData<ReadOnlyMemory<byte>, UnmanagedMemoryManager<byte>> msg,
        CancellationToken token)
    {
        using (msg)
        {
            using var queueWrapper = _taskPool.Create();
            var queue = queueWrapper.AsValue();
            foreach (var socket in _sockets.Values)
                queue.Enqueue(socket.SendAsync(msg.Data, token).AsTask());
            await Task.WhenAll(queue).ConfigureAwait(false);
        }
    }

    public ValueTask SendMessageInline<T>(in NetworkMessage<TPlayerIndex, T> msg,
        CancellationToken cancellationToken) where T : unmanaged, INetworkPayload
    {
        if (!_sockets.ContainsKey(msg.Target))
            return ValueTask.CompletedTask;
        var data = _messageEncoder.EncodeNonAlloc(msg);
        // We're not disposing the message here but when flushing to send them in tick
        return _sentParallelBufferWrapper.AddToBuffer(new SentMessage<TPlayerIndex>
        {
            Data = data,
            MessageType = msg.MessageType,
            Target = msg.Target
        }, cancellationToken);
    }

    public async ValueTask FlushSentMessage(CancellationToken cancellationToken)
    {
        using var queueTask = _taskPool.Create();
        using var messagesBuffer = await _sentParallelBufferWrapper.GetBuffer(cancellationToken);
        var messages = messagesBuffer.Data;
        var queue = queueTask.AsValue();
        SendMessagesToQueue(messages, queue, cancellationToken);
        await Task.WhenAll(queue).ConfigureAwait(false);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void SendMessagesToQueue(in Memory<SentMessage<TPlayerIndex>> messages,
        in Queue<Task> queue, in CancellationToken cancellationToken)
    {
        try
        {
            foreach (var message in messages.Span)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;
                SendMessageInternal(message, queue, cancellationToken);
            }
        }
        finally
        {
            messages.Dispose();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void SendMessageInternal(in SentMessage<TPlayerIndex> message,
        in Queue<Task> queueTask, CancellationToken cancellationToken)
    {
        if (message.MessageType == MessageType.Broadcast)
            Broadcast(message.Data.Data, queueTask, cancellationToken);
        else
            SentMessageToTarget(message.Target, message.Data.Data, queueTask, cancellationToken);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Broadcast(in ReadOnlyMemory<byte> msg, in Queue<Task> queueTasks, in CancellationToken token)
    {
        foreach (var socket in _sockets.Values)
            queueTasks.Enqueue(socket.SendAsync(msg, token).AsTask());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void SentMessageToTarget(in TPlayerIndex target, in ReadOnlyMemory<byte> msg,
        in Queue<Task> queueTasks, in CancellationToken token)
    {
        if (!_sockets.TryGetValue(target, out var socket))
            return;
        queueTasks.Enqueue(socket.SendAsync(msg, token).AsTask());
    }


    public void Clear()
    {
        _receivedParallelBufferWrapper.Clear();
        _sentParallelBufferWrapper.Clear();
    }

    public async IAsyncEnumerable<NetworkMessage<TPlayerIndex, INetworkPayload>> FlushReceivedMessagesAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        using var messagesBuffer = await
            _receivedParallelBufferWrapper.GetBuffer(cancellationToken);
        var messages = messagesBuffer.Data;
        var length = messages.Length;
        var newIndex = 0;
        while (newIndex < length)
        {
            if (cancellationToken.IsCancellationRequested)
                break;
            var converted =
                _messageDecoder.Decode(messages[newIndex..], out newIndex);
            if (converted.HasValue)
                yield return converted.Value;
        }
    }

    public async ValueTask StartFlushingReceivedMessagesAsync(CancellationToken cancellationToken)
    {
        using var queueTasks = _taskPool.Create();
        var queue = queueTasks.AsValue();
        uint countBytes = 0;
        foreach (var socket in _sockets.Values)
            countBytes += (uint)socket.Available;
        if (countBytes <= 0)
            return;
        _receivedParallelBufferWrapper.Resize(countBytes); //Resize to avoid reallocation 
        foreach (var socket in _sockets.Values)
            if (socket.Available > 0)
                queue.Enqueue(FlushTaskRunner(socket, cancellationToken));
        await Task.WhenAll(queue);
    }

    private async Task FlushTaskRunner(Socket socket, CancellationToken cancellationToken)
    {
        var available = socket.Available;
        if (available == 0)
            return;
        using var buffer = new AutoSizeBuffer<byte>((uint)available);
        using var memoryManagerWrapper = _memoryManagerPool.Create(buffer.DangerousBuffer);
        var memoryManager = memoryManagerWrapper.AsValue();
        await socket.FlushReceivedData(memoryManager.Memory,
                _receivedParallelBufferWrapper, cancellationToken)
            .ConfigureAwait(false);
    }
}