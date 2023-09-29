namespace SummerNet.Realtime.Controllers.Filters.Interfaces;

public interface IMessageFilter<TData>
{
    Task<TData> Filter(TData data);
}