namespace Realtime.Utils.Factory;

public class DisposableQueue<T> : Queue<T>, IPoolableObject
{
    public void Dispose()
    {
        Clear();
        GC.SuppressFinalize(this);
    }

    public void Initialize()
    {
    }
}

public interface IPoolableObject : IDisposable
{
    void Initialize();
}
public interface IPoolableObject<TParam> : IDisposable
{
    void Initialize(in TParam param);
}

public class PoolableWrapper<TParam, T> : BasePoolableWrapper<T>, IPoolableObject<TParam> where T : IPoolableObject<TParam>
{
    public PoolableWrapper(IPool pool, T wrappedValue) : base(pool, wrappedValue)
    {
    }
    public void Initialize(in TParam param)
    {
        Value.Initialize(param);
    }
}


public class PoolableWrapper<T> : BasePoolableWrapper<T>, IPoolableObject where T : IPoolableObject
{
    public PoolableWrapper(IPool pool, T wrappedValue) : base(pool, wrappedValue)
    {
    }
    public void Initialize()
    {
        Value.Initialize();
    }
}

public class BasePoolableWrapper<T> : IDisposable where T : IDisposable
{
    private readonly IPool _pool;
    protected T Value;
    public BasePoolableWrapper(IPool pool, T wrappedValue)
    {
        _pool = pool;
        Value = wrappedValue;
    }
    public ref readonly T WrappedValue => ref Value;
    public void Dispose()
    {
        Value.Dispose();
        _pool.Dispose(this);
        GC.SuppressFinalize(this);
    }
}