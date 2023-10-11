using Realtime.Controllers.Transporters.Messages;
using Realtime.Data;

namespace Realtime.Controllers.Match.Interfaces;

public interface IMatchMessageAsyncHandler<TMessageData, TMatchData, TPlayerIndex, TAuthData, TPlayer> 
    where TMessageData : unmanaged, INetworkPayload
    where TPlayer : PlayerData<TPlayerIndex, TAuthData>
    where TMatchData : MatchData<TPlayerIndex, TAuthData, TPlayer>
    where TPlayerIndex : unmanaged
{
    ValueTask OnMessage(IMatchContext<TMatchData, TPlayerIndex, TAuthData, TPlayer> matchContext, 
        ReceivedNetworkMessage<TPlayerIndex, TMessageData> message, CancellationToken cancellationToken);
}