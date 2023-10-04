using System.Net.Sockets;
using Realtime.Controllers.Transporters.Impl;

namespace Realtime.Utils;

public static class SocketExtensions
{
    public static async Task FlushReceivedData(this Socket socket, Memory<byte> receivedBuffer, 
        IBufferStorage<byte> resultBuffer,
        CancellationToken cancellationToken)
    {
        var dataCount = await socket.ReceiveAsync(receivedBuffer, cancellationToken);
        if (dataCount > 0)
        {
            await resultBuffer.AddToBuffer(receivedBuffer[..dataCount], cancellationToken);
        }
    }
}