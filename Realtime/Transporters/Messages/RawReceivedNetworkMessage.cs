using Realtime.Utils.Buffers;

namespace Realtime.Transporters.Messages;

public readonly struct RawReceivedNetworkMessage<TPlayerIndex> : IDisposable
{
    public ushort Opcode { get; init; }
    public TPlayerIndex Owner { get; init; }
    public BufferPointer<byte> Payload { get; init; }
    public void Dispose()
    {
        Payload.Dispose();
    }
}