namespace SummerNet.Realtime.Controllers.Distributors.Interfaces;

public interface IMessageDistributor<in TPlayerIndex>
{
    void BroadcastMessage<T>(uint opcode, T message);
    void SendMessage<T>(uint opcode, TPlayerIndex target, T message);
}