using MemoryPack;

namespace Realtime.Networks;

public partial class MessageDecoder<TPlayerIndex> where TPlayerIndex : unmanaged, INetworkIndex
{
    public NetworkMessage<TPlayerIndex, INetworkPayload>? Decode(ReadOnlySpan<byte> buffer, out int newIndex)
    {
        var id = BitConverter.ToUInt16(buffer[..2]);
        newIndex = NetworkMessageHelper.GetPayloadLength(id) + 2; //Remove message id segment
        return MemoryPackSerializer.Deserialize<NetworkMessage<TPlayerIndex, INetworkPayload>>(buffer[2..newIndex]);
    }
}