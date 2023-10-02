using SummerNet.Realtime.Data;

namespace SummerNet.Realtime.Controllers.Handlers.Interfaces;

public interface IMatchInitHandler<TMatchData, TPlayerIndex, TPlayer> 
    where TPlayer : PlayerData<TPlayerIndex>
    where TMatchData : MatchData<TPlayerIndex, TPlayer>
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="matchController"></param>
    /// <returns>Is the match initialized successfully or not (false means ending the match)</returns>
    Task<bool> OnInitialize(IMatchController<TMatchData, TPlayerIndex, TPlayer> matchController);
}