using Realtime.Controllers.Transporters.Interfaces;
using Realtime.Data;

namespace Realtime.Controllers.Handlers.Interfaces;

public interface IMatchRunner<TMatchData, TPlayerIndex, TPlayer> : 
    ITransporter<TPlayerIndex, TPlayer> where TPlayer : PlayerData<TPlayerIndex>
    where TMatchData : MatchData<TPlayerIndex, TPlayer>
{
    TMatchData MatchData { get; }
    void RegisterInitializeHandler(IMatchInitHandler<TMatchData, TPlayerIndex, TPlayer> handler);
    void RegisterMatchJoinHandler(IMatchJoinHandler<TMatchData, TPlayerIndex, TPlayer> handler);
    void RegisterMatchLeaveHandler(IMatchLeaveHandler<TMatchData, TPlayerIndex, TPlayer> handler);
    void RegisterMatchTickHandler(IMatchTickHandler<TMatchData, TPlayerIndex, TPlayer> handler);
    void RegisterMatchShutdownHandler(IMatchShutdownHandler<TMatchData, TPlayerIndex, TPlayer> handler);
    ValueTask<TMatchData> StartMatch();
    ValueTask<TMatchData> Shutdown();
}