namespace Realtime.Networks;

public struct NetworkMessage<T> where T : unmanaged
{
    public uint Opcode { get; init; }
    public T Value { get; init; }
}
public struct NetworkMessage<TPlayerIndex, TData> where TData : unmanaged
{
    public NetworkMessage<TData> Payload;
    public TPlayerIndex Owner;
    public TPlayerIndex Target;
}