using SummerNet.Realtime.Networks;

namespace SummerNet.Realtime.Controllers.Distributors.Interfaces;

public interface IMessageDistributor<in TPlayerIndex>
{
    void BroadcastMessage<T>(NetworkMessage<T> msg) where T : unmanaged;
    void SendMessage<T>(TPlayerIndex target, NetworkMessage<T> msg) where T : unmanaged;
}