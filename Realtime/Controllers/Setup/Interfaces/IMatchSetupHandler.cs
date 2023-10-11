using Realtime.Controllers.Match.Interfaces;
using Realtime.Controllers.Transporters.Messages;
using Realtime.Data;
using Realtime.Networks;

namespace Realtime.Controllers.Setup.Interfaces;

//Only implement 1 time
public interface IMatchSetupHandler<TMatchData, TPlayerIndex, TAuthData, TPlayer>
    where TPlayer : PlayerData<TPlayerIndex, TAuthData>
    where TMatchData : MatchData<TPlayerIndex, TAuthData, TPlayer>
    where TPlayerIndex : unmanaged, INetworkIndex
{
    Task<IMatchDataProvider<TMatchData, TPlayerIndex, TPlayer>> OnSetup(
        IMatchRunner<TMatchData, TPlayerIndex, TAuthData, TPlayer> machRunner);
}