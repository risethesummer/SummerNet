using MemoryPack;
using Realtime.Data;
using Realtime.Handlers.Interfaces;
using Realtime.Transporters.Interfaces;
using Realtime.Transporters.Messages;
using Realtime.Utils.Extensions;
using Realtime.Utils.Factory;

namespace Realtime.Handlers.Impl;

public partial class MatchTickManager<TMatchData, TPlayerIndex, TAuthData, TPlayer> : IMatchTickManager<TMatchData, TPlayerIndex, TAuthData, TPlayer>
    where TPlayer : PlayerData<TPlayerIndex, TAuthData>, new()
    where TMatchData : MatchData<TPlayerIndex, TAuthData, TPlayer>
{
    private readonly List<IMatchTickHandler<TMatchData, TPlayerIndex, TAuthData, TPlayer>> _tickHandlers;
    private readonly ITransporter<TPlayerIndex, TAuthData, TPlayer> _matchContext;
    private readonly MatchTickCounter _matchTickCounter;
    private readonly UnmanagedMemoryManager<byte> _memoryManager;
    private readonly Queue<Task> _receivedTasks = new();
    public MatchTickManager(ITransporter<TPlayerIndex, TAuthData, TPlayer> matchContext, MatchTickCounter matchTickCounter, 
        UnmanagedMemoryManager<byte> memoryManager)
    {
        _matchContext = matchContext;
        _matchTickCounter = matchTickCounter;
        _memoryManager = memoryManager;
    }



    public void AddHandler(IMatchTickHandler<TMatchData, TPlayerIndex, TAuthData, TPlayer> handler)
    {
        _tickHandlers.Add(handler);
    }

    // Generate code to call _msgReceiver.Flush() => Flush()
    // Generate all message handlers of tick
    // private readonly IEnumerable<IMatchMessageHandler<>> _tickHandlers;
    //private partial Task SendMessage(CancellationToken cancellationToken);

 
    // Generated
    // Base on opcode 0
    public struct MyMessage
    {
    }
    private readonly IMatchMessageHandler<MyMessage, TPlayerIndex>[] _0MessageHandlers;
    private readonly IEnumerable<IMatchMessageAsyncHandler<MyMessage, TMatchData, TPlayerIndex, TAuthData, TPlayer>> _0MessageAsyncHandlers;
    public void AddHandler<TMessage>(IMatchMessageHandler<TMessage, TPlayerIndex> handler)
    {
        
    }
    private partial void ReceiveMessage(in RawReceivedNetworkMessage<TPlayerIndex> message,
        in Queue<Task> taskQueue,
        in ulong tick, in ushort order, in CancellationToken cancellationToken)
    {
        switch (message.Opcode)
        {
            case 0:
            {
                var byteData = message.Payload.GetMemory(_memoryManager);
                var msg = new ReceivedNetworkMessage<TPlayerIndex, MyMessage>
                {
                    Opcode = message.Opcode,
                    AssociatedClient = message.Owner,
                    Payload = MemoryPackSerializer.Deserialize<MyMessage>(byteData.Span),
                    Tick = tick,
                    OrderInTick = order,
                    MessageType = MessageType.ClientToSever
                };
                foreach (var handler in _0MessageHandlers)
                    handler.OnMessage(msg);
                foreach (var handler in _0MessageAsyncHandlers)
                    taskQueue.Enqueue(handler.OnMessage(msg, cancellationToken).AsTask());
                byteData.Free();
                break;
            }
        }
    }
    // private readonly IEnumerable<IMatchMessageHandler<MyMessage, TMatchData, TPlayerIndex, TAuthData, TPlayer>>
    //     _myMessageHandlers;    
    // private readonly IEnumerable<IMatchMessageAsyncHandler<MyMessage, TMatchData, TPlayerIndex, TAuthData, TPlayer>>
    //     _myMessageAsyncHandlers;


    private partial void ReceiveMessage(in RawReceivedNetworkMessage<TPlayerIndex> message,
        in Queue<Task> taskQueue,
        in ulong tick, in ushort order, in CancellationToken cancellationToken);
    private async ValueTask ReceiveMessagesAsync(CancellationToken cancellationToken)
    {
        var tick = _matchTickCounter.Tick;
        var data = await _matchContext.FlushReceivedMessagesAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            _receivedTasks.Clear();
            ReceiveMessages(data, _receivedTasks, tick, cancellationToken);
            await Task.WhenAll(_receivedTasks).ConfigureAwait(false);
        }
        finally
        {
            data.Dispose();
        }
    }
    private void ReceiveMessages(in ReadOnlyMemory<RawReceivedNetworkMessage<TPlayerIndex>> messages, in Queue<Task> taskQueue,
        in ulong tick, in CancellationToken cancellationToken)
    {
        ushort order = 0;
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

    // Reset StartReceivingMessagesForNewTick
    // Call StartReceivingMessagesForNewTick
    // Wait for new tick
    // Cancel StartReceivingMessagesForNewTick
    // Tick
    private ValueTask StartReceivingMessagesForNewTick(CancellationToken token)
    {
        return _matchContext.StartReceivingMessagesAsync(token);
    }
    
    public async ValueTask Tick(IMatchContext<TMatchData, TPlayerIndex, TAuthData, TPlayer> matchContext, 
        CancellationToken token)
    {
        var tick = _matchTickCounter.Tick;
        foreach (var handler in _tickHandlers)
            handler.OnStartTick(matchContext, tick);
        await ReceiveMessagesAsync(token).ConfigureAwait(false);
        await _matchContext.FlushSentMessage(token).ConfigureAwait(false);
        foreach (var handler in _tickHandlers)
            handler.OnEndTick(matchContext, tick);
        await _matchTickCounter.EndTick(token).ConfigureAwait(false);
        _matchContext.Clear();
    }

 
}