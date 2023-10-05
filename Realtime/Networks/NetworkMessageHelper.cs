namespace Realtime.Networks;

public static class NetworkMessageHelper
{
    // Generate length depends on opcode
    public static int GetPayloadLength(in ushort id);
    public static uint GetPayloadId<TData>() where TData : INetworkPayload;
}