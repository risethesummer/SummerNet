namespace Realtime.Utils.Buffers;

public readonly struct AutoDisposableData<T, TDisposable> : IDisposable where TDisposable : IDisposable
{
    public readonly T Data;
    public readonly TDisposable? Disposable;
    
    public AutoDisposableData(T data, TDisposable disposable)
    {
        Data = data;
        Disposable = disposable;
    }
    public void Dispose()
    {
        Disposable?.Dispose();
    }
}