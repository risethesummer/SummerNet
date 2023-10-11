using Realtime.Data;

namespace Realtime.Controllers.Transporters.Interfaces;

public interface IPlayerAcceptor<TPlayerIndex, TAuthData, out TPlayer> : IDisposable 
    where TPlayer : PlayerData<TPlayerIndex, TAuthData>, new() where TPlayerIndex : unmanaged
{
    IAsyncEnumerable<TPlayer> BeginAccepting(CancellationToken cancellationToken);
}