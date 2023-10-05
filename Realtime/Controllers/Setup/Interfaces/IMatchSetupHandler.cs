using Realtime.Controllers.Handlers.Interfaces;
using Realtime.Data;
using Realtime.Networks;

namespace Realtime.Controllers.Setup.Interfaces;

//Only implement 1 time
public interface IMatchSetupHandler<TMatchData, TPlayerIndex, TPlayer>
    where TPlayer : PlayerData<TPlayerIndex>
    where TMatchData : MatchData<TPlayerIndex, TPlayer>
    where TPlayerIndex : unmanaged, INetworkIndex
{
    Task<IMatchDataProvider<TMatchData, TPlayerIndex, TPlayer>> OnSetup(
        IMatchRunner<TMatchData, TPlayerIndex, TPlayer> machRunner);
}