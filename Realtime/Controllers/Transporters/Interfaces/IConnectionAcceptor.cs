using Realtime.Data;
using Realtime.Networks;

namespace Realtime.Controllers.Transporters.Interfaces;

public interface IConnectionAcceptor : IDisposable
{
    IAsyncEnumerable<ISocket> BeginAccepting(CancellationToken cancellationToken);
}

public interface IPlayerAcceptor<TPlayerIndex, TAuthData, out TPlayer> : IDisposable 
    where TPlayer : PlayerData<TPlayerIndex, TAuthData>, new() where TPlayerIndex : unmanaged, INetworkIndex
{
    IAsyncEnumerable<TPlayer> BeginAccepting(CancellationToken cancellationToken);
}