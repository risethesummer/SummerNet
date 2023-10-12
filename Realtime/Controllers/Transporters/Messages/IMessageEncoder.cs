using Realtime.Utils.Buffers;

namespace Realtime.Controllers.Transporters.Messages;

public interface IMessageEncoder
{
    BufferPointer<byte> EncodeNonAlloc<TData>(in uint opcode, in TData payload);
}