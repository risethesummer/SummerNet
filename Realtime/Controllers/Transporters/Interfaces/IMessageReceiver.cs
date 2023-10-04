using Realtime.Controllers.Transporters.Impl;
using Realtime.Networks;

namespace Realtime.Controllers.Transporters.Interfaces;

public interface IMessageReceiver<TPlayerIndex>
{
    // Task<T> WaitForMessage<T>(uint opcode, int count);
    IEnumerable<NetworkMessage<TPlayerIndex, INetworkPayload>> FlushReceivedMessages();
    ValueTask StartFlushingReceivedMessages(CancellationToken cancellationToken);
}