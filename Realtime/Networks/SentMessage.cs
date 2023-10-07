using Realtime.Utils.Buffers;
using Realtime.Utils.Factory;

namespace Realtime.Networks;

public readonly struct SentMessage<TPlayerIndex> : IDisposable
{ 
    public MessageType MessageType { get; init; }
    public TPlayerIndex Target { get; init; }
    public AutoDisposableData<ReadOnlyMemory<byte>, UnmanagedMemoryManager<byte>> DisposableDataWrapper { get; init; }
    public void Dispose()
    {
        DisposableDataWrapper.Dispose();
    }
}