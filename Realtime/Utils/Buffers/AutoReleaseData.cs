namespace Realtime.Utils.Buffers;

public readonly struct AutoReleaseData<T> : IDisposable
{
    private readonly SemaphoreSlim _semaphore;
    public readonly T Data;
    public AutoReleaseData(SemaphoreSlim semaphore, T data)
    {
        _semaphore = semaphore;
        Data = data;
    }
    public void Dispose()
    {
        _semaphore.Release();
    }
}