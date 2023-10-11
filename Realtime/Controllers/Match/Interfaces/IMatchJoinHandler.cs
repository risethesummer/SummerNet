using Realtime.Controllers.Transporters.Messages;
using Realtime.Data;

namespace Realtime.Controllers.Match.Interfaces;

public interface IMatchJoinHandler<TMatchData, TPlayerIndex, TAuthData, TPlayer> 
    where TPlayer : PlayerData<TPlayerIndex, TAuthData>
    where TMatchData : MatchData<TPlayerIndex, TAuthData, TPlayer>
    where TPlayerIndex : unmanaged, INetworkIndex
{
    /// <summary>
    /// </summary>
    /// <param name="matchRunner"></param>
    /// <param name="playerData"></param>
    /// <param name="tick"></param>
    /// <returns>The player data to be added (null to refuse player)</returns>
    ValueTask<TPlayer> OnJoin(IMatchRunner<TMatchData, TPlayerIndex, TAuthData, TPlayer> matchRunner, TPlayer playerData, int tick);
}