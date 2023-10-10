using Realtime.Data;
using Realtime.Networks;

namespace Realtime.Controllers.Match.Interfaces;

public interface IMatchLeaveHandler<TMatchData, TPlayerIndex, TAuthData, TPlayer> 
    where TPlayer : PlayerData<TPlayerIndex, TAuthData>
    where TMatchData : MatchData<TPlayerIndex, TAuthData, TPlayer>
    where TPlayerIndex : unmanaged, INetworkIndex
{
    /// <summary>
    /// </summary>
    /// <param name="matchRunner"></param>
    /// <param name="playerData"></param>
    /// <param name="tick"></param>
    /// <returns>Should remove the player from the match</returns>
    ValueTask<bool> OnLeave(IMatchRunner<TMatchData, TPlayerIndex, TAuthData, TPlayer> matchRunner, TPlayer playerData, int tick);
}