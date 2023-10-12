namespace Realtime.Controllers.Transporters.Payloads;

public readonly struct NetworkIndex
{
    public ulong Value { get; init; }
    public bool IsValid => Value > 0;
    public static NetworkIndex Invalid => default;
    public ulong SequenceIndex => Value - 1;
}