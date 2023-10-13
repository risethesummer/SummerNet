using Realtime.Transporters.Messages;

namespace Realtime.Handlers.Interfaces;

public interface IMatchMessageHandler<TMessageData, TPlayerIndex>
{
    void OnMessage(in ReceivedNetworkMessage<TPlayerIndex, TMessageData> message);
}