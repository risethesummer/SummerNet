namespace Realtime.Transporters.Payloads;

public readonly struct NetworkIndex
{
    public int Value { get; init; }
    public bool IsValid => Value > 0;
    public static NetworkIndex Invalid => default;

    public NetworkIndex(int value)
    {
        Value = value;
    }
    public int SequenceIndex => Value - 1;
}