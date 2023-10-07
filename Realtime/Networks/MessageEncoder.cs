using MemoryPack;
using Realtime.Utils.Buffers;
using Realtime.Utils.Factory;
namespace Realtime.Networks;

public class MessageEncoder
{
    private readonly IFactory<BufferPointer<byte>, 
        PoolableWrapper<BufferPointer<byte>, UnmanagedMemoryManager<byte>>> _memoryManagerPool; 
    public AutoDisposableData<ReadOnlyMemory<byte>, UnmanagedMemoryManager<byte>> EncodeNonAlloc<TData>(
        in uint opcode, in TData payload)
        where TData : unmanaged, INetworkPayload
    {
        // The message always from the server, so no need to attach owner, target and message type
        // We just send: [messageId opcode payload]
        // We call the section [messageId opcode] (4 bytes) is header
        Span<byte> header = stackalloc byte[NetworkMessageCommonInfo.ServerMsgArgumentPosition.HeaderSize];
        var msgId = NetworkMessageHelper.GetPayloadId<TData>();
        BitConverter.TryWriteBytes(header, msgId); // Write messageId
        BitConverter.TryWriteBytes(header[NetworkMessageCommonInfo.HeaderArgumentSize..], opcode); // Write opcode
        var buffer = new EncodeBufferWriter(header.Length); //Make room for the header in sent data before serilizing it 
        using (buffer)
        {
            var options = MemoryPackWriterOptionalStatePool.Rent(null);
            var writer = new MemoryPackWriter<EncodeBufferWriter>(ref buffer, options);
            MemoryPackSerializer.Serialize(ref writer, payload);
            var resultBuffer = buffer.PrependAndGet(header); //Write the header in front of the payload segment
            var memoryManagerWrapper = _memoryManagerPool.Create(resultBuffer);
            ref readonly var memoryManager = ref memoryManagerWrapper.WrappedValue;
            return new AutoDisposableData<ReadOnlyMemory<byte>, UnmanagedMemoryManager<byte>>(memoryManager.Memory, memoryManager);
        }
    }
}