namespace Realtime.Controllers.Transporters.Interfaces;

public interface IConnectionAcceptor : IDisposable
{
    IAsyncEnumerable<ISocket> BeginAccepting(CancellationToken cancellationToken);
}