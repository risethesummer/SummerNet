using SummerNet.Realtime.Data;

namespace SummerNet.Realtime.Controllers.Handlers.Interfaces;

public interface IMatchTickHandler<TMatchData, TPlayerIndex, TPlayer> 
    where TPlayer : PlayerData<TPlayerIndex>
    where TMatchData : MatchData<TPlayerIndex, TPlayer>
{
    void OnStartTick(IMatchController<TMatchData, TPlayerIndex, TPlayer> matchController, TMatchData matchData, int tick);
    void OnEndTick(IMatchController<TMatchData, TPlayerIndex, TPlayer> matchController, TMatchData matchData, int tick);
}