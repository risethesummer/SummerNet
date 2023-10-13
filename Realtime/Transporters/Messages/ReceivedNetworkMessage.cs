namespace Realtime.Transporters.Messages;

public readonly struct ReceivedNetworkMessage<TPlayerIndex, TData>
{
    public ushort Opcode { get; init; }
    public ulong Tick { get; init; }
    public ushort OrderInTick { get; init; }
    public TData Payload { get; init; }
    public TPlayerIndex AssociatedClient { get; init; }
    public MessageType MessageType { get; init; }
}