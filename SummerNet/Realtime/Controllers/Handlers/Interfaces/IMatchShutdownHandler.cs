using SummerNet.Realtime.Data;

namespace SummerNet.Realtime.Controllers.Handlers.Interfaces;

public interface IMatchShutdownHandler<TMatchData, TPlayerIndex, TPlayer> 
    where TPlayer : PlayerData<TPlayerIndex>
    where TMatchData : MatchData<TPlayerIndex, TPlayer>
{
    Task OnShutdown(IMatchController<TMatchData, TPlayerIndex, TPlayer> matchController, int tick);
}