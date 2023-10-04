using Realtime.Data;

namespace Realtime.Controllers.Transporters.Interfaces;

public interface ITransporter<TPlayerIndex, TPlayer> : 
    IMessageReceiver<TPlayerIndex>, IMessageDistributor<TPlayerIndex>
    where TPlayer : PlayerData<TPlayerIndex>
{
    ValueTask RemovePlayerAsync(TPlayerIndex target);
    ValueTask<TPlayer> AddPlayerAsync(TPlayer playerData);
    ValueTask InitMatchPlayersAsync(CancellationToken cancellationToken);
    void Clear();
}