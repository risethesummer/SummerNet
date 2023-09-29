using System.Net;

namespace SummerNet.Realtime.Data;

public class PlayerData<TId>
{
    public TId PlayerId { get; init; }
    public string Name { get; init; }
    public DateTime JoinedTime { get; init; }
    public IPAddress Address { get; init; }
    public PlayerStatus Status { get; init; }
}