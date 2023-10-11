using System.Net;
using System.Runtime.CompilerServices;
using Realtime.Controllers.Transporters.Interfaces;

namespace Realtime.Controllers.Transporters.Impl;

public class WebSocketConnectionAcceptor : IConnectionAcceptor
{
    private readonly HttpListener _httpListener;
    private readonly string? _subProtocol;
    private readonly TimeSpan _keepAliveInterval;

    public WebSocketConnectionAcceptor(HttpListener httpListener, string? subProtocol, TimeSpan keepAliveInterval)
    {
        _httpListener = httpListener;
        _subProtocol = subProtocol;
        _keepAliveInterval = keepAliveInterval;
    }
    public void Dispose()
    {
        _httpListener.Close();
        GC.SuppressFinalize(this);
    }
    public async IAsyncEnumerable<ISocket> BeginAccepting([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested && _httpListener.IsListening)
        {
            var sock = await AcceptAsync(cancellationToken);
            if (sock is not null)
                yield return sock;
        }
    }

    private async ValueTask<ISocket?> AcceptAsync(CancellationToken cancellationToken)
    {

        var context = await _httpListener.GetContextAsync();
        if (!context.Request.IsWebSocketRequest) 
            return null;
        var webSocketContext = await context.AcceptWebSocketAsync(_subProtocol, _keepAliveInterval);
        var webSocket = webSocketContext.WebSocket;
        return new WebSocketAdapter(webSocket, context.Request.RemoteEndPoint);
    }
}