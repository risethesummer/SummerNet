using Realtime.Controllers.Transporters.Messages;

namespace Realtime.Controllers.Transporters.Interfaces;

public interface IMessageReceiver<TPlayerIndex> where TPlayerIndex : unmanaged, INetworkIndex
{
    // Task<T> WaitForMessage<T>(uint opcode, int count);
    ValueTask<ReadOnlyMemory<ReceivedNetworkMessage<TPlayerIndex>>> FlushReceivedMessagesAsync(CancellationToken cancellationToken);
    ValueTask StartReceivingMessagesAsync(CancellationToken cancellationToken);
}