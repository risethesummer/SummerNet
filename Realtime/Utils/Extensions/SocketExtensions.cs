using System.Net.Sockets;
using System.Runtime.CompilerServices;
using Realtime.Utils.Buffers;

namespace Realtime.Utils.Extensions;

public static class SocketExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task FlushReceivedData(this Socket socket, 
        Memory<byte> receivedBuffer,
        int ignoreHeader,
        IParallelBuffer<byte> resultParallelBuffer,
        CancellationToken cancellationToken)
    {
        var dataCount = await socket.ReceiveAsync(receivedBuffer[ignoreHeader..], cancellationToken)
            .ConfigureAwait(false);
        if (dataCount > 0)
        {
            await resultParallelBuffer
                .AddToBuffer(receivedBuffer[..(dataCount + ignoreHeader)], cancellationToken)
                .ConfigureAwait(false);
        }
    }
}