using Realtime.Transporters.Messages;

namespace Realtime.Filters.Interfaces;

public interface IMessageFilter<TPlayerIndex, TData>
    where TPlayerIndex : unmanaged
{
    Task<TData> Filter(SentNetworkMessage<TPlayerIndex, TData> data);
}