namespace Realtime.Controllers.Handlers.Impl;

public class MatchTickCounter
{
    public int Tick { get; private set; } = 0;
    private readonly TimeSpan _tickWait;
    public MatchTickCounter(int tickRate)
    {
        _tickWait = TimeSpan.FromSeconds(60 / (double)tickRate);
    }
    public async Task EndTick(CancellationToken token)
    {
        Tick++;
        await Task.Delay(_tickWait, token);
    }
}