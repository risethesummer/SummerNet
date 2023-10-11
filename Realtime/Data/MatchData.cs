namespace Realtime.Data;

public class MatchData<TPlayerIndex, TAuthData, TPlayer> 
    where TPlayer : PlayerData<TPlayerIndex, TAuthData> 
    where TPlayerIndex : unmanaged
{
    public string Id { get; init; } = Guid.NewGuid().ToString();
    public DateTime CreatedTime { get; init; } = DateTime.Now;
    public int TickRate { get; init; } = 30;
    public MatchStatus Status { get; set; } = MatchStatus.Starting;
    public int Ticks { get; set; }
    public int MaxPlayers { get; init; }
    public IDictionary<TPlayerIndex, TPlayer> Players { get; init; }
}

