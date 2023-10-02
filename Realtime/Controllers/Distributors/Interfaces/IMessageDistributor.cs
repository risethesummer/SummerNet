using Realtime.Networks;

namespace Realtime.Controllers.Distributors.Interfaces;

public interface IMessageDistributor<TPlayerIndex>
{
    void BroadcastMessage<T>(NetworkMessage<T> msg) where T : unmanaged;
    void SendMessage<T>(NetworkMessage<TPlayerIndex, T> msg) where T : unmanaged;
}
