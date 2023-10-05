using Realtime.Data;

namespace Realtime.Controllers.Filters.Interfaces;

public interface IPlayerAuthenticator<TPlayerIndex, TPlayerData> 
    where TPlayerData : PlayerData<TPlayerIndex>
{
    ValueTask<TPlayerData> Authenticate(TPlayerData data);
}