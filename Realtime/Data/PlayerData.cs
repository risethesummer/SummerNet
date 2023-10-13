using System.Net;
using Realtime.Filters.Interfaces;

namespace Realtime.Data;

/// <summary>
/// Storing your player data, inherit this class <b>once</b> for introducing more necessary fields <br/>
/// Please remember this is singleton for each player, modify this class instance will affect the player data permanently
/// </summary>
/// <typeparam name="TId">The type of player ID, for better performance please use <see cref="int"/></typeparam>
/// <typeparam name="TAuthData">The type of your authentication data, it's regularly a string token,
/// will be used in authentication process <see cref="IPlayerAuthenticator{TPlayerIndex,TAuthData,TPlayer}"/>. If you don't need it, feel free to use <see cref="bool"/>, <see cref="int"/>... whatever you like</typeparam>
public class PlayerData<TId, TAuthData>
{
    public TId PlayerId { get; set; }
    public TAuthData? AuthData { get; init; }
    public DateTime JoinedTime { get; init; } = DateTime.Now;
    public PlayerStatus Status { get; set; } = PlayerStatus.Setup;
    public IPAddress Address { get; init; }
}
