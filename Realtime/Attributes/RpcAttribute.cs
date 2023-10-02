namespace Realtime.Attributes;

public enum RpcDirection
{
    ServerToClient, ClientToServer
}

[AttributeUsage(AttributeTargets.Method)]
public class RpcAttribute : Attribute
{
}