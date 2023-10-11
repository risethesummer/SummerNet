using Realtime.Utils.Buffers;

namespace Realtime.Controllers.Transporters.Messages;

public readonly struct DecodedSentMessage<TPlayerIndex> : IDisposable where TPlayerIndex : unmanaged
{ 
    public MessageType MessageType { get; init; }
    public TPlayerIndex Target { get; init; }
    public BufferPointer<byte> Payload { get; init; }
    public void Dispose()
    {
        Payload.Dispose();
    }
}