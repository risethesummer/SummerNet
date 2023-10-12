namespace Realtime.Controllers.Objects;

public partial interface INetworkBehaviour : IDisposable
{
    NetworkObject? NetworkObject { get; }
}