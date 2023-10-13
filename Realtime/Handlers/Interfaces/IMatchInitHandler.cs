using Realtime.Data;

namespace Realtime.Handlers.Interfaces;

public interface IMatchInitHandler<TMatchData, TPlayerIndex, TAuthData, TPlayer> 
    where TPlayer : PlayerData<TPlayerIndex, TAuthData>, new()
    where TMatchData : MatchData<TPlayerIndex, TAuthData, TPlayer>
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="matchContext"></param>
    /// <returns>Is the match initialized successfully or not (false means ending the match)</returns>
    ValueTask<bool> OnInitialize(IMatchContext<TMatchData, TPlayerIndex, TAuthData, TPlayer> matchContext);
}