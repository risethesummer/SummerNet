namespace SummerNet.Realtime.Networks;

public struct NetworkMessage<T> where T : unmanaged
{
    public uint Opcode { get; init; }
    public T Value { get; init; }
}