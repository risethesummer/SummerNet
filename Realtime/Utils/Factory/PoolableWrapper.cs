using System.Buffers;

namespace Realtime.Utils.Factory;

public class DisposableQueue<T> : Queue<T>, IDisposable
{
    public void Dispose()
    {
    }
}

public sealed class DisposablePoolWrapper<T> : PoolableWrapper<T> where T : IDisposable 
{
    public DisposablePoolWrapper(IPool<PoolableWrapper<T>> pool, T wrappedValue) : base(pool, wrappedValue)
    {
    }
    public override void Dispose()
    {
        Value.Dispose();
        base.Dispose();
    }
}
public class PoolableWrapper<T> : IDisposable
{
    private readonly IPool<PoolableWrapper<T>> _pool;
    public T Value { get; }
    public PoolableWrapper(IPool<PoolableWrapper<T>> pool, T wrappedValue)
    {
        _pool = pool;
        Value = wrappedValue;
    }
    public static implicit operator T(PoolableWrapper<T> entry) => entry.Value;
    public virtual void Dispose()
    {
        _pool.Dispose(this);
    }
}