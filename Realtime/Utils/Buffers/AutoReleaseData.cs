namespace Realtime.Utils.Buffers;

public readonly struct SemaphoreReleaser : IDisposable
{
    private readonly SemaphoreSlim _semaphore;
    public SemaphoreReleaser(SemaphoreSlim semaphore)
    {
        _semaphore = semaphore;
    }
    public void Dispose()
    {
        _semaphore.Release();
    }
}