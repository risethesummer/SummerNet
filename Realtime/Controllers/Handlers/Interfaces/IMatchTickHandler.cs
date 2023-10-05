using Realtime.Data;
using Realtime.Networks;

namespace Realtime.Controllers.Handlers.Interfaces;

public interface IMatchTickHandler<TMatchData, TPlayerIndex, TPlayer> 
    where TPlayer : PlayerData<TPlayerIndex>
    where TMatchData : MatchData<TPlayerIndex, TPlayer>
    where TPlayerIndex : unmanaged, INetworkIndex
{
    /// <summary>
    /// Called when a tick is being processed \n
    /// Should be used to setup some tasks before handling incoming messages
    /// </summary>
    /// <param name="matchRunner"></param>
    /// <param name="tick"></param>
    void OnStartTick(IMatchRunner<TMatchData, TPlayerIndex, TPlayer> matchRunner, int tick);
    /// <summary>
    /// Called when a tick is being ended \n
    /// Should be used to dispose resources after handling all messages
    /// </summary>
    /// <param name="matchRunner"></param>
    /// <param name="tick"></param>
    void OnEndTick(IMatchRunner<TMatchData, TPlayerIndex, TPlayer> matchRunner, int tick);
}