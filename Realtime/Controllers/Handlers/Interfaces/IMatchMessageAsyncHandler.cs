using Realtime.Data;
using Realtime.Networks;

namespace Realtime.Controllers.Handlers.Interfaces;

public interface IMatchMessageAsyncHandler<TMessageData, TMatchData, TPlayerIndex, TAuthData, TPlayer> 
    where TMessageData : unmanaged, INetworkPayload
    where TPlayer : PlayerData<TPlayerIndex, TAuthData>
    where TMatchData : MatchData<TPlayerIndex, TAuthData, TPlayer>
    where TPlayerIndex : unmanaged, INetworkIndex
{
    ValueTask OnMessage(IMatchRunner<TMatchData, TPlayerIndex, TAuthData, TPlayer> matchRunner, in int tick, in int messageOrderInTick, 
        in NetworkMessage<TPlayerIndex, TMessageData> message);
}