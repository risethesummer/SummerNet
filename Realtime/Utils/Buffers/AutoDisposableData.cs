namespace Realtime.Utils.Buffers;

public readonly struct AutoDisposableData<T, TDisposable> : IDisposable where TDisposable : IDisposable
{
    public readonly T Data;
    private readonly TDisposable? _disposable;
    
    public AutoDisposableData(T data, TDisposable disposable)
    {
        Data = data;
        _disposable = disposable;
    }
    public void Dispose()
    {
        _disposable?.Dispose();
    }
}