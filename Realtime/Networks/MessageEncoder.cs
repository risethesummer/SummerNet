using MemoryPack;
using Realtime.Utils.Buffers;
using Realtime.Utils.Factory;
namespace Realtime.Networks;

public class MessageEncoder
{
    private readonly IFactory<DangerousBuffer<byte>, 
        PoolableWrapper<DangerousBuffer<byte>, UnmanagedMemoryManager<byte>>> _memoryManagerPool; 
    public AutoDisposableData<ReadOnlyMemory<byte>, UnmanagedMemoryManager<byte>> EncodeNonAlloc<TPlayerIndex, TData>(in NetworkMessage<TPlayerIndex, TData> msg)
        where TData : unmanaged, INetworkPayload where TPlayerIndex : unmanaged, INetworkIndex
    {
        Span<byte> idSpan = stackalloc byte[sizeof(ushort)];
        var msgId = NetworkMessageHelper.GetPayloadId<TData>();
        BitConverter.TryWriteBytes(idSpan, msgId);
        var buffer = new EncodeBufferWriter(idSpan.Length);
        using (buffer)
        {
            var options = MemoryPackWriterOptionalStatePool.Rent(null);
            var writer = new MemoryPackWriter<EncodeBufferWriter>(ref buffer, options);
            MemoryPackSerializer.Serialize(ref writer, msg);
            var resultBuffer = buffer.PrependAndGet(idSpan); 
            var memoryManagerWrapper = _memoryManagerPool.Create(resultBuffer);
            ref var memoryManager = ref memoryManagerWrapper.AsValue();
            return new AutoDisposableData<ReadOnlyMemory<byte>, UnmanagedMemoryManager<byte>>(memoryManager.Memory, memoryManager);
        }
    }
}