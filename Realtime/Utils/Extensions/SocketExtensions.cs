using MemoryPack;
using Realtime.Controllers.Transporters.Interfaces;
using Realtime.Utils.Buffers;
using Realtime.Utils.Factory;

namespace Realtime.Utils.Extensions;

public static class SocketExtensions
{
    // [MethodImpl(MethodImplOptions.AggressiveInlining)]
    // public static async Task FlushReceivedData<TPlayerIndex>(this ISocket socket, 
    //     Memory<byte> receivedBuffer,
    //     Queue<ReceivedNetworkMessage<TPlayerIndex>> resultParallelBuffer,
    //     CancellationToken cancellationToken) 
    //     where TPlayerIndex : unmanaged, INetworkIndex
    // {
    //     var dataCount = await socket.ReceiveAsync(receivedBuffer, cancellationToken)
    //         .ConfigureAwait(false);
    //     if (dataCount > 0)
    //     {
    //         await resultParallelBuffer
    //             .AddToBuffer(receivedBuffer[..(dataCount + ignoreHeader)], cancellationToken)
    //             .ConfigureAwait(false);
    //     }
    // }
    
    public static async ValueTask<T?> GetRawMessageAsync<T>(this ISocket socket, 
        IFactory<BufferPointer<byte>, PoolableWrapper<BufferPointer<byte>, UnmanagedMemoryManager<byte>>> memoryManagerPool,
        CancellationToken cancellationToken) 
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var buffer = new AutoSizeBuffer<byte>(1024);
            // Memory manager will free the pointer, so we don't need using keyword
            using var memoryManagerWrapper = memoryManagerPool.Create(buffer.BufferPointer);
            var memory = memoryManagerWrapper.WrappedValue.Memory;
            var handshakeData = await socket.ReceiveAsync(memory, cancellationToken);
            var data = MemoryPackSerializer.Deserialize<T>(memory[..handshakeData].Span);
            return data;
        }
        return default;
    }
}