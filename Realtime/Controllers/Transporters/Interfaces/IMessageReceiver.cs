using Realtime.Controllers.Transporters.Messages;

namespace Realtime.Controllers.Transporters.Interfaces;

public interface IMessageReceiver<TPlayerIndex> where TPlayerIndex : unmanaged
{
    // Task<T> WaitForMessage<T>(uint opcode, int count);
    ValueTask<ReadOnlyMemory<RawReceivedNetworkMessage<TPlayerIndex>>> FlushReceivedMessagesAsync(CancellationToken cancellationToken);
    ValueTask StartReceivingMessagesAsync(CancellationToken cancellationToken);
}