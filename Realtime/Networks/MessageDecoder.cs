using MemoryPack;

namespace Realtime.Networks;

public partial class MessageDecoder
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
        // The message from clients will be [id opcode payloadLength payload]
        if (buffer.Length <= NetworkMessageCommonInfo.ClientMsgArgumentPosition.HeaderSize)
            return null;
        var bufferSpan = buffer.Span;
        var ownerInx = BitConverter.ToUInt16(bufferSpan[NetworkMessageCommonInfo.ClientMsgArgumentPosition.OwnerIdx]);
        var id = BitConverter.ToUInt16(bufferSpan[NetworkMessageCommonInfo.ClientMsgArgumentPosition.MessageId]);
        var opcode = BitConverter.ToUInt16(bufferSpan[NetworkMessageCommonInfo.ClientMsgArgumentPosition.Opcode]);
        var payloadLength = BitConverter.ToUInt16(bufferSpan[NetworkMessageCommonInfo.ClientMsgArgumentPosition.PayloadLength]);
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