using SummerNet.Realtime.Data;
using SummerNet.Realtime.Networks;

namespace SummerNet.Realtime.Controllers.Handlers.Interfaces;

public interface IMatchMessageHandler<TMessageData, TMatchData, TPlayerIndex, TPlayer> 
    where TMessageData : unmanaged
    where TPlayer : PlayerData<TPlayerIndex>
    where TMatchData : MatchData<TPlayerIndex, TPlayer>
{
    object OnMessage(IMatchController<TMatchData, TPlayerIndex, TPlayer> matchController, 
        TMatchData matchData, int tick, in NetworkMessage<TMessageData> message);
}