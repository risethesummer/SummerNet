using Realtime.Data;

namespace Realtime.Controllers.Handlers.Interfaces;

public interface IMatchJoinHandler<TMatchData, TPlayerIndex, TPlayer> 
    where TPlayer : PlayerData<TPlayerIndex>
    where TMatchData : MatchData<TPlayerIndex, TPlayer>
{
    /// <summary>
    /// </summary>
    /// <param name="matchRunner"></param>
    /// <param name="playerData"></param>
    /// <param name="tick"></param>
    /// <returns>The player data to be added (null to refuse player)</returns>
    ValueTask<TPlayer> OnJoin(IMatchRunner<TMatchData, TPlayerIndex, TPlayer> matchRunner, TPlayer playerData, int tick);
}