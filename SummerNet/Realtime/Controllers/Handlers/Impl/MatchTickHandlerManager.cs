using SummerNet.Realtime.Controllers.Distributors.Interfaces;
using SummerNet.Realtime.Controllers.Handlers.Interfaces;
using SummerNet.Realtime.Data;

namespace SummerNet.Realtime.Controllers.Handlers.Impl;

internal class MatchTickHandlerManager<TMatchData, TPlayerIndex, TPlayer> : IDisposable 
    where TPlayer : PlayerData<TPlayerIndex>
    where TMatchData : MatchData<TPlayerIndex, TPlayer>
{
    private readonly CancellationTokenSource _token;
    private readonly TMatchData _matchData;
    private readonly IEnumerable<IMatchTickHandler<TMatchData, TPlayerIndex, TPlayer>> _tickHandlers;
    private readonly IMessageReceiver<> _msgReceiver;
    //Generate all message handlers of tick
    private readonly IEnumerable<IMatchMessageHandler<>> _tickHandlers;
    public MatchTickHandlerManager(TMatchData matchData, IEnumerable<IMatchTickHandler<TMatchData, TPlayerIndex, TPlayer>> tickHandlers)
    {
        _matchData = matchData;
        _tickHandlers = tickHandlers;
        _token = new CancellationTokenSource();
    }
    public void Tick()
    {
        foreach (var handler in _tickHandlers)
        {
            handler.//Start tick
        }
        // Generate code to call _msgReceiver.Flush() => Flush()
        foreach (var handler in _tickHandlers)
        {
            handler.//End tick
        }
    }
    public void Dispose()
    {
        _token.Dispose();
    }
}