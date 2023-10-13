using Realtime.Objects;

namespace Realtime.Handlers.Interfaces;

public interface IMatchObjectManager<in TPlayerIndex>
{
    IEnumerable<NetworkObject> Objects { get; }
    // Add and assign networkIndex
    ValueTask<NetworkObject?> SpawnAsync(NetworkObject? parent, TPlayerIndex? author, IDependenciesInstaller? installer);
    ValueTask<TBehaviour?> RegisterBehaviourAsync<TBehaviour>(NetworkObject networkObject, IDependenciesInstaller? installer) 
        where TBehaviour : INetworkBehaviour;
    ValueTask<TBehaviour?> RemoveBehaviourAsync<TBehaviour>(NetworkObject networkObject) 
        where TBehaviour : INetworkBehaviour;
    ValueTask<TBehaviour?> RegisterBehaviourAsync<TBehaviour>(NetworkObject networkObject, TBehaviour behaviour) 
        where TBehaviour : INetworkBehaviour;
    ValueTask<NetworkObject?> DespawnAsync(NetworkObject networkObject);
}