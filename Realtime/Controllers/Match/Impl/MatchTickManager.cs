using MemoryPack;
using Realtime.Controllers.Match.Interfaces;
using Realtime.Data;
using Realtime.Networks;
using Realtime.Utils.Extensions;
using Realtime.Utils.Factory;

namespace Realtime.Controllers.Match.Impl;

public struct MyMessage : INetworkPayload
{
}
internal partial class MatchTickManager<TMatchData, TPlayerIndex, TAuthData, TPlayer> 
    where TPlayer : PlayerData<TPlayerIndex, TAuthData>
    where TMatchData : MatchData<TPlayerIndex, TAuthData, TPlayer>
    where TPlayerIndex : unmanaged, INetworkIndex
{
    private readonly IEnumerable<IMatchTickHandler<TMatchData, TPlayerIndex, TAuthData, TPlayer>> _tickHandlers;
    private readonly IMatchRunner<TMatchData, TPlayerIndex, TAuthData, TPlayer> _matchRunner;
    private readonly MatchTickCounter _matchTickCounter;

    // Generate code to call _msgReceiver.Flush() => Flush()
    // Generate all message handlers of tick
    // private readonly IEnumerable<IMatchMessageHandler<>> _tickHandlers;
    //private partial Task SendMessage(CancellationToken cancellationToken);

 
    // Generated
    private readonly IEnumerable<IMatchMessageHandler<MyMessage, TMatchData, TPlayerIndex, TAuthData, TPlayer>>
        _myMessageHandlers;    
    private readonly IEnumerable<IMatchMessageAsyncHandler<MyMessage, TMatchData, TPlayerIndex, TAuthData, TPlayer>>
        _myMessageAsyncHandlers;
    
    private readonly UnmanagedMemoryManager<byte> memoryManager;
    private readonly Queue<Task> receivedTasks;
    private async ValueTask ReceiveMessagesAsync(CancellationToken cancellationToken)
    {
        var tick = _matchTickCounter.Tick;
        var data = await _matchRunner.FlushReceivedMessagesAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            receivedTasks.Clear();
            ReceiveMessages(data, receivedTasks, tick, cancellationToken);
            await Task.WhenAll(receivedTasks).ConfigureAwait(false);
        }
        finally
        {
            data.Dispose();
        }
    }
    private void ReceiveMessages(in ReadOnlyMemory<ReceivedNetworkMessage<TPlayerIndex>> messages, in Queue<Task> taskQueue,
        in int tick, in CancellationToken cancellationToken)
    {
        int order = 0;
        var msgSpan = messages.Span;
        foreach (var message in msgSpan)
        {
            if (cancellationToken.IsCancellationRequested)
                return;
            // Generate a handler caller for each message id
            ReceiveMessage(message, taskQueue, tick, order, cancellationToken);
            order++;
        }
    }

    // This method is intended to be generated
    private void ReceiveMessage(in ReceivedNetworkMessage<TPlayerIndex> message,
        in Queue<Task> taskQueue,
        in int tick, in int order, in CancellationToken cancellationToken)
    {
        switch (message.MessageId)
        {
            case 0:
            {
                var byteData = message.Payload.GetMemory(memoryManager);
                var msg = new NetworkMessage<TPlayerIndex, MyMessage>
                {
                    Opcode = message.Opcode,
                    AssociatedClient = message.Owner,
                    Payload = MemoryPackSerializer.Deserialize<MyMessage>(byteData.Span),
                    MessageType = MessageType.ClientToSever
                };
                foreach (var handler in _myMessageHandlers)
                    handler.OnMessage(_matchRunner, tick, order, msg);
                foreach (var handler in _myMessageAsyncHandlers)
                    taskQueue.Enqueue(handler.OnMessage(_matchRunner, tick, order, msg, cancellationToken).AsTask());
                byteData.Free();
                break;
            }
        }
    }


    // Reset StartReceivingMessagesForNewTick
    // Call StartReceivingMessagesForNewTick
    // Wait for new tick
    // Cancel StartReceivingMessagesForNewTick
    // Tick
    private ValueTask StartReceivingMessagesForNewTick(CancellationToken token)
    {
        return _matchRunner.StartReceivingMessagesAsync(token);
    }
    
    public async ValueTask Tick(IMatchRunner<TMatchData, TPlayerIndex, TAuthData, TPlayer> matchRunner, 
        CancellationToken token)
    {
        var tick = _matchTickCounter.Tick;
        foreach (var handler in _tickHandlers)
            handler.OnStartTick(matchRunner, tick);
        await ReceiveMessagesAsync(token).ConfigureAwait(false);
        await _matchRunner.FlushSentMessage(token).ConfigureAwait(false);
        foreach (var handler in _tickHandlers)
            handler.OnEndTick(matchRunner, tick);
        await _matchTickCounter.EndTick(token).ConfigureAwait(false);
        _matchRunner.Clear();
    }
}