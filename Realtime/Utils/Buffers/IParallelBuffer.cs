namespace Realtime.Utils.Buffers;

/// <summary>
/// Represents for thread safe buffers
/// </summary>
/// <typeparam name="TWrappedData"></typeparam>
public interface IParallelBuffer<TWrappedData>
{
    void Resize(uint size);
    ValueTask AddToBuffer(Memory<TWrappedData> data, CancellationToken token);
    ValueTask AddToBuffer(TWrappedData data, CancellationToken token);
    void Clear();
    ValueTask<AutoDisposableData<Memory<TWrappedData>, SemaphoreReleaser>> GetBuffer(CancellationToken token);
}