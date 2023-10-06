using Realtime.Utils.Factory;

namespace Realtime.Utils.Buffers;

public class ParallelBufferWrapper<TWrappedData> : IDisposable, IParallelBuffer<TWrappedData> 
    where TWrappedData : unmanaged
{
    private AutoSizeBuffer<TWrappedData> _buffer;
    private readonly SemaphoreSlim _semaphoreSlim = new(0, 1);
    private readonly UnmanagedMemoryManager<TWrappedData> _memoryManager;
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

    public void Resize(uint size)
    {
        _buffer.Resize(size);
    }

    public async ValueTask AddToBuffer(Memory<TWrappedData> data, CancellationToken token)
    {
        try
        {
            await _semaphoreSlim.WaitAsync(token).ConfigureAwait(false);
            _buffer.Write(data, data.Length);
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

    public async ValueTask<AutoDisposableData<Memory<TWrappedData>, SemaphoreReleaser>> GetBuffer(
         
        CancellationToken token)
    {
        await _semaphoreSlim.WaitAsync(token).ConfigureAwait(false);
        _memoryManager.Initialize(_buffer.DangerousBuffer);
        var memoryBuffer = _memoryManager.Memory;
        return new AutoDisposableData<Memory<TWrappedData>, SemaphoreReleaser>(memoryBuffer, 
            new SemaphoreReleaser());
    }

    public void Dispose()
    {
        _semaphoreSlim.Dispose();
        _buffer.Dispose();
        GC.SuppressFinalize(this);
    }
}