using Realtime.Data;

namespace Realtime.Transporters.Interfaces;

public interface ITransporter<TPlayerIndex, TAuthData, TPlayer> : 
    IMessageReceiver<TPlayerIndex>, IMessageDistributor<TPlayerIndex>
    where TPlayer : PlayerData<TPlayerIndex, TAuthData> 
{
    ValueTask RemovePlayerAsync(TPlayerIndex target, CancellationToken cancellationToken);
    ValueTask<TPlayer?> AddPlayerAsync(TPlayer playerData, ISocket socket);
    void Clear();
}