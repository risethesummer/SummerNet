using System.Buffers;
using MemoryPack;
using Realtime.Utils.Buffers;
using Realtime.Utils.Factory;

namespace Realtime.Networks;

[MemoryPackable]
public struct NetworkMessage<TPlayerIndex, TData> 
    where TData : INetworkPayload where TPlayerIndex : unmanaged, INetworkIndex
{
    public ushort Opcode { get; init; }
    public TData Payload { get; init; }
    public TPlayerIndex Owner { get; init; }
    public TPlayerIndex Target { get; init; }
    public MessageType MessageType { get; init; }
}

public interface INetworkIndex
{
}

public enum MessageType
{
    ServerToClient, ClientToSever, Broadcast, 
}

public interface INetworkPayload
{
}

// Id => Length
// Data
public struct SentMessage<TPlayerIndex> : IDisposable
{ 
    public MessageType MessageType { get; init; }
    public TPlayerIndex Target { get; init; }
    public AutoDisposableData<ReadOnlyMemory<byte>, UnmanagedMemoryManager<byte>> Data { get; init; }
    public void Dispose()
    {
        Data.Dispose();
    }
}