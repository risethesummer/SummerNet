namespace Realtime.Networks;

public struct NetworkMessage<TPlayerIndex, TData> where TData : INetworkPayload
{
    public ushort Opcode { get; init; }
    public TData Payload { get; init; }
    public TPlayerIndex Owner { get; init; }
    public TPlayerIndex Target { get; init; }
    public MessageType MessageType { get; init; }
}


public enum MessageType
{
    ServerToClient, ClientToSever, Broadcast, 
}

public interface INetworkPayload
{
    // ushort PayloadId { get; }
}

// Id => Length
// Data
public struct SentMessage<TPlayerIndex>
{
    public MessageType MessageType { get; init; }
    public TPlayerIndex Target { get; init; }
    public byte[] Data { get; init; }
}