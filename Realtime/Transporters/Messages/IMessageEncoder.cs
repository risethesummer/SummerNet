using Realtime.Utils.Buffers;

namespace Realtime.Transporters.Messages;

public interface IMessageEncoder
{
    BufferPointer<byte> EncodeNonAlloc<TData>(in uint opcode, in TData payload);
}