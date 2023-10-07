using System.Buffers;
using MemoryPack;

namespace Realtime.Networks;

[MemoryPackable]
public readonly struct NetworkMessage<TPlayerIndex, TData> 
    where TData : INetworkPayload where TPlayerIndex : unmanaged, INetworkIndex
{
    public ushort Opcode { get; init; }
    public TData Payload { get; init; }
    public TPlayerIndex AssociatedClient { get; init; }
    public MessageType MessageType { get; init; }
}

public readonly struct RawNetworkMessage<TPlayerIndex> where TPlayerIndex : unmanaged, INetworkIndex
{
    public ushort MessageId { get; init; }
    public ushort Opcode { get; init; }
    public TPlayerIndex Owner { get; init; }
    public ReadOnlyMemory<byte> Payload { get; init; }
}

public static class NetworkMessageCommonInfo
{
    public const int HeaderArgumentSize = sizeof(ushort);
    public static class ServerMsgArgumentPosition
    {
        public const int HeaderValuesCount = 2;
        public const int HeaderSize = HeaderValuesCount * HeaderArgumentSize;
        public static readonly Range Message = Range.EndAt(HeaderArgumentSize); 
        public static readonly Range Opcode = new(HeaderArgumentSize, HeaderArgumentSize * 2);
    }
    
    public static class ClientMsgArgumentPosition
    {
        public const int HeaderValuesCount = 3;
        public const int HeaderSize = HeaderValuesCount * HeaderArgumentSize;
        public static readonly Range OwnerIdx = Range.EndAt(HeaderArgumentSize);
        public static readonly Range Message = new(HeaderArgumentSize, HeaderArgumentSize * 2); 
        public static readonly Range Opcode = new(HeaderArgumentSize * 2, HeaderArgumentSize * 3);
    }
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