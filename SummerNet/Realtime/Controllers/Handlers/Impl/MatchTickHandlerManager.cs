using SummerNet.Realtime.Controllers.Distributors.Interfaces;
using SummerNet.Realtime.Controllers.Handlers.Interfaces;
using SummerNet.Realtime.Data;

namespace SummerNet.Realtime.Controllers.Handlers.Impl;

internal partial class MatchTickHandlerManager<TMatchData, TPlayerIndex, TPlayer> : IDisposable 
    where TPlayer : PlayerData<TPlayerIndex>
    where TMatchData : MatchData<TPlayerIndex, TPlayer>
{
    private readonly CancellationTokenSource _tokenSource;
    private CancellationToken Token => _tokenSource.Token;
    private readonly IEnumerable<IMatchTickHandler<TMatchData, TPlayerIndex, TPlayer>> _tickHandlers;
    private readonly IMessageReceiver<TPlayerIndex> _msgReceiver;
    private readonly MatchTickCounter _matchTickCounter;

    public MatchTickHandlerManager(IEnumerable<IMatchTickHandler<TMatchData, TPlayerIndex, TPlayer>> tickHandlers, 
        IMessageReceiver<TPlayerIndex> msgReceiver, MatchTickCounter matchTickCounter)
    {
        _tickHandlers = tickHandlers;
        _msgReceiver = msgReceiver;
        _matchTickCounter = matchTickCounter;
        _tokenSource = new CancellationTokenSource();
    }

    // Generate code to call _msgReceiver.Flush() => Flush()
    // Generate all message handlers of tick
    // private readonly IEnumerable<IMatchMessageHandler<>> _tickHandlers;
    private partial Task SendMessage(CancellationToken cancellationToken);
    public async Task Tick(IMatchController<TMatchData, TPlayerIndex, TPlayer> matchController)
    {
        var tick = _matchTickCounter.Tick;
        foreach (var handler in _tickHandlers)
            handler.OnStartTick(matchController, tick);
        await SendMessage(Token);
        foreach (var handler in _tickHandlers)
            handler.OnEndTick(matchController, tick);
        await _matchTickCounter.EndTick(Token);
    }
    public void Dispose()
    {
        _tokenSource.Dispose();
    }
}