using Realtime.Networks;

namespace Realtime.Controllers.Transporters.Interfaces;

public interface IMessageDistributor<TPlayerIndex>
{
    ValueTask SendMessage<T>(in NetworkMessage<TPlayerIndex, T> msg,
        CancellationToken cancellationToken) where T : unmanaged, INetworkPayload;
    ValueTask FlushSentMessage(CancellationToken cancellationToken);
}
