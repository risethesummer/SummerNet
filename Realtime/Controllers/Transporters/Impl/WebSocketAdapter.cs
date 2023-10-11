using System.Net;
using System.Net.WebSockets;
using Realtime.Controllers.Transporters.Interfaces;

namespace Realtime.Controllers.Transporters.Impl;

public class WebSocketAdapter : ISocket
{
    private readonly WebSocket _socket;
    private readonly IPEndPoint _remoteEndPoint;
    public WebSocketAdapter(WebSocket socket, IPEndPoint remoteEndPoint)
    {
        _socket = socket;
        _remoteEndPoint = remoteEndPoint;
    }

    public async ValueTask<int> ReceiveAsync(Memory<byte> buffer, CancellationToken cancellationToken)
    {
        var res = await _socket.ReceiveAsync(buffer, cancellationToken);
        return res.Count;
    }

    public async ValueTask<int> SendAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken)
    {
        try
        {
            await _socket.SendAsync(buffer, WebSocketMessageType.Binary, true, cancellationToken);
            return buffer.Length;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return 0;
        }
    }
    public ValueTask DisconnectAsync(bool reuse, CancellationToken cancellationToken) =>
        new(_socket.CloseAsync(WebSocketCloseStatus.Empty, string.Empty, cancellationToken));

    public EndPoint EndPoint => _remoteEndPoint;

    public void Dispose()
    {
        _socket.Dispose();
        GC.SuppressFinalize(this);
    }
}