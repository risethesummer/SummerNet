namespace Realtime.Controllers.Transporters.Messages;

public readonly struct NetworkMessage<TPlayerIndex, TData> 
    where TData : INetworkPayload where TPlayerIndex : unmanaged, INetworkIndex
{
    public ushort Opcode { get; init; }
    public TData Payload { get; init; }
    public TPlayerIndex AssociatedClient { get; init; }
    public MessageType MessageType { get; init; }
}

// Id => Length
// Data