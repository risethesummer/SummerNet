namespace Realtime.Data;

/// <summary>
/// Storing your match data, inherit this class <b>once</b> for introducing more necessary fields <br/>
/// Please remember this is singleton for each match, modify this class instance will the match data permanently
/// </summary>
/// <typeparam name="TPlayerIndex">The type of player index used in <see cref="PlayerData{TId,TAuthData}"/></typeparam>
/// <typeparam name="TAuthData">The type of player authentication data used in <see cref="PlayerData{TId,TAuthData}"/></typeparam>
/// <typeparam name="TPlayer">The type of custom <see cref="PlayerData{TId,TAuthData}"/> you has defined</typeparam>
public class MatchData<TPlayerIndex, TAuthData, TPlayer> 
    where TPlayer : PlayerData<TPlayerIndex, TAuthData> 
    where TPlayerIndex : unmanaged
{
    public string Id { get; init; } = Guid.NewGuid().ToString();
    public required DateTime CreatedTime { get; init; } = DateTime.Now;
    public required int TickRate { get; init; } = 30;
    public int Ticks { get; set; } = 0;
    public int MaxPlayers { get; init; }
    public MatchStatus Status { get; set; } = MatchStatus.Starting;
    public bool IsRunning => Status == MatchStatus.Running;
    public TPlayer? this[in TPlayerIndex index] => Players.TryGetValue(index, out var player) ? player : null;
    public IDictionary<TPlayerIndex, TPlayer> Players { get; init; }
}