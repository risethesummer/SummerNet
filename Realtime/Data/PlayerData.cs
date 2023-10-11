using System.Net;

namespace Realtime.Data;

public class PlayerData<TId, TAuthData> where TId : unmanaged 
{
    public TId PlayerId { get; set; }
    public TAuthData? AuthData { get; init; }
    public required DateTime JoinedTime { get; init; }
    public required IPAddress Address { get; init; }
    public required PlayerStatus Status { get; set; }
}
