public interface IMatchObjectManager<TPlayerIndex> where TPlayerIndex : unmanaged
{
    IEnumerable<NetworkObject> Objects { get; }
    // Add and assign networkIndex
    ValueTask<T> SpawnAsync<T>(NetworkObject? parent, TPlayerIndex? author, IDependenciesInstaller? installer);
    ValueTask<TBehaviour> RegisterBehaviourAsync<TBehaviour>(NetworkObject networkObject, IDependenciesInstaller? installer) 
        where TBehaviour : INetworkBehaviour;
    ValueTask<TBehaviour> RegisterBehaviourAsync<TBehaviour>(NetworkObject networkObject, TBehaviour behaviour) 
        where TBehaviour : INetworkBehaviour;
    ValueTask DespawnAsync(NetworkObject networkObject);
}