using MemoryPack;
using Realtime.Utils.Buffers;

namespace Realtime.Transporters.Messages;

public class MessageEncoder : IMessageEncoder
{
    private static void SetHeader(ref Span<byte> header, in uint opcode, in uint payloadLength)
    {
        BitConverter.TryWriteBytes(header[NetworkMessageCommonInfo.ServerMsgArgumentPosition.Opcode], opcode); // Write opcode
        BitConverter.TryWriteBytes(header[NetworkMessageCommonInfo.ServerMsgArgumentPosition.PayloadLength], payloadLength); // Write payloadLength
    }
    public BufferPointer<byte> EncodeNonAlloc<TData>(
        in uint opcode, in TData payload)
    {
        // The message always from the server, so no need to attach owner, target and message type
        // We just send: [messageId opcode payloadLength payload]
        // We call the section [messageId opcode payloadLength] (6 bytes) is header
        Span<byte> header = stackalloc byte[NetworkMessageCommonInfo.ServerMsgArgumentPosition.HeaderSize];
        var buffer = new EncodeBufferWriter(header.Length); //Make room for the header in sent data before serilizing it 
        using (buffer)
        {
            var options = MemoryPackWriterOptionalStatePool.Rent(null);
            var writer = new MemoryPackWriter<EncodeBufferWriter>(ref buffer, options);
            MemoryPackSerializer.Serialize(ref writer, payload);
            SetHeader(ref header, opcode, (uint)buffer.Size);
            var resultBuffer = buffer.PrependAndGet(header); //Write the header in front of the payload segment
            // using var memoryManagerWrapper = _memoryManagerPool.Create(resultBuffer);
            // ref readonly var memoryManager = ref memoryManagerWrapper.WrappedValue;
            return resultBuffer;
        }
    }
}