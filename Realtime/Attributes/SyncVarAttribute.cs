namespace Realtime.Attributes;

[AttributeUsage(AttributeTargets.Field)]
public class SyncVarAttribute : Attribute
{
    public string? GenName;
    public bool TickAligned;
    public SyncVarAttribute(string? genName, bool tickAligned = true)
    {
        GenName = genName;
        TickAligned = tickAligned;
    }
}