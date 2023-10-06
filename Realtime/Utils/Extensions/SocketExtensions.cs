using System.Net.Sockets;
using Realtime.Utils.Buffers;

namespace Realtime.Utils.Extensions;

public static class SocketExtensions
{
    public static async Task FlushReceivedData(this Socket socket, 
        Memory<byte> receivedBuffer, 
        IParallelBuffer<byte> resultParallelBuffer,
        CancellationToken cancellationToken)
    {
        var dataCount = await socket.ReceiveAsync(receivedBuffer, cancellationToken)
            .ConfigureAwait(false);
        if (dataCount > 0)
        {
            await resultParallelBuffer
                .AddToBuffer(receivedBuffer, cancellationToken) //Don't need to slice because we did use socket.Available
                .ConfigureAwait(false);
        }
    }
}