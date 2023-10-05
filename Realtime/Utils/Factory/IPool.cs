namespace Realtime.Utils.Factory;

public interface IPool<T> : IFactory<T>
{
    void Dispose(T obj);
}