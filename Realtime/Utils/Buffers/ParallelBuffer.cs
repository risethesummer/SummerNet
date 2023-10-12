using Realtime.Utils.Factory;

namespace Realtime.Utils.Buffers;

public class ParallelBuffer<TWrappedData> : IDisposable 
    where TWrappedData : unmanaged
{
    private AutoSizeBuffer<TWrappedData> _buffer;
    private readonly SemaphoreSlim _semaphoreSlim = new(0, 1);
    private readonly UnmanagedMemoryManager<TWrappedData> _memoryManager;
    public ParallelBuffer(UnmanagedMemoryManager<TWrappedData> memoryManager)
    {
        _memoryManager = memoryManager;
    }
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

    public async ValueTask<AutoDisposableData<ReadOnlyMemory<TWrappedData>, SemaphoreReleaser>> GetBuffer(
        CancellationToken token)
    {
        await _semaphoreSlim.WaitAsync(token).ConfigureAwait(false);
        _memoryManager.Initialize(_buffer.BufferPointer);
        var memoryBuffer = _memoryManager.ForgetMemory;
        return new AutoDisposableData<ReadOnlyMemory<TWrappedData>, SemaphoreReleaser>(memoryBuffer, 
            new SemaphoreReleaser(_semaphoreSlim));
    }

    public void Dispose()
    {
        _semaphoreSlim.Dispose();
        _buffer.Dispose();
        GC.SuppressFinalize(this);
    }
}