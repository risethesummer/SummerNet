﻿using Autofac;
using Realtime.Controllers.Transporters.Payloads;

namespace Realtime.Controllers.Objects;

public sealed partial class NetworkObject : IManagedNetwork
{
    public bool IsValid => NetworkIndex.IsValid;
    public NetworkIndex NetworkIndex { get; init; } //Auto-incremental id
    public ILifetimeScope? Scope { get; set; }
    private readonly HashSet<INetworkBehaviour> _behaviours;
    public IEnumerable<INetworkBehaviour> Behaviours => _behaviours;
    public void AddBehaviour(INetworkBehaviour behaviour)
    {
        _behaviours.Add(behaviour);
    }
    public void RemoveBehaviour(INetworkBehaviour behaviour)
    {
        _behaviours.Remove(behaviour);
    }
}