namespace SummerNet.Realtime.Attributes;

public class MessageFilterAttribute : Attribute
{
    public readonly string? After;
    public readonly int Order;
    public readonly string[]? Focuses;
    public readonly string[]? Exceptions;
    public MessageFilterAttribute(string? after = null, int order = 0, string[]? focuses = null, string[]? exceptions = null)
    {
        After = after;
        Order = order;
        Focuses = focuses;
        Exceptions = exceptions;
    }
}