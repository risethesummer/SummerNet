namespace Realtime.Utils.Factory;

public interface IFactory<out T> where T : IPoolableObject
{
    T Create();
}

public interface IFactory<TParam, out T> where T : IPoolableObject<TParam>
{
    T Create(in TParam param);
}