using Realtime.Controllers.Handlers.Interfaces;
using Realtime.Data;
using Realtime.Networks;

namespace Realtime.Controllers.Handlers.Impl;

internal partial class MatchTickManager<TMatchData, TPlayerIndex, TPlayer> 
    where TPlayer : PlayerData<TPlayerIndex>
    where TMatchData : MatchData<TPlayerIndex, TPlayer>
{
    private readonly IEnumerable<IMatchTickHandler<TMatchData, TPlayerIndex, TPlayer>> _tickHandlers;
    private readonly IMatchRunner<TMatchData, TPlayerIndex, TPlayer> _matchRunner;
    private readonly MatchTickCounter _matchTickCounter;

    // Generate code to call _msgReceiver.Flush() => Flush()
    // Generate all message handlers of tick
    // private readonly IEnumerable<IMatchMessageHandler<>> _tickHandlers;
    //private partial Task SendMessage(CancellationToken cancellationToken);
    private void ReceiveMessages(CancellationToken cancellationToken)
    {
        int order = 0;
        foreach (var message in _matchRunner.FlushReceivedMessages())
        {
            ReceiveMessage(message, order);
            order++;
        }
    }

    public struct MyMessage : INetworkPayload
    {
        
    }
    // Generated
    private readonly IEnumerable<IMatchMessageHandler<MyMessage, TMatchData, TPlayerIndex, TPlayer>>
        _myMessageHandlers;
    private void ReceiveMessage(in NetworkMessage<TPlayerIndex, INetworkPayload> message, in int order)
    {
        switch (message.Opcode)
        {
            case 0:
            {
                foreach (var handler in _myMessageHandlers)
                {
                    var msg = new NetworkMessage<TPlayerIndex, MyMessage>
                    {
                        Opcode = message.Opcode,
                        Payload = (MyMessage)message.Payload,
                        Owner = message.Owner,
                        Target = message.Target,
                        MessageType = message.MessageType
                    };
                    handler.OnMessage(_matchRunner, _matchTickCounter.Tick, order, msg);
                }
                break;
            }
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
        await _matchRunner.StartFlushingReceivedMessages(token).ConfigureAwait(false);
        foreach (var handler in _tickHandlers)
            handler.OnStartTick(matchRunner, tick);
        ReceiveMessages(token);
        await SendMessages(token);
        foreach (var handler in _tickHandlers)
            handler.OnEndTick(matchRunner, tick);
        await _matchTickCounter.EndTick(token).ConfigureAwait(false);
    }
}