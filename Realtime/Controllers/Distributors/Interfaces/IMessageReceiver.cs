using Realtime.Networks;

namespace Realtime.Controllers.Distributors.Interfaces;

public interface IMessageReceiver<TPlayerIndex>
{
    // Task<T> WaitForMessage<T>(uint opcode, int count);
    Task<T> WaitForMessage<T>(uint opcode, TPlayerIndex target);
    int Flush<T>(ref Span<NetworkMessage<TPlayerIndex, T>> result) where T : unmanaged;
}