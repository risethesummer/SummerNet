﻿using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using Realtime.Data;
using Realtime.Transporters.Interfaces;
using Realtime.Transporters.Messages;
using Realtime.Utils.Buffers;
using Realtime.Utils.Extensions;
using Realtime.Utils.Factory;
using static System.Threading.Tasks.ValueTask;

namespace Realtime.Transporters.Impl;

// Decode into segment

public class SocketTransporter<TPlayerIndex, TAuthData, TPlayer> : 
    ITransporter<TPlayerIndex, TAuthData, TPlayer>, IDisposable
    where TPlayerIndex : unmanaged
    where TPlayer : PlayerData<TPlayerIndex, TAuthData>
{
    private readonly IMessageDecoder _messageDecoder;
    private readonly IMessageEncoder _messageEncoder;
    private readonly ParallelBuffer<RawReceivedNetworkMessage<TPlayerIndex>> _receivedParallelBufferWrapper;
    private readonly ParallelBuffer<DecodedSentMessage<TPlayerIndex>> _sentParallelBufferWrapper;
    private readonly IFactory<PoolableWrapper<DisposableQueue<Task>>> _taskPool;
    private readonly IFactory<BufferPointer<byte>, 
        PoolableWrapper<BufferPointer<byte>, UnmanagedMemoryManager<byte>>> _memoryManagerPool;
    private readonly ConcurrentDictionary<TPlayerIndex, ISocket> _sockets = new();
    private readonly List<TPlayerIndex> _indexToPlayerIndex = new();
    public SocketTransporter(IMessageDecoder messageDecoder, IMessageEncoder messageEncoder, 
        ParallelBuffer<RawReceivedNetworkMessage<TPlayerIndex>> receivedParallelBufferWrapper, 
        ParallelBuffer<DecodedSentMessage<TPlayerIndex>> sentParallelBufferWrapper, 
        IFactory<PoolableWrapper<DisposableQueue<Task>>> taskPool, 
        IFactory<BufferPointer<byte>, PoolableWrapper<BufferPointer<byte>, UnmanagedMemoryManager<byte>>> memoryManagerPool)
    {
        _messageDecoder = messageDecoder;
        _messageEncoder = messageEncoder;
        _receivedParallelBufferWrapper = receivedParallelBufferWrapper;
        _sentParallelBufferWrapper = sentParallelBufferWrapper;
        _taskPool = taskPool;
        _memoryManagerPool = memoryManagerPool;
    }
    public async ValueTask RemovePlayerAsync(TPlayerIndex target, CancellationToken cancellationToken)
    {
        if (!_sockets.TryGetValue(target, out var socket))
            return;
        await socket.DisconnectAsync(false, cancellationToken).ConfigureAwait(false);
    }

    public ValueTask<TPlayer?> AddPlayerAsync(TPlayer playerData, ISocket socket)
    {
        if (_sockets.TryAdd(playerData.PlayerId, socket))
        {
            _indexToPlayerIndex.Add(playerData.PlayerId);
            return FromResult<TPlayer?>(playerData);
        }
        return FromResult<TPlayer?>(null);
    }
    public ValueTask SendMessage<T>(in SentNetworkMessage<TPlayerIndex, T> msg,
        CancellationToken cancellationToken)
    {
        if (!_sockets.ContainsKey(msg.AssociatedClient))
            return CompletedTask;
        var data = _messageEncoder.EncodeNonAlloc(msg.Opcode, msg.Payload);
        using var memoryManagerWrapper = _memoryManagerPool.Create(data);
        ref readonly var memoryManager = ref memoryManagerWrapper.WrappedValue;
        var dangerousMemory = new DangerousMemory<byte>(memoryManager.ForgetMemory);
        if (msg.MessageType != MessageType.Broadcast)
            return SentMessageToTarget(msg.AssociatedClient, dangerousMemory, cancellationToken);
        return Broadcast(dangerousMemory, cancellationToken);
    }
    private async ValueTask SentMessageToTarget(TPlayerIndex target, DangerousMemory<byte> msg, CancellationToken token)
    {
        using (msg)
        {
            if (!_sockets.TryGetValue(target, out var socket))
                return;
            await socket.SendAsync(msg.Memory, token).ConfigureAwait(false);
        }
    }
    private async ValueTask Broadcast(DangerousMemory<byte> msg,
        CancellationToken token)
    {
        using (msg)
        {
            using var queueWrapper = _taskPool.Create();
            var queue = queueWrapper.WrappedValue;
            foreach (var socket in _sockets.Values)
                queue.Enqueue(socket.SendAsync(msg.Memory, token).AsTask());
            await Task.WhenAll(queue).ConfigureAwait(false);
        }
    }
    public ValueTask SendMessageInline<T>(in SentNetworkMessage<TPlayerIndex, T> msg,
        CancellationToken cancellationToken)
    {
        if (!_sockets.ContainsKey(msg.AssociatedClient))
            return CompletedTask;
        var data = _messageEncoder.EncodeNonAlloc(msg.Opcode, msg.Payload);
        // We're not disposing the message here but when flushing to send them in tick
        return _sentParallelBufferWrapper.AddToBuffer(new DecodedSentMessage<TPlayerIndex>
        {
            Payload = data,
            MessageType = msg.MessageType,
            Target = msg.AssociatedClient
        }, cancellationToken);
        // return ValueTask.CompletedTask;
    }

    public async ValueTask FlushSentMessage(CancellationToken cancellationToken)
    {
        using var queueTask = _taskPool.Create();
        var queue = queueTask.WrappedValue;
        using var buffer = await _sentParallelBufferWrapper
            .GetBuffer(cancellationToken).ConfigureAwait(false);
        try
        {
            SendMessagesToQueue(buffer.Data, queue, cancellationToken);
            await Task.WhenAll(queue).ConfigureAwait(false);
        }
        finally
        {
            buffer.Data.Dispose();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void SendMessagesToQueue(in ReadOnlyMemory<DecodedSentMessage<TPlayerIndex>> messages,
        in Queue<Task> queue, in CancellationToken cancellationToken)
    {
        using var memoryManagerWrapper = _memoryManagerPool.Create(new BufferPointer<byte>());
        var memoryManager = memoryManagerWrapper.WrappedValue;
        foreach (var message in messages.Span)
        {
            if (cancellationToken.IsCancellationRequested)
                break;
            SendMessageInternal(message, memoryManager, queue, cancellationToken);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void SendMessageInternal(in DecodedSentMessage<TPlayerIndex> message, in UnmanagedMemoryManager<byte> memoryManager,
        in Queue<Task> queueTask, CancellationToken cancellationToken)
    {
        var sentData = message.Payload.GetMemory(memoryManager);
        if (message.MessageType == MessageType.Broadcast)
            Broadcast(sentData, queueTask, cancellationToken);
        else
            SentMessageToTarget(message.Target, sentData, queueTask, cancellationToken);
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

    public ValueTask Shutdown()
    {
        throw new NotImplementedException();
    }

    public async ValueTask<ReadOnlyMemory<RawReceivedNetworkMessage<TPlayerIndex>>> FlushReceivedMessagesAsync(CancellationToken cancellationToken)
    {
        using var messagesBuffer = await
            _receivedParallelBufferWrapper.GetBuffer(cancellationToken).ConfigureAwait(false);
        return messagesBuffer.Data;
    }

    public async ValueTask StartReceivingMessagesAsync(CancellationToken cancellationToken)
    {
        // Dispose graph: queueTasks->queue
        using var queueTasks = _taskPool.Create();
        var queue = queueTasks.WrappedValue;
        for (ushort i = 0; i < _indexToPlayerIndex.Count; i++)
            queue.Enqueue(ReceivingMessagesRunner(i, _sockets[_indexToPlayerIndex[i]], cancellationToken));
        await Task.WhenAll(queue).ConfigureAwait(false);
    }

    private readonly List<ReadOnlyMemory<byte>> _previousNotPrecessedMemories = new();
    private async Task ReceivingMessagesRunner(ushort ownerIndex, ISocket socket, 
        CancellationToken cancellationToken)
    {
        // Dispose graph: memoryManagerWrapper->memoryManager->buffer
        // In case we need to store incomplete messages, buffer will need to dispose itself
        // Because we borrow memoryManager for allocating the message stored in _previousNotPrecessedMemories
        using var buffer = new AutoSizeBuffer<byte>(1024); 
        using var memoryManagerWrapper = _memoryManagerPool.Create(buffer.BufferPointer);
        var memoryManager = memoryManagerWrapper.WrappedValue;
        var memory = memoryManager.Memory;

        var previousMemory = _previousNotPrecessedMemories[ownerIndex];
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var previousLength = 0;
                if (!previousMemory.IsEmpty)
                {
                    buffer.Write(previousMemory);
                    previousLength = previousMemory.Length;
                }
            
                var dataCount = await socket.ReceiveAsync(memory[previousLength..], cancellationToken)
                    .ConfigureAwait(false);
                if (dataCount <= 0)
                    break;
            
                ReadOnlyMemory<byte> received = memory[..(previousLength + dataCount)];
                while (received.Length > 0 && !cancellationToken.IsCancellationRequested) 
                {
                    var decode = _messageDecoder.Decode(received);
                    if (!decode.HasValue)
                        break;
                    var decodeVal = decode.Value;
                    // Alloc another buffer for BufferPointer
                    var payload = AutoSizeBuffer<byte>.GetBufferPointer(received[..decodeVal.Length]);
                    await _receivedParallelBufferWrapper.AddToBuffer(new RawReceivedNetworkMessage<TPlayerIndex>
                    {
                        Opcode = decodeVal.Opcode,
                        Owner = _indexToPlayerIndex[ownerIndex],
                        Payload = payload
                    }, cancellationToken);
                    if (decodeVal.Length >= received.Length) //Processed all the messages
                        break;
                    received = received[decodeVal.Length..]; //Continue with the remaining part
                }
                previousMemory = received.Length > 0 ? received : ReadOnlyMemory<byte>.Empty;
            }
        }
        finally
        {
            //Deallocate the previous message
            if (!_previousNotPrecessedMemories[ownerIndex].IsEmpty)
                _previousNotPrecessedMemories[ownerIndex].Free();

            // If we still have incomplete messages
            // Try to handle it in the next tick
            if (previousMemory.IsEmpty)
                _previousNotPrecessedMemories[ownerIndex] = ReadOnlyMemory<byte>.Empty;
            else
            {
                // previousMemory now use the same memory region with memoryManager
                // If there's any not processed message, we need to create a new segment for it
                memoryManager.Initialize(AutoSizeBuffer<byte>.GetBufferPointer(previousMemory));
                _previousNotPrecessedMemories[ownerIndex] = memoryManager.ForgetMemory;
            }
        }
    }

    public void Dispose()
    {
        foreach (var socket in _sockets)
            socket.Value.Dispose();
    }
}