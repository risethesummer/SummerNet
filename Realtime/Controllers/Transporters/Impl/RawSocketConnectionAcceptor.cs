using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using Realtime.Controllers.Transporters.Interfaces;

namespace Realtime.Controllers.Transporters.Impl;

public class RawSocketConnectionAcceptor : IConnectionAcceptor
{
    private readonly Socket _acceptSocket;
    public RawSocketConnectionAcceptor(Socket acceptSocket)
    {
        _acceptSocket = acceptSocket;
    }
    public void Dispose()
    {
        _acceptSocket.Dispose();
        GC.SuppressFinalize(this);
    }
    public static RawSocketAdapter CreateTcpListener(IPEndPoint endPoint, int maxListen)
    {
        using Socket listener = new(
            endPoint.AddressFamily, 
            SocketType.Stream, 
            ProtocolType.Tcp);
        listener.Bind(endPoint);
        listener.Listen(maxListen);
        return new RawSocketAdapter(listener);
    }
    public async IAsyncEnumerable<ISocket> BeginAccepting([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var accept = await _acceptSocket.AcceptAsync(cancellationToken).ConfigureAwait(false);
            yield return new RawSocketAdapter(accept);
        }
    }
}