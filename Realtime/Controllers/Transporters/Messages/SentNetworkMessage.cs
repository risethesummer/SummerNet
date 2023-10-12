namespace Realtime.Controllers.Transporters.Messages;

public readonly struct SentNetworkMessage<TPlayerIndex, TData> where TPlayerIndex : unmanaged
{
    public ushort Opcode { get; init; }
    public TData Payload { get; init; }
    public TPlayerIndex AssociatedClient { get; init; }
    public MessageType MessageType { get; init; }
}


// Id => Length
// Data