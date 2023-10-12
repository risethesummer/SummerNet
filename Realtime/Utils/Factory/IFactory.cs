namespace Realtime.Utils.Factory;

public interface IFactory<out T>
{
    T Create();
}

public interface IFactory<TParam, out T>
{
    T Create(in TParam param);
}