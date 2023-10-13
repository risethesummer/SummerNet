using Realtime.Data;

namespace Realtime.Handlers.Interfaces;

public interface IMatchJoinHandler<TMatchData, TPlayerIndex, TAuthData, TPlayer> 
    where TPlayer : PlayerData<TPlayerIndex, TAuthData>, new()
    where TMatchData : MatchData<TPlayerIndex, TAuthData, TPlayer>
{
    /// <summary>
    /// </summary>
    /// <param name="matchContext"></param>
    /// <param name="playerData"></param>
    /// <param name="tick"></param>
    /// <returns>The player data to be added (null to refuse player)</returns>
    ValueTask<TPlayer> OnJoin(IMatchContext<TMatchData, TPlayerIndex, TAuthData, TPlayer> matchContext, TPlayer playerData, int tick);
}