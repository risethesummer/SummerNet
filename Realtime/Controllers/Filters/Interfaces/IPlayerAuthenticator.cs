using Realtime.Controllers.Transporters.Messages;
using Realtime.Data;
using Realtime.Networks;

namespace Realtime.Controllers.Filters.Interfaces;

public interface IPlayerAuthenticator<TPlayerIndex, TAuthData, TPlayer> 
    where TPlayer : PlayerData<TPlayerIndex, TAuthData> where TPlayerIndex : unmanaged, INetworkIndex
{
    ValueTask<TPlayer?> Authenticate(TPlayer data);
}