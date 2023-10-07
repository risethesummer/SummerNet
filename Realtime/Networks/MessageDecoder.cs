using MemoryPack;

namespace Realtime.Networks;

public partial class MessageDecoder<TPlayerIndex> where TPlayerIndex : unmanaged, INetworkIndex
{
    public readonly struct DecodeResult
    {
        public ushort MessageId { get; init; }
        public ushort Opcode { get; init; }
        public ushort OwnerIndex { get; init; }
        public int EndIndex { get; init; }
    }
    public DecodeResult? Decode(in ReadOnlyMemory<byte> buffer)
    {
        if (buffer.Length <= NetworkMessageCommonInfo.ClientMsgArgumentPosition.HeaderSize)
            return null;
        var bufferSpan = buffer.Span;
        var ownerInx = BitConverter.ToUInt16(bufferSpan[NetworkMessageCommonInfo.ClientMsgArgumentPosition.OwnerIdx]);
        var id = BitConverter.ToUInt16(bufferSpan[NetworkMessageCommonInfo.ClientMsgArgumentPosition.Message]);
        var payloadLength = NetworkMessageHelper.GetPayloadLength(id);
        var opcode = BitConverter.ToUInt16(bufferSpan[NetworkMessageCommonInfo.ClientMsgArgumentPosition.Opcode]);
        var newIndex = payloadLength + NetworkMessageCommonInfo.ClientMsgArgumentPosition.HeaderSize + 1; //Add header segment
        return new DecodeResult
        {
            MessageId = id,
            Opcode = opcode,
            OwnerIndex = ownerInx,
            EndIndex = newIndex
        };
    }
}