using System.Net;
using Realtime.Controllers.Match.Interfaces;
using Realtime.Data;

namespace Realtime.Controllers.Setup.Interfaces;

//Only implement 1 time
public interface IMatchSetupRunner<TMatchContext, TMatchData, TPlayerIndex, TAuthData, TPlayer>
    where TPlayer : PlayerData<TPlayerIndex, TAuthData>
    where TMatchData : MatchData<TPlayerIndex, TAuthData, TPlayer>
    where TPlayerIndex : unmanaged
    where TMatchContext : IMatchContext<TMatchData, TPlayerIndex, TAuthData, TPlayer>
{
    ValueTask<TMatchContext> Setup();
}

public enum SocketType
{
    RawTcp, WebSocket, WebSocketSecure
}

public enum TickLateStrategy
{
    Accept, ForceNext
}

public enum HandleMessageStrategy
{
    Sync, Async
}

public class RealtimeConfiguration
{
    public int Port { get; set; } = 1905; //My birthdate bro
    public SocketType SocketType { get; set; } = SocketType.RawTcp;
    public float AutoDisconnectPlayerDuration { get; set; } = 0;
    public float AutoDownDuration { get; set; } = 0;
    public int MaxPlayers { get; set; } = 0;
    public int TickRate { get; set; } = 30;
    public TickLateStrategy TickLateStrategy { get; set; } = TickLateStrategy.Accept;
    public HandleMessageStrategy HandleMessageStrategy { get; set; } = HandleMessageStrategy.Sync;
} 
