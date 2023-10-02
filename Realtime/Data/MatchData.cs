namespace Realtime.Data;

public class MatchData<TPlayerIndex, TPlayer> where TPlayer : PlayerData<TPlayerIndex>
{
    public string Id { get; init; }
    public DateTime CreatedTime { get; init; }
    public int TickRate { get; init; }
    public int Ticks { get; set; }
    public MatchStatus Status { get; set; }
    public IDictionary<TPlayerIndex, TPlayer> Players { get; init; }
}

