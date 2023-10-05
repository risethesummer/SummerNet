using Realtime.Utils.Extensions;

namespace Realtime.Networks;

public class MessageEncoder
{
    public byte[] EncodeNonAlloc<TPlayerIndex, TData>(in NetworkMessage<TPlayerIndex, TData> msg)
        where TData : unmanaged, INetworkPayload where TPlayerIndex : unmanaged, INetworkIndex
    {
        Span<byte> idSpan = stackalloc byte[sizeof(ushort)];
        var msgId = NetworkMessageHelper.GetPayloadId<TData>();
        BitConverter.TryWriteBytes(idSpan, msgId);
        return MemoryPackExtensions.SerializeWith(msg, idSpan);
    }
}