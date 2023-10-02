using SummerNet.Realtime.Data;

namespace SummerNet.Realtime.Controllers.Handlers.Interfaces;

public interface IMatchJoinHandler<TMatchData, TPlayerIndex, TPlayer> 
    where TPlayer : PlayerData<TPlayerIndex>
    where TMatchData : MatchData<TPlayerIndex, TPlayer>
{
    /// <summary>
    /// </summary>
    /// <param name="matchController"></param>
    /// <param name="playerData"></param>
    /// <param name="tick"></param>
    /// <returns>Should add the player into the match</returns>
    Task<bool> OnJoin(IMatchController<TMatchData, TPlayerIndex, TPlayer> matchController, TPlayer playerData, int tick);
}