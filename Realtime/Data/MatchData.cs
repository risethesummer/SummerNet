﻿using Realtime.Networks;

namespace Realtime.Data;

public class MatchData<TPlayerIndex, TPlayer> 
    where TPlayer : PlayerData<TPlayerIndex> 
    where TPlayerIndex : unmanaged, INetworkIndex
{
    public string Id { get; init; } = Guid.NewGuid().ToString();
    public DateTime CreatedTime { get; init; } = DateTime.Now;
    public int TickRate { get; init; } = 30;
    public int Ticks { get; set; }
    public MatchStatus Status { get; set; } = MatchStatus.Waiting;
    public IDictionary<TPlayerIndex, TPlayer> Players { get; init; }
}

