namespace Realtime.Setup.Data;

/// <summary>
/// Network configuration for the program converted from the configuration file ("Realtime" section) <br/>
/// Please note that you can only override <see cref="Port"/>, <see cref="Endpoint"/>
/// <see cref="AutoDisconnectPlayerDuration"/>, <see cref="AutoDownDuration"/>,
/// <see cref="MaxPlayers"/>, <see cref="TickRate"/> <br/>
/// Because of source generating, if we try to change other information, it'll generate lots of source code, so
/// you should define the stable configures before running the program
/// </summary>
public class RealtimeConfiguration
{
    public const string ConfigureSection = "Realtime";
    /// <summary>
    /// Port for accepting socket
    /// </summary>
    public int Port { get; set; } = 1905;

    /// <summary>
    /// Raw sockets (host ip), eg. "127.0.0.1"
    /// Websocket connections (endpoint), eg. "/ws" <br/>
    /// </summary>
    public string Endpoint { get; set; } = "127.0.0.1";
    
    /// <summary>
    /// refer to <see cref="SocketType"/>
    /// </summary>
    public SocketType Socket { get; set; } = SocketType.RawTcp;
    /// <summary>
    /// refer to <see cref="TickLateStrategy"/>
    /// </summary>
    public TickLateStrategy TickStrategy { get; set; } = TickLateStrategy.Accept;
    /// <summary>
    /// refer to <see cref="HandleMessageStrategy"/>
    /// </summary>
    public HandleMessageStrategy MessageStrategy { get; set; } = HandleMessageStrategy.Mix;
    /// <summary>
    /// How long (in seconds) a player should be disconnected automatically since the last active time (not sending any message) <br/>
    /// Set it to 0 for disabling this feature
    /// </summary>
    public float AutoDisconnectPlayerDuration { get; set; } = 0;
    /// <summary>
    /// How long (in seconds) a match should be ended automatically since the last active time (not receiving any message) <br/>
    /// Set it to 0 for disabling this feature
    /// </summary>
    public float AutoDownDuration { get; set; } = 0;
    /// <summary>
    /// The match will reject any late comer if the number of players exceed this value <br/>
    /// Set it to 0 for disabling this feature
    /// </summary>
    public int MaxPlayers { get; set; } = 0;
    /// <summary>
    /// Should keep a player information if he is disconnected from the match <br/>
    /// I highly recommend you'd better use another approach (like Redis, Memcached,...)
    /// </summary>
    public bool SupportReconnect { get; set; } = false;
    /// <summary>
    /// How many tick should happen in 1 sec <br/>
    /// </summary>
    public int TickRate { get; set; } = 30;
} 
