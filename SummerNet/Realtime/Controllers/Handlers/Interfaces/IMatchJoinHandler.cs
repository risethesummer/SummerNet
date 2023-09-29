using SummerNet.Realtime.Data;

namespace SummerNet.Realtime.Controllers.Handlers.Interfaces;

public interface IMatchJoinHandler<TMatchData, TPlayerIndex, TPlayer> 
    where TPlayer : PlayerData<TPlayerIndex>
    where TMatchData : MatchData<TPlayerIndex, TPlayer>
{
    void OnJoin(IMatchController<TMatchData, TPlayerIndex, TPlayer> matchController, 
        TMatchData matchData, TPlayer playerData, int tick);
}