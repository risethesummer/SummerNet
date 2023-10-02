namespace Realtime.Controllers.Distributors.Interfaces;

public interface ITransporter<TPlayerIndex> : IMessageReceiver<TPlayerIndex>, IMessageDistributor<TPlayerIndex>
{
    
}