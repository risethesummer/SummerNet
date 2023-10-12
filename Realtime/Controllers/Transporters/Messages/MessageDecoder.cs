namespace Realtime.Controllers.Transporters.Messages;

public class MessageDecoder
{
    public readonly struct DecodeResult
    {
        public ushort Opcode { get; init; }
        public int Length { get; init; }
    }
    public DecodeResult? Decode(in ReadOnlyMemory<byte> buffer)
    {
        var bufferLength = buffer.Length;
        // The message from clients will be [id opcode payloadLength payload]
        if (bufferLength <= NetworkMessageCommonInfo.ClientMsgArgumentPosition.HeaderSize)
            return null;
        var bufferSpan = buffer.Span;
        var opcode = BitConverter.ToUInt16(bufferSpan[NetworkMessageCommonInfo.ClientMsgArgumentPosition.Opcode]);
        var payloadLength = BitConverter.ToUInt16(bufferSpan[NetworkMessageCommonInfo.ClientMsgArgumentPosition.PayloadLength]);
        var totalLength = payloadLength + NetworkMessageCommonInfo.ClientMsgArgumentPosition.HeaderSize + 1; //Add header segment
        if (bufferLength < totalLength)
            return null;
        return new DecodeResult
        {
            Opcode = opcode,
            Length = totalLength
        };
    }
}