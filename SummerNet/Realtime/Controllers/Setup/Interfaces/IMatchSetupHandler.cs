using SummerNet.Realtime.Controllers.Handlers.Interfaces;
using SummerNet.Realtime.Data;

namespace SummerNet.Realtime.Controllers.Setup.Interfaces;

//Just implement 1 time
public interface IMatchSetupHandler<TMatchData, TPlayerIndex, TPlayer>
    where TPlayer : PlayerData<TPlayerIndex>
    where TMatchData : MatchData<TPlayerIndex, TPlayer>
{
    Task<IMatchDataProvider<TMatchData, TPlayerIndex, TPlayer>> OnSetup(
        IMatchController<TMatchData, TPlayerIndex, TPlayer> machController);
}