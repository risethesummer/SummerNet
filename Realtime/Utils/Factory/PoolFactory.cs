using System.Collections.Concurrent;
using Autofac;
using IContainer = System.ComponentModel.IContainer;

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

public class PoolFactory<TParam, T> : ObjectPool<T>, IFactory<TParam, T> where T : IPoolableObject<TParam>
{
    private readonly IFactory<TParam, T> _factory;
    public PoolFactory(IFactory<TParam, T> factory)
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

public class Factory<TParam, T> : IFactory<TParam, T>
{
    private readonly ILifetimeScope _container;
    public Factory(ILifetimeScope container)
    {
        _container = container;
    }
    public T Create(in TParam param)
    {
        return _container.Resolve<T>(TypedParameter.From(param));
    }
}

public class Factory<T> : IFactory<T> where T : IPoolableObject
{
    private readonly ILifetimeScope _container;
    public Factory(ILifetimeScope container)
    {
        _container = container;
    }
    public T Create()
    {
        return _container.Resolve<T>();
    }
}

public class PoolFactory<T> : ObjectPool<T>, IFactory<T> 
    where T : IPoolableObject
{
    private readonly IFactory<T> _factory;
    public PoolFactory(IFactory<T> factory)
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