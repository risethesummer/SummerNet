using Realtime.Networks;

namespace Realtime.Controllers.Transporters.Interfaces;

public interface IMessageReceiver<TPlayerIndex> where TPlayerIndex : unmanaged, INetworkIndex
{
    // Task<T> WaitForMessage<T>(uint opcode, int count);
    IAsyncEnumerable<NetworkMessage<TPlayerIndex, INetworkPayload>> FlushReceivedMessagesAsync(
        CancellationToken cancellationToken);
    ValueTask StartFlushingReceivedMessagesAsync(CancellationToken cancellationToken);
}