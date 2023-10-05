using Realtime.Data;
using Realtime.Networks;

namespace Realtime.Controllers.Handlers.Interfaces;

public interface IMatchMessageAsyncHandler<TMessageData, TMatchData, TPlayerIndex, TPlayer> 
    where TMessageData : unmanaged, INetworkPayload
    where TPlayer : PlayerData<TPlayerIndex>
    where TMatchData : MatchData<TPlayerIndex, TPlayer>
    where TPlayerIndex : unmanaged, INetworkIndex
{
    ValueTask OnMessage(IMatchRunner<TMatchData, TPlayerIndex, TPlayer> matchRunner, in int tick, in int messageOrderInTick, 
        in NetworkMessage<TPlayerIndex, TMessageData> message);
}