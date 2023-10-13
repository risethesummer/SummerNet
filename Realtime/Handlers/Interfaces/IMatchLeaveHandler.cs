using Realtime.Data;

namespace Realtime.Handlers.Interfaces;

public interface IMatchLeaveHandler<TMatchData, TPlayerIndex, TAuthData, TPlayer> 
    where TPlayer : PlayerData<TPlayerIndex, TAuthData>, new()
    where TMatchData : MatchData<TPlayerIndex, TAuthData, TPlayer>
{
    /// <summary>
    /// </summary>
    /// <param name="matchContext"></param>
    /// <param name="playerData"></param>
    /// <param name="tick"></param>
    /// <returns>Should remove the player from the match</returns>
    ValueTask<bool> OnLeave(IMatchContext<TMatchData, TPlayerIndex, TAuthData, TPlayer> matchContext, TPlayer playerData, int tick);
}