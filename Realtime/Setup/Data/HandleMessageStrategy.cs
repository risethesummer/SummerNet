using Realtime.Attributes;

namespace Realtime.Setup.Data;

/// <summary>
/// How do we handle all messages of a tick 
/// </summary>
public enum HandleMessageStrategy
{
    /// <summary>
    /// The messages will be handled in order (synchronously) <br/>
    /// That means a message is only processed after the previous one is done 
    /// </summary>
    Sync, 
    /// <summary>
    /// The messages will be handled in parallel (asynchronously) therefore giving a better performance <br/>
    /// This strategy may create many threads, so you should apply it carefully 
    /// </summary>
    Async,
    /// <summary>
    /// We try to mix <see cref="Sync"/> and <see cref="Async"/> strategies <br/>
    /// <see cref="RpcAttribute"/> methods are define with <see cref="Task"/> or <see cref="ValueTask"/> return type will be processed async <br/>
    /// <see cref="RpcAttribute"/> methods are define with <see cref="Void"/> return type will be processed sync <br/>
    /// I highly recommend apply this approach if you don't know what to do
    /// </summary>
    Mix
}