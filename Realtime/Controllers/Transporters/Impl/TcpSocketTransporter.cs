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
    private readonly IPool<PoolableWrapper<byte[]>> _bufferPool;
    private readonly IPool<DisposablePoolWrapper<DisposableQueue<Task>>> _taskPool;

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
        var queue = _taskPool.Create().Value;
        Broadcast(data, queue, cancellationToken);
        return new ValueTask(Task.WhenAll(queue));
    }
    
    public ValueTask SendMessageInline<T>(in NetworkMessage<TPlayerIndex, T> msg,
        CancellationToken cancellationToken) where T : unmanaged, INetworkPayload
    {
        if (!_sockets.ContainsKey(msg.Target))
            return ValueTask.CompletedTask;
        var data = _messageEncoder.EncodeNonAlloc(msg);
        return _sentParallelBufferWrapper.AddToBuffer(new SentMessage<TPlayerIndex>
        {
            Data =  data,
            MessageType = msg.MessageType,
            Target = msg.Target
        }, cancellationToken);
    }

    public async ValueTask FlushSentMessage(CancellationToken cancellationToken)
    {
        using var queueTask = _taskPool.Create().Value;
        using var messagesBuffer = await _sentParallelBufferWrapper.GetBuffer(cancellationToken);
        var messages = messagesBuffer.Data;
        foreach (var message in messages)
        {
            if (cancellationToken.IsCancellationRequested)
                break;
            SendMessageInternal(message, queueTask, cancellationToken);
        }
        await Task.WhenAll(queueTask).ConfigureAwait(false);
    }

    private void SendMessageInternal(in SentMessage<TPlayerIndex> message, 
        in Queue<Task> queueTask, CancellationToken cancellationToken)
    {
        if (message.MessageType == MessageType.Broadcast)
            Broadcast(message.Data, queueTask, cancellationToken);
        else
            SentMessageToTarget(message.Target, message.Data, queueTask, cancellationToken);
    }
    private void Broadcast(in byte[] msg, in Queue<Task> queueTasks, in CancellationToken token)
    {
        foreach (var socket in _sockets.Values)
            queueTasks.Enqueue(socket.SendAsync(msg, token).AsTask());
    }
    private Task SentMessageToTarget(in TPlayerIndex target, in byte[] msg, in CancellationToken token)
    {
        if (!_sockets.TryGetValue(target, out var socket))
            return Task.CompletedTask;
        return socket.SendAsync(msg, token).AsTask();
    }
    private void SentMessageToTarget(in TPlayerIndex target, in byte[] msg, 
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
        var length = messages.Count;
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

    public ValueTask StartFlushingReceivedMessagesAsync(CancellationToken cancellationToken)
    {
        using var queueTasks = _taskPool.Create().Value;
        queueTasks.Clear();
        foreach (var socket in _sockets.Values)
            queueTasks.Enqueue(FlushTaskRunner(socket, cancellationToken));
        return new ValueTask(Task.WhenAll(queueTasks));
    }
    private Task FlushTaskRunner(Socket socket, CancellationToken cancellationToken)
    {
        using var bufferWrapper = _bufferPool.Create();
        return socket.FlushReceivedData(bufferWrapper.Value, 
            _receivedParallelBufferWrapper, cancellationToken);
    }
}