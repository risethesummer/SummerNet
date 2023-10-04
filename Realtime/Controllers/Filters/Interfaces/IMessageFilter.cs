using Realtime.Networks;

namespace Realtime.Controllers.Filters.Interfaces;

public interface IMessageFilter<TPlayerIndex, TData> where TData : unmanaged, INetworkPayload
{
    Task<TData> Filter(NetworkMessage<TPlayerIndex, TData> data);
}