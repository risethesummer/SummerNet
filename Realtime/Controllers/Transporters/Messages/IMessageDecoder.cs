namespace Realtime.Controllers.Transporters.Messages;

public interface IMessageDecoder
{
    DecodeResult? Decode(in ReadOnlyMemory<byte> buffer);
    public readonly struct DecodeResult
    {
        public ushort Opcode { get; init; }
        public int Length { get; init; }
    }
}