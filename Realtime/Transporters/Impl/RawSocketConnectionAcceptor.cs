using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using Realtime.Transporters.Interfaces;

namespace Realtime.Transporters.Impl;

public class RawSocketConnectionAcceptor : IConnectionAcceptor
{
    private readonly Socket _acceptSocket;
    public RawSocketConnectionAcceptor(IPEndPoint endPoint, 
        int maxListen)
    {
        _acceptSocket = new Socket(
            endPoint.AddressFamily, 
            SocketType.Stream,
            ProtocolType.Tcp);
        _acceptSocket.Bind(endPoint);
        _acceptSocket.Listen(maxListen);
    }
    public void Dispose()
    {
        _acceptSocket.Dispose();
        GC.SuppressFinalize(this);
    }
    public void Shutdown()
    {
        _acceptSocket.Shutdown(SocketShutdown.Both);
    }
    public async IAsyncEnumerable<ISocket> BeginAccepting([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var accept = await _acceptSocket.AcceptAsync(cancellationToken).ConfigureAwait(false);
            yield return new RawSocketAdapter(accept);
        }
        Shutdown();
    }
}