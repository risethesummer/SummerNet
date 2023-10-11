namespace Realtime.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class MessageHandlerAttribute : Attribute
{
    public readonly int Opcode;
    public MessageHandlerAttribute(int opcode)
    {
        Opcode = opcode;
    }
}