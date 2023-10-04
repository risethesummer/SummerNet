using Realtime.Data;

namespace Realtime.Controllers.Handlers.Interfaces;

public interface IMatchShutdownHandler<TMatchData, TPlayerIndex, TPlayer> 
    where TPlayer : PlayerData<TPlayerIndex>
    where TMatchData : MatchData<TPlayerIndex, TPlayer>
{
    Task OnShutdown(IMatchRunner<TMatchData, TPlayerIndex, TPlayer> matchRunner, int tick);
}