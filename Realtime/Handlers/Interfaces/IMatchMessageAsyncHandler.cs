using Realtime.Data;
using Realtime.Transporters.Messages;

namespace Realtime.Handlers.Interfaces;

public interface IMatchMessageAsyncHandler<TMessageData, TMatchData, TPlayerIndex, TAuthData, TPlayer> 
    where TPlayer : PlayerData<TPlayerIndex, TAuthData>
    where TMatchData : MatchData<TPlayerIndex, TAuthData, TPlayer>
{
    ValueTask OnMessage(ReceivedNetworkMessage<TPlayerIndex, TMessageData> message, CancellationToken cancellationToken);
}