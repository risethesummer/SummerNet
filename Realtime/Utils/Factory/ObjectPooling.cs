using System.Collections.Concurrent;

namespace Realtime.Utils.Factory;

public abstract class ObjectPool<T> : IPool
{
    protected readonly ConcurrentQueue<T> Pool = new();
    protected T? Deque()
    {
        if (!Pool.IsEmpty && Pool.TryDequeue(out var obj))
            return obj;
        return default;
    }
    public void Dispose(in object obj)
    {
        Pool.Enqueue((T)obj);
    }
}

public class ObjectPooling<TParam, T> : ObjectPool<T>, IFactory<TParam, T> where T : IPoolableObject<TParam>
{
    private readonly IFactory<TParam, T> _factory;
    public ObjectPooling(IFactory<TParam, T> factory)
    {
        _factory = factory;
    }
    public T Create(in TParam param)
    {
        var obj = Deque() ?? _factory.Create(param);
        obj.Initialize(param);
        return obj;
    }
}

public class ObjectPooling<T> : ObjectPool<T>, IFactory<T> where T : IPoolableObject
{
    private readonly IFactory<T> _factory;
    public ObjectPooling(IFactory<T> factory)
    {
        _factory = factory;
    }
    public T Create()
    {
        var obj = Deque() ?? _factory.Create();
        obj.Initialize();
        return obj;
    }
}