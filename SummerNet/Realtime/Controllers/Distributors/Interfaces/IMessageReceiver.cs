using SummerNet.Realtime.Networks;

namespace SummerNet.Realtime.Controllers.Distributors.Interfaces;

public interface IMessageReceiver<in TPlayerIndex>
{
    T WaitForMessage<T>(uint opcode, int count);
    T WaitForMessage<T>(uint opcode, TPlayerIndex target);
    int Flush<T>(ref Span<NetworkMessage<T>> result) where T : unmanaged;
}