using Realtime.Data;
using Realtime.Networks;

namespace Realtime.Controllers.Handlers.Interfaces;

public interface IMatchMessageHandler<TMessageData, TMatchData, TPlayerIndex, TAuthData, TPlayer> 
    where TMessageData : unmanaged, INetworkPayload
    where TPlayer : PlayerData<TPlayerIndex, TAuthData>
    where TMatchData : MatchData<TPlayerIndex, TAuthData, TPlayer>
    where TPlayerIndex : unmanaged, INetworkIndex
{
    void OnMessage(IMatchRunner<TMatchData, TPlayerIndex, TAuthData, TPlayer> matchRunner, int tick, in int messageOrderInTick,
        in NetworkMessage<TPlayerIndex, TMessageData> message);
}