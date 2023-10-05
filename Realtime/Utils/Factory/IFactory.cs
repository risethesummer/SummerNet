namespace Realtime.Utils.Factory;

public interface IFactory<out T>
{
    T Create();
}