using Realtime.Controllers.Handlers.Interfaces;
using Realtime.Data;

namespace Realtime.Controllers.Setup.Interfaces;

//Only implement 1 time
public interface IMatchSetupHandler<TMatchData, TPlayerIndex, TPlayer>
    where TPlayer : PlayerData<TPlayerIndex>
    where TMatchData : MatchData<TPlayerIndex, TPlayer>
{
    Task<IMatchDataProvider<TMatchData, TPlayerIndex, TPlayer>> OnSetup(
        IMatchController<TMatchData, TPlayerIndex, TPlayer> machController);
}