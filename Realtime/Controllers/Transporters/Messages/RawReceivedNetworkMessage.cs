using Realtime.Utils.Buffers;

namespace Realtime.Controllers.Transporters.Messages;

public readonly struct RawReceivedNetworkMessage<TPlayerIndex> : IDisposable where TPlayerIndex : unmanaged
{
    public ushort Opcode { get; init; }
    public TPlayerIndex Owner { get; init; }
    public BufferPointer<byte> Payload { get; init; }
    public void Dispose()
    {
        Payload.Dispose();
    }
}