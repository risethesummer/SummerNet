using Realtime.Controllers.Transporters.Interfaces;
using Realtime.Controllers.Transporters.Messages;
using Realtime.Data;

namespace Realtime.Controllers.Match.Interfaces;

public interface IMatchRunner<TMatchData, TPlayerIndex, TAuthData, TPlayer> : 
    ITransporter<TPlayerIndex, TAuthData, TPlayer> where TPlayer : PlayerData<TPlayerIndex, TAuthData>
    where TMatchData : MatchData<TPlayerIndex, TAuthData, TPlayer>
    where TPlayerIndex : unmanaged, INetworkIndex
{
    TMatchData MatchData { get; }
    void RegisterInitializeHandler(IMatchInitHandler<TMatchData, TPlayerIndex, TAuthData, TPlayer> handler);
    void RegisterMatchJoinHandler(IMatchJoinHandler<TMatchData, TPlayerIndex, TAuthData, TPlayer> handler);
    void RegisterMatchLeaveHandler(IMatchLeaveHandler<TMatchData, TPlayerIndex, TAuthData, TPlayer> handler);
    void RegisterMatchTickHandler(IMatchTickHandler<TMatchData, TPlayerIndex, TAuthData, TPlayer> handler);
    void RegisterMatchShutdownHandler(IMatchShutdownHandler<TMatchData, TPlayerIndex, TAuthData, TPlayer> handler);
    ValueTask<TMatchData> StartMatch();
    ValueTask<TMatchData> Shutdown();
}