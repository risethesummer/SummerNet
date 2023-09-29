using SummerNet.Realtime.Controllers.Distributors.Interfaces;
using SummerNet.Realtime.Data;

namespace SummerNet.Realtime.Controllers.Handlers.Interfaces;

public interface IMatchController<TMatchData, TPlayerIndex, TPlayer> : IMessageDistributor<TPlayerIndex> 
    where TPlayer : PlayerData<TPlayerIndex>
    where TMatchData : MatchData<TPlayerIndex, TPlayer>
{
    TMatchData MatchData { get; }
    void RegisterInitializeHandler(IMatchInitHandler<TMatchData, TPlayerIndex, TPlayer> handler);
    void RegisterMatchJoinHandler(IMatchJoinHandler<TMatchData, TPlayerIndex, TPlayer> handler);
    void RegisterMatchLeaveHandler(IMatchLeaveHandler<TMatchData, TPlayerIndex, TPlayer> handler);
    void RegisterMatchTickHandler(IMatchTickHandler<TMatchData, TPlayerIndex, TPlayer> handler);
    void RegisterMatchShutdownHandler(IMatchShutdownHandler<TMatchData, TPlayerIndex, TPlayer> handler);
    void KickPlayer(TPlayerIndex target);
    void AddPlayer(TPlayer playerData);
    Task<TMatchData> Shutdown();
}