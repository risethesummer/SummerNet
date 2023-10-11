using Realtime.Data;

namespace Realtime.Controllers.Match.Interfaces;

public interface IMatchShutdownHandler<TMatchData, TPlayerIndex, TAuthData, TPlayer> 
    where TPlayer : PlayerData<TPlayerIndex, TAuthData>
    where TMatchData : MatchData<TPlayerIndex, TAuthData, TPlayer>
    where TPlayerIndex : unmanaged
{
    Task OnShutdown(IMatchContext<TMatchData, TPlayerIndex, TAuthData, TPlayer> matchContext, int tick);
}