namespace Realtime.Setup.Data;

/// <summary>
/// When a tick takes too long for its jobs. You need to define a strategy when handling that situation. <br/>
/// This is a strong influence on your game performance, please select the right approach based on your requirements
/// </summary>
public enum TickLateStrategy
{
    /// <summary>
    /// We will wait for the tick handling, that means our clients will receive the tick response later than usual <br/>
    /// </summary>
    Accept,
    /// <summary>
    /// We try to complete the tick by ignoring incomplete jobs in the tick, please use this strategy carefully because of lost client messages 
    /// </summary>
    Ignore,
    /// <summary>
    /// We try to complete the tick by moving the incomplete jobs to the next tick 
    /// </summary>
    MoveToNextTick
}