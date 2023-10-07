using Realtime.Data;
using Realtime.Networks;

namespace Realtime.Controllers.Filters.Interfaces;

public interface IPlayerAuthenticator<TPlayerIndex, TPlayerData> 
    where TPlayerData : PlayerData<TPlayerIndex> where TPlayerIndex : unmanaged, INetworkIndex
{
    ValueTask<TPlayerData> Authenticate(TPlayerData data);
}