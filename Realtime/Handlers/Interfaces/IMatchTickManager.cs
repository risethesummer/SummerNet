using Realtime.Data;

namespace Realtime.Handlers.Interfaces;

public interface IMatchTickManager<TMatchData, TPlayerIndex, TAuthData, TPlayer> 
    where TPlayer : PlayerData<TPlayerIndex, TAuthData>, new()
    where TMatchData : MatchData<TPlayerIndex, TAuthData, TPlayer>
{
    public ValueTask Tick(IMatchContext<TMatchData, TPlayerIndex, TAuthData, TPlayer> matchContext,
        CancellationToken token);
    void AddHandler<TMessage>(IMatchMessageHandler<TMessage, TPlayerIndex> handler);
    void AddHandler(IMatchTickHandler<TMatchData, TPlayerIndex, TAuthData, TPlayer> handler);
}