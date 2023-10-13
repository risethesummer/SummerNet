namespace Realtime.Setup.Data;

/// <summary>
/// Which type of socket used for sending and receiving messages
/// </summary>
public enum SocketType
{
    /// <summary>
    /// Raw TCP socket, currently we do not accept UDP socket because of its complexity
    /// </summary>
    RawTcp,
    /// <summary>
    /// WebSocket communicating on HTTP connection, you must use this if your client is WebGL 
    /// </summary>
    WebSocket,
    /// <summary>
    /// Similar to WebSocket but SSL setup, you also need to configure the SSL step manually  
    /// </summary>
    WebSocketSecure
}