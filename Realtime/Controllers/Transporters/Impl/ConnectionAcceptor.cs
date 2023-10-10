using System.Runtime.CompilerServices;
using Realtime.Controllers.Transporters.Interfaces;

namespace Realtime.Controllers.Transporters.Impl;

public class ConnectionAcceptor : IConnectionAcceptor
{
    private readonly ISocket _acceptSocket;
    public ConnectionAcceptor(ISocket acceptSocket)
    {
        _acceptSocket = acceptSocket;
    }
    public void Dispose()
    {
        _acceptSocket.Dispose();
        GC.SuppressFinalize(this);
    }
    public async IAsyncEnumerable<ISocket> BeginAccepting([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var accept = await _acceptSocket.AcceptAsync(cancellationToken).ConfigureAwait(false);
            yield return accept;
        }
    }
}