using System.Net;
using Realtime.Networks;

namespace Realtime.Data;

public class PlayerData<TId> where TId : unmanaged, INetworkIndex
{
    public TId PlayerId { get; init; }
    public string Name { get; init; }
    public DateTime JoinedTime { get; init; }
    public IPAddress Address { get; init; }
    public PlayerStatus Status { get; set; }
}