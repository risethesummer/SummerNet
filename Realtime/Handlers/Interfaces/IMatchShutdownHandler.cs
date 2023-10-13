using Realtime.Data;

namespace Realtime.Handlers.Interfaces;

public interface IMatchShutdownHandler<TMatchData, TPlayerIndex, TAuthData, TPlayer> 
    where TPlayer : PlayerData<TPlayerIndex, TAuthData>, new()
    where TMatchData : MatchData<TPlayerIndex, TAuthData, TPlayer>
{
    Task OnShutdown(IMatchContext<TMatchData, TPlayerIndex, TAuthData, TPlayer> matchContext, int tick);
}