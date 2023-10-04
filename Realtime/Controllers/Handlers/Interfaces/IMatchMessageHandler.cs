using Realtime.Data;
using Realtime.Networks;

namespace Realtime.Controllers.Handlers.Interfaces;

public interface IMatchMessageHandler<TMessageData, TMatchData, TPlayerIndex, TPlayer> 
    where TMessageData : unmanaged, INetworkPayload
    where TPlayer : PlayerData<TPlayerIndex>
    where TMatchData : MatchData<TPlayerIndex, TPlayer>
{
    void OnMessage(IMatchRunner<TMatchData, TPlayerIndex, TPlayer> matchRunner, int tick, in int messageOrderInTick,
        in NetworkMessage<TPlayerIndex, TMessageData> message);
}