namespace Realtime.Utils.Buffers;

public class ParallelBufferWrapper<TWrappedData> : IDisposable, IParallelBuffer<TWrappedData> 
    where TWrappedData : unmanaged
{
    private readonly AutoSizeBuffer<TWrappedData> _buffer = new(10000);
    private readonly SemaphoreSlim _semaphoreSlim = new(0, 1);
    public async ValueTask AddToBuffer(TWrappedData data, CancellationToken token)
    {
        try
        {
            await _semaphoreSlim.WaitAsync(token).ConfigureAwait(false);
            _buffer.Write(data);
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }
    
    public async ValueTask AddToBuffer(ArraySegment<TWrappedData> data, CancellationToken token)
    {
        try
        {
            await _semaphoreSlim.WaitAsync(token).ConfigureAwait(false);
            _buffer.Write(data);
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }

    public void Clear()
    {
        _buffer.Clear();
    }

    public async ValueTask<AutoReleaseData<ArraySegment<TWrappedData>>> GetBuffer(CancellationToken token)
    {
        await _semaphoreSlim.WaitAsync(token).ConfigureAwait(false);
        return new AutoReleaseData<ArraySegment<TWrappedData>>(_semaphoreSlim, 
            _buffer.AllocArray());
    }

    public void Dispose()
    {
        _semaphoreSlim.Dispose();
    }
}