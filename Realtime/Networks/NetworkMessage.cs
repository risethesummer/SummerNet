using Realtime.Utils.Buffers;

namespace Realtime.Networks;

public readonly struct NetworkMessage<TPlayerIndex, TData> 
    where TData : INetworkPayload where TPlayerIndex : unmanaged, INetworkIndex
{
    public ushort Opcode { get; init; }
    public TData Payload { get; init; }
    public TPlayerIndex AssociatedClient { get; init; }
    public MessageType MessageType { get; init; }
}

public readonly struct ReceivedNetworkMessage<TPlayerIndex> : IDisposable where TPlayerIndex : unmanaged, INetworkIndex
{
    public ushort MessageId { get; init; }
    public ushort Opcode { get; init; }
    public TPlayerIndex Owner { get; init; }
    public BufferPointer<byte> Payload { get; init; }
    public void Dispose()
    {
        Payload.Dispose();
    }
}

public static class NetworkMessageCommonInfo
{
    public const int HeaderArgumentSize = sizeof(ushort);
    public static class ServerMsgArgumentPosition
    {
        public const int HeaderValuesCount = 3;
        public const int HeaderSize = HeaderValuesCount * HeaderArgumentSize;
        public const int MessageId = 0;
        public const int Opcode = MessageId + HeaderArgumentSize;
        public const int PayloadLength = Opcode + HeaderArgumentSize;
        public const int Payload = PayloadLength + HeaderArgumentSize;
    }
    
    public static class ClientMsgArgumentPosition
    {
        public const int HeaderValuesCount = 3;
        public const int HeaderSize = HeaderValuesCount * HeaderArgumentSize;
        // public static readonly Range OwnerIdx = Range.EndAt(HeaderArgumentSize);
        public static readonly Range MessageId = Range.EndAt(HeaderArgumentSize); 
        public static readonly Range Opcode = new(MessageId.End, HeaderArgumentSize * 2);
        public static readonly Range PayloadLength = new(Opcode.End, HeaderArgumentSize * 3); 
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