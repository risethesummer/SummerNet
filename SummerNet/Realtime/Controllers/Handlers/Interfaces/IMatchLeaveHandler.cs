using SummerNet.Realtime.Data;

namespace SummerNet.Realtime.Controllers.Handlers.Interfaces;

public interface IMatchLeaveHandler<TMatchData, TPlayerIndex, TPlayer> 
    where TPlayer : PlayerData<TPlayerIndex>
    where TMatchData : MatchData<TPlayerIndex, TPlayer>
{
    void OnLeave(IMatchController<TMatchData, TPlayerIndex, TPlayer> matchController, 
        TMatchData matchData, TPlayer playerData, int tick);
}