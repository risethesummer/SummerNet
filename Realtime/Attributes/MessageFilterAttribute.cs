namespace Realtime.Attributes;

// Add before NetworkData
// All filter will be registered manually
[AttributeUsage(AttributeTargets.Struct)]
public class MessageFilterAttribute : Attribute
{
    public readonly Type Filter;
    public MessageFilterAttribute(Type filter)
    {
        Filter = filter;
    }
}