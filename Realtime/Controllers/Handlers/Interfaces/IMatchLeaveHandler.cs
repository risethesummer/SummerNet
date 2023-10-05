using Realtime.Data;
using Realtime.Networks;

namespace Realtime.Controllers.Handlers.Interfaces;

public interface IMatchLeaveHandler<TMatchData, TPlayerIndex, TPlayer> 
    where TPlayer : PlayerData<TPlayerIndex>
    where TMatchData : MatchData<TPlayerIndex, TPlayer>
    where TPlayerIndex : unmanaged, INetworkIndex
{
    /// <summary>
    /// </summary>
    /// <param name="matchRunner"></param>
    /// <param name="playerData"></param>
    /// <param name="tick"></param>
    /// <returns>Should remove the player from the match</returns>
    ValueTask<bool> OnLeave(IMatchRunner<TMatchData, TPlayerIndex, TPlayer> matchRunner, TPlayer playerData, int tick);
}