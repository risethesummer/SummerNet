using Realtime.Utils.Buffers;

namespace Realtime.Controllers.Transporters.Messages;

public readonly struct ReceivedNetworkMessage<TPlayerIndex> : IDisposable where TPlayerIndex : unmanaged, INetworkIndex
{
    public ushort Opcode { get; init; }
    public TPlayerIndex Owner { get; init; }
    public BufferPointer<byte> Payload { get; init; }
    public void Dispose()
    {
        Payload.Dispose();
    }
}