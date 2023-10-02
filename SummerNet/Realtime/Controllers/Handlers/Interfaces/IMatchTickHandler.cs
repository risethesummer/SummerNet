using SummerNet.Realtime.Data;

namespace SummerNet.Realtime.Controllers.Handlers.Interfaces;

public interface IMatchTickHandler<TMatchData, TPlayerIndex, TPlayer> 
    where TPlayer : PlayerData<TPlayerIndex>
    where TMatchData : MatchData<TPlayerIndex, TPlayer>
{
    /// <summary>
    /// Called when a tick is being processed \n
    /// Should be used to setup some tasks before handling incoming messages
    /// </summary>
    /// <param name="matchController"></param>
    /// <param name="tick"></param>
    void OnStartTick(IMatchController<TMatchData, TPlayerIndex, TPlayer> matchController, int tick);
    /// <summary>
    /// Called when a tick is being ended \n
    /// Should be used to dispose resources after handling all messages
    /// </summary>
    /// <param name="matchController"></param>
    /// <param name="tick"></param>
    void OnEndTick(IMatchController<TMatchData, TPlayerIndex, TPlayer> matchController, int tick);
}