using Realtime.Controllers.Transporters.Messages;
using Realtime.Networks;

namespace Realtime.Controllers.Filters.Interfaces;

public interface IMessageFilter<TPlayerIndex, TData> 
    where TData : unmanaged, INetworkPayload 
    where TPlayerIndex : unmanaged, INetworkIndex
{
    Task<TData> Filter(NetworkMessage<TPlayerIndex, TData> data);
}