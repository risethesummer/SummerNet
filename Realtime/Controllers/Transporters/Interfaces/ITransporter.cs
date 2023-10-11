using Realtime.Controllers.Transporters.Messages;
using Realtime.Data;
using Realtime.Networks;

namespace Realtime.Controllers.Transporters.Interfaces;

public interface ITransporter<TPlayerIndex, TAuthData, TPlayer> : 
    IMessageReceiver<TPlayerIndex>, IMessageDistributor<TPlayerIndex>
    where TPlayer : PlayerData<TPlayerIndex, TAuthData> 
    where TPlayerIndex : unmanaged, INetworkIndex
{
    ValueTask RemovePlayerAsync(TPlayerIndex target, CancellationToken cancellationToken);
    ValueTask<TPlayer> AddPlayerAsync(TPlayer playerData, ISocket socket);
    void Clear();
}