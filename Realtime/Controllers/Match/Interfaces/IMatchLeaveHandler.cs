using Realtime.Data;

namespace Realtime.Controllers.Match.Interfaces;

public interface IMatchLeaveHandler<TMatchData, TPlayerIndex, TAuthData, TPlayer> 
    where TPlayer : PlayerData<TPlayerIndex, TAuthData>
    where TMatchData : MatchData<TPlayerIndex, TAuthData, TPlayer>
    where TPlayerIndex : unmanaged
{
    /// <summary>
    /// </summary>
    /// <param name="matchContext"></param>
    /// <param name="playerData"></param>
    /// <param name="tick"></param>
    /// <returns>Should remove the player from the match</returns>
    ValueTask<bool> OnLeave(IMatchContext<TMatchData, TPlayerIndex, TAuthData, TPlayer> matchContext, TPlayer playerData, int tick);
}