using Realtime.Data;
using Realtime.Networks;

namespace Realtime.Controllers.Match.Interfaces;

public interface IMatchShutdownHandler<TMatchData, TPlayerIndex, TAuthData, TPlayer> 
    where TPlayer : PlayerData<TPlayerIndex, TAuthData>
    where TMatchData : MatchData<TPlayerIndex, TAuthData, TPlayer>
    where TPlayerIndex : unmanaged, INetworkIndex
{
    Task OnShutdown(IMatchRunner<TMatchData, TPlayerIndex, TAuthData, TPlayer> matchRunner, int tick);
}