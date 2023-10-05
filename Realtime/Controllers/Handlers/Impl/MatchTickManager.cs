using Realtime.Controllers.Handlers.Interfaces;
using Realtime.Data;
using Realtime.Networks;

namespace Realtime.Controllers.Handlers.Impl;

internal partial class MatchTickManager<TMatchData, TPlayerIndex, TPlayer> 
    where TPlayer : PlayerData<TPlayerIndex>
    where TMatchData : MatchData<TPlayerIndex, TPlayer>
    where TPlayerIndex : unmanaged, INetworkIndex
{
    private readonly IEnumerable<IMatchTickHandler<TMatchData, TPlayerIndex, TPlayer>> _tickHandlers;
    private readonly IMatchRunner<TMatchData, TPlayerIndex, TPlayer> _matchRunner;
    private readonly MatchTickCounter _matchTickCounter;

    // Generate code to call _msgReceiver.Flush() => Flush()
    // Generate all message handlers of tick
    // private readonly IEnumerable<IMatchMessageHandler<>> _tickHandlers;
    //private partial Task SendMessage(CancellationToken cancellationToken);


    public struct MyMessage : INetworkPayload
    {
        
    }
    // Generated
    private readonly IEnumerable<IMatchMessageHandler<MyMessage, TMatchData, TPlayerIndex, TPlayer>>
        _myMessageHandlers;    
    private readonly IEnumerable<IMatchMessageAsyncHandler<MyMessage, TMatchData, TPlayerIndex, TPlayer>>
        _myMessageAsyncHandlers;
    private async ValueTask ReceiveMessages(CancellationToken cancellationToken)
    {
        int order = 0;
        var tick = _matchTickCounter.Tick;
        await foreach (var message in  _matchRunner.FlushReceivedMessagesAsync(cancellationToken))
        {
            if (cancellationToken.IsCancellationRequested)
                return;
            switch (message.Opcode)
            {
                case 0:
                {
                    var msg = new NetworkMessage<TPlayerIndex, MyMessage>
                    {
                        Opcode = message.Opcode,
                        Payload = (MyMessage)message.Payload,
                        Owner = message.Owner,
                        Target = message.Target,
                        MessageType = message.MessageType
                    };
                    foreach (var handler in _myMessageHandlers)
                        handler.OnMessage(_matchRunner, tick, order, msg);
                    foreach (var handler in _myMessageAsyncHandlers)
                        await handler.OnMessage(_matchRunner, tick, order, msg);
                    break;
                }
            }
            order++;
        }
    }
    //

    private ValueTask SendMessages(CancellationToken cancellationToken)
    {
        return _matchRunner.FlushSentMessage(cancellationToken);
    }
    public async ValueTask Tick(IMatchRunner<TMatchData, TPlayerIndex, TPlayer> matchRunner, 
        CancellationToken token)
    {
        var tick = _matchTickCounter.Tick;
        await _matchRunner.StartFlushingReceivedMessagesAsync(token).ConfigureAwait(false);
        foreach (var handler in _tickHandlers)
            handler.OnStartTick(matchRunner, tick);
        await ReceiveMessages(token);
        await SendMessages(token);
        foreach (var handler in _tickHandlers)
            handler.OnEndTick(matchRunner, tick);
        await _matchTickCounter.EndTick(token).ConfigureAwait(false);
        _matchRunner.Clear();
    }
}