using Realtime.Data;

namespace Realtime.Controllers.Match.Interfaces;

public interface IMatchTickHandler<TMatchData, TPlayerIndex, TAuthData, TPlayer> 
    where TPlayer : PlayerData<TPlayerIndex, TAuthData>
    where TMatchData : MatchData<TPlayerIndex, TAuthData, TPlayer>
    where TPlayerIndex : unmanaged
{
    /// <summary>
    /// Called when a tick is being processed \n
    /// Should be used to setup some tasks before handling incoming messages
    /// </summary>
    /// <param name="matchContext"></param>
    /// <param name="tick"></param>
    void OnStartTick(IMatchContext<TMatchData, TPlayerIndex, TAuthData, TPlayer> matchContext, ulong tick);
    /// <summary>
    /// Called when a tick is being ended \n
    /// Should be used to dispose resources after handling all messages
    /// </summary>
    /// <param name="matchContext"></param>
    /// <param name="tick"></param>
    void OnEndTick(IMatchContext<TMatchData, TPlayerIndex, TAuthData, TPlayer> matchContext, ulong tick);
}