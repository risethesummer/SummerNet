using Realtime.Transporters.Messages;

namespace Realtime.Transporters.Interfaces;

public interface IMessageReceiver<TPlayerIndex>
{
    // Task<T> WaitForMessage<T>(uint opcode, int count);
    ValueTask<ReadOnlyMemory<RawReceivedNetworkMessage<TPlayerIndex>>> FlushReceivedMessagesAsync(CancellationToken cancellationToken);
    ValueTask StartReceivingMessagesAsync(CancellationToken cancellationToken);
}