using Realtime.Data;
using Realtime.Networks;

namespace Realtime.Controllers.Match.Interfaces;

public interface IMatchMessageAsyncHandler<TMessageData, TMatchData, TPlayerIndex, TAuthData, TPlayer> 
    where TMessageData : unmanaged, INetworkPayload
    where TPlayer : PlayerData<TPlayerIndex, TAuthData>
    where TMatchData : MatchData<TPlayerIndex, TAuthData, TPlayer>
    where TPlayerIndex : unmanaged, INetworkIndex
{
    ValueTask OnMessage(IMatchRunner<TMatchData, TPlayerIndex, TAuthData, TPlayer> matchRunner, int tick, int messageOrderInTick, 
        NetworkMessage<TPlayerIndex, TMessageData> message, CancellationToken cancellationToken);
}