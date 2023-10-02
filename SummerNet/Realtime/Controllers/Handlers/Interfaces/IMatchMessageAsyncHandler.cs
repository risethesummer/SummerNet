using SummerNet.Realtime.Data;
using SummerNet.Realtime.Networks;

namespace SummerNet.Realtime.Controllers.Handlers.Interfaces;

public interface IMatchMessageAsyncHandler<TMessageData, TMatchData, TPlayerIndex, TPlayer> 
    where TMessageData : unmanaged
    where TPlayer : PlayerData<TPlayerIndex>
    where TMatchData : MatchData<TPlayerIndex, TPlayer>
{
    Task OnMessage(IMatchController<TMatchData, TPlayerIndex, TPlayer> matchController, int tick, 
        in NetworkMessage<TMessageData> message);
}