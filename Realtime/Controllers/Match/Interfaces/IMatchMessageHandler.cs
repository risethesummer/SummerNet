using Realtime.Controllers.Transporters.Messages;

namespace Realtime.Controllers.Match.Interfaces;

public interface IMatchMessageHandler<TMessageData, TPlayerIndex> where TPlayerIndex : unmanaged
{
    void OnMessage(in ReceivedNetworkMessage<TPlayerIndex, TMessageData> message);
}