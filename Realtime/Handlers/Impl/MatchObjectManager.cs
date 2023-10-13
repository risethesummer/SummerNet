using System.Collections.Concurrent;
using Autofac;
using Realtime.Handlers.Interfaces;
using Realtime.Objects;
using Realtime.Transporters.Payloads;

namespace Realtime.Handlers.Impl;

public class MatchObjectManager<TPlayerIndex> : IMatchObjectManager<TPlayerIndex>
{
    private readonly List<NetworkObject?> _objects = new();
    private readonly ConcurrentQueue<int> _emptySlots = new();
    public IEnumerable<NetworkObject> Objects => _objects;
    private readonly ILifetimeScope _rootScope;
    public ValueTask<NetworkObject?> SpawnAsync(NetworkObject? parent, TPlayerIndex? author, IDependenciesInstaller? installer)
    {
        // Send create message
        var scope = parent?.Scope ?? _rootScope;
        if (!_emptySlots.TryDequeue(out var empty))
        {
            _objects.Add(null);
            empty = _objects.Count;
        }
        var obj = scope.Resolve<NetworkObject>(
            TypedParameter.From(new NetworkIndex(empty)), 
            TypedParameter.From(author));
        obj.Scope = installer?.Install(obj, scope);
        _objects[empty] = obj;
        return ValueTask.FromResult<NetworkObject?>(obj);
    }

    public ValueTask<TBehaviour?> RegisterBehaviourAsync<TBehaviour>(NetworkObject networkObject, 
        IDependenciesInstaller? installer) where TBehaviour : INetworkBehaviour
    {
        var scope = networkObject.Scope ?? _rootScope;
        var behaviour = scope.Resolve<TBehaviour>(TypedParameter.From(networkObject));
        scope = installer?.Install(behaviour, scope);
        behaviour.Scope = scope;
        return RegisterBehaviourAsync(networkObject, behaviour);
    }

    public ValueTask<TBehaviour?> RemoveBehaviourAsync<TBehaviour>(NetworkObject networkObject) where TBehaviour : INetworkBehaviour
    {

    }

    public ValueTask<TBehaviour?> RegisterBehaviourAsync<TBehaviour>(NetworkObject networkObject, TBehaviour behaviour) where TBehaviour : INetworkBehaviour
    {
        networkObject.AddBehaviour(behaviour);
        return ValueTask.FromResult<TBehaviour?>(behaviour);
    }

    public ValueTask<NetworkObject?> DespawnAsync(NetworkObject networkObject)
    {
    }
}