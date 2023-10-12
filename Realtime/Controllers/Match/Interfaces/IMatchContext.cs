using Realtime.Attributes;
using Realtime.Controllers.Transporters.Interfaces;
using Realtime.Data;

namespace Realtime.Controllers.Match.Interfaces;


/// <summary>
/// The context of a match, you may not interact with this class directly
/// but using attributes <see cref="RpcAttribute"/>, <see cref="SyncVarAttribute"/>,
/// <see cref="MessageFilterAttribute"/>,...
/// </summary>
/// <typeparam name="TMatchData">Your custom data inherits from <see cref="MatchData{TPlayerIndex,TAuthData,TPlayer}"/></typeparam>
/// <typeparam name="TPlayerIndex">TId in your <see cref="PlayerData{TId, TAuthData}"/></typeparam>
/// <typeparam name="TAuthData">TAuthData in your <see cref="PlayerData{TId, TAuthData}"/></typeparam>
/// <typeparam name="TPlayer">Your custom player data inherits from <see cref="PlayerData{TId, TAuthData}"/></typeparam>
public interface IMatchContext<TMatchData, TPlayerIndex, TAuthData, TPlayer> : 
    ITransporter<TPlayerIndex, TAuthData, TPlayer>, IMatchObjectManager<TPlayerIndex>,
    IPlayerAcceptor<TPlayerIndex, TAuthData, TPlayer> 
    where TPlayer : PlayerData<TPlayerIndex, TAuthData>, new()
    where TMatchData : MatchData<TPlayerIndex, TAuthData, TPlayer>
    where TPlayerIndex : unmanaged
{
    TMatchData MatchData { get; }
    void RegisterInitializeHandler(IMatchInitHandler<TMatchData, TPlayerIndex, TAuthData, TPlayer> handler);
    void RegisterMatchJoinHandler(IMatchJoinHandler<TMatchData, TPlayerIndex, TAuthData, TPlayer> handler);
    void RegisterMatchLeaveHandler(IMatchLeaveHandler<TMatchData, TPlayerIndex, TAuthData, TPlayer> handler);
    void RegisterMatchTickHandler(IMatchTickHandler<TMatchData, TPlayerIndex, TAuthData, TPlayer> handler);
    void RegisterMatchShutdownHandler(IMatchShutdownHandler<TMatchData, TPlayerIndex, TAuthData, TPlayer> handler);
    ValueTask<TMatchData> StartMatch(CancellationToken cancellationToken);
    ValueTask<TMatchData> Shutdown();
}