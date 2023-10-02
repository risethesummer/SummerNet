using Realtime.Data;

namespace Realtime.Controllers.Handlers.Interfaces;

public interface IMatchLeaveHandler<TMatchData, TPlayerIndex, TPlayer> 
    where TPlayer : PlayerData<TPlayerIndex>
    where TMatchData : MatchData<TPlayerIndex, TPlayer>
{
    /// <summary>
    /// </summary>
    /// <param name="matchController"></param>
    /// <param name="playerData"></param>
    /// <param name="tick"></param>
    /// <returns>Should remove the player from the match</returns>
    Task<bool> OnLeave(IMatchController<TMatchData, TPlayerIndex, TPlayer> matchController, TPlayer playerData, int tick);
}