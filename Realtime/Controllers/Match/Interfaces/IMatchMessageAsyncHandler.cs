using Realtime.Controllers.Transporters.Messages;
using Realtime.Data;

namespace Realtime.Controllers.Match.Interfaces;

public interface IMatchMessageAsyncHandler<TMessageData, TMatchData, TPlayerIndex, TAuthData, TPlayer> 
    where TPlayer : PlayerData<TPlayerIndex, TAuthData>
    where TMatchData : MatchData<TPlayerIndex, TAuthData, TPlayer>
    where TPlayerIndex : unmanaged
{
    ValueTask OnMessage(ReceivedNetworkMessage<TPlayerIndex, TMessageData> message, CancellationToken cancellationToken);
}