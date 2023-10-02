namespace Realtime.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class MessageHandlerAttribute : Attribute
{
    public readonly int Opcode;
    public readonly string? After;
    public MessageHandlerAttribute(int opcode, string? after)
    {
        Opcode = opcode;
        After = after;
    }
}