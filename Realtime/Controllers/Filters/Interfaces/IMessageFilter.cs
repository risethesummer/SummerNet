using Realtime.Controllers.Transporters.Messages;

namespace Realtime.Controllers.Filters.Interfaces;

public interface IMessageFilter<TPlayerIndex, TData>
    where TPlayerIndex : unmanaged
{
    Task<TData> Filter(SentNetworkMessage<TPlayerIndex, TData> data);
}