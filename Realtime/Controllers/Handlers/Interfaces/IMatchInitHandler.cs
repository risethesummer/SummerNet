using Realtime.Data;
using Realtime.Networks;

namespace Realtime.Controllers.Handlers.Interfaces;

public interface IMatchInitHandler<TMatchData, TPlayerIndex, TPlayer> 
    where TPlayer : PlayerData<TPlayerIndex>
    where TMatchData : MatchData<TPlayerIndex, TPlayer>
    where TPlayerIndex : unmanaged, INetworkIndex
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="matchRunner"></param>
    /// <returns>Is the match initialized successfully or not (false means ending the match)</returns>
    ValueTask<bool> OnInitialize(IMatchRunner<TMatchData, TPlayerIndex, TPlayer> matchRunner);
}