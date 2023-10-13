using Realtime.Data;

namespace Realtime.Transporters.Interfaces;

public interface IPlayerAcceptor<TPlayerIndex, TAuthData, out TPlayer> 
    where TPlayer : PlayerData<TPlayerIndex, TAuthData>, new()
{
    IAsyncEnumerable<TPlayer> BeginAccepting(CancellationToken cancellationToken);
}