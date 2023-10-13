namespace Realtime.Objects;

public partial interface INetworkBehaviour : IManagedNetwork
{
    NetworkObject? NetworkObject { get; }
}