using Realtime.Controllers.Transporters.Messages;
using Realtime.Data;

namespace Realtime.Controllers.Match.Interfaces;

public interface IMatchInitHandler<TMatchData, TPlayerIndex, TAuthData, TPlayer> 
    where TPlayer : PlayerData<TPlayerIndex, TAuthData>
    where TMatchData : MatchData<TPlayerIndex, TAuthData, TPlayer>
    where TPlayerIndex : unmanaged, INetworkIndex
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="matchRunner"></param>
    /// <returns>Is the match initialized successfully or not (false means ending the match)</returns>
    ValueTask<bool> OnInitialize(IMatchRunner<TMatchData, TPlayerIndex, TAuthData, TPlayer> matchRunner);
}