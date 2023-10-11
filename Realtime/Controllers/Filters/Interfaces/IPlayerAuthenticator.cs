using Realtime.Data;

namespace Realtime.Controllers.Filters.Interfaces;

public interface IPlayerAuthenticator<TPlayerIndex, TAuthData, TPlayer> 
    where TPlayer : PlayerData<TPlayerIndex, TAuthData> where TPlayerIndex : unmanaged
{
    ValueTask<TPlayer?> Authenticate(TPlayer data);
}