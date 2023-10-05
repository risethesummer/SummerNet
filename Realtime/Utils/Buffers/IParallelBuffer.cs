namespace Realtime.Utils.Buffers;

/// <summary>
/// Represents for thread safe buffers
/// </summary>
/// <typeparam name="TWrappedData"></typeparam>
public interface IParallelBuffer<TWrappedData>
{
    ValueTask AddToBuffer(ArraySegment<TWrappedData> data, CancellationToken token);
    ValueTask AddToBuffer(TWrappedData data, CancellationToken token);
    void Clear();
    ValueTask<AutoReleaseData<ArraySegment<TWrappedData>>> GetBuffer(CancellationToken token);
}