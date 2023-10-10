using Realtime.Utils.Buffers;
using Realtime.Utils.Factory;

namespace Realtime.Networks;

public readonly struct SentMessage<TPlayerIndex> : IDisposable where TPlayerIndex : unmanaged, INetworkIndex
{ 
    public MessageType MessageType { get; init; }
    public TPlayerIndex Target { get; init; }
    public BufferPointer<byte> Payload { get; init; }
    public void Dispose()
    {
        Payload.Dispose();
    }
}