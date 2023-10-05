namespace Realtime.Utils.Factory;

public class SimpleObjectPooling<T> : IPool<T>
{
    private readonly Queue<T> _pool;
    private readonly IFactory<T> _factory;
    public SimpleObjectPooling(int capacity, IFactory<T> factory)
    {
        _pool = new Queue<T>(capacity);
        _factory = factory;
    }
    public T Create()
    {
        if (_pool.Count > 0)
            return _pool.Dequeue();
        var newObj = _factory.Create();
        _pool.Enqueue(newObj);
        return newObj;
    }
    public void Dispose(T obj)
    {
        _pool.Enqueue(obj);
    }
}