using System.Runtime.CompilerServices;
using Cysharp.Threading;
using Realtime.Data;
using Realtime.Handlers.Interfaces;
using Realtime.Objects;
using Realtime.Setup.Data;
using Realtime.Transporters.Interfaces;
using Realtime.Transporters.Messages;

namespace Realtime.Handlers.Impl;

public sealed class MatchContext<TMatchData, TPlayerIndex, TAuthData, TPlayer> : IMatchContext<TMatchData, TPlayerIndex, TAuthData, TPlayer>
    where TPlayer : PlayerData<TPlayerIndex, TAuthData>, new()
    where TMatchData : MatchData<TPlayerIndex, TAuthData, TPlayer>
{
    public TMatchData MatchData { get; }
    private readonly RealtimeConfiguration _configuration;
    private readonly IMatchObjectManager<TPlayerIndex> _matchObjectManager;
    private readonly ITransporter<TPlayerIndex, TAuthData, TPlayer> _transporter;
    private readonly IPlayerAcceptor<TPlayerIndex, TAuthData, TPlayer> _playerAcceptor;
    private readonly IMatchTickManager<TMatchData, TPlayerIndex, TAuthData, TPlayer> _tickManager;
    private readonly List<IMatchJoinHandler<TMatchData, TPlayerIndex, TAuthData, TPlayer>> _joinHandlers = new();
    private readonly List<IMatchLeaveHandler<TMatchData, TPlayerIndex, TAuthData, TPlayer>> _leaveHandlers = new();
    private readonly List<IMatchInitHandler<TMatchData, TPlayerIndex, TAuthData, TPlayer>> _initHandlers = new();
    private readonly List<IMatchShutdownHandler<TMatchData, TPlayerIndex, TAuthData, TPlayer>> _shutdownHandlers = new();
    public IEnumerable<NetworkObject> Objects => _matchObjectManager.Objects;
    public ValueTask<ReadOnlyMemory<RawReceivedNetworkMessage<TPlayerIndex>>> FlushReceivedMessagesAsync(
        CancellationToken cancellationToken)
        => _transporter.FlushReceivedMessagesAsync(cancellationToken);
    public ValueTask StartReceivingMessagesAsync(CancellationToken cancellationToken)
        => _transporter.StartReceivingMessagesAsync(cancellationToken);
    public ValueTask SendMessageInline<T>(in SentNetworkMessage<TPlayerIndex, T> msg, CancellationToken cancellationToken)
        => _transporter.SendMessageInline(msg, cancellationToken);
    public ValueTask SendMessage<T>(in SentNetworkMessage<TPlayerIndex, T> msg, CancellationToken cancellationToken)
        => _transporter.SendMessage(msg, cancellationToken);
    public ValueTask FlushSentMessage(CancellationToken cancellationToken)
        => _transporter.FlushSentMessage(cancellationToken);
    public ValueTask RemovePlayerAsync(TPlayerIndex target, CancellationToken cancellationToken)
    {
        MatchData.Players.Remove(target);
        return _transporter.RemovePlayerAsync(target, cancellationToken);
    }
    public ValueTask<TPlayer?> AddPlayerAsync(TPlayer playerData, ISocket socket)
    {
        MatchData.Players.TryAdd(playerData.PlayerId, playerData);
        return _transporter.AddPlayerAsync(playerData, socket);
    }
    public void Clear()
    {
        _transporter.Clear();
    }

    public async ValueTask<TMatchData> StartMatch(CancellationToken cancellationToken)
    {
        MatchData.Status = MatchStatus.Running;
        using var looper = new LogicLooper(MatchData.Ticks, 1);
        await looper.RegisterActionAsync(async (_, token) =>
        {
            if (MatchData.IsRunning)
                await _tickManager.Tick(this, token);
            return token.IsCancellationRequested; 
        }, cancellationToken).ConfigureAwait(false);
        return MatchData;
    }

    public ValueTask<TMatchData> Shutdown()
    {
        MatchData.Status = MatchStatus.Ended;
        return ValueTask.FromResult(MatchData);
    }

    public async ValueTask<NetworkObject?> SpawnAsync(NetworkObject? parent, TPlayerIndex? author, 
        IDependenciesInstaller? installer)
    {
        var obj = await _matchObjectManager.SpawnAsync(parent, author, installer);
        if (obj is not null)
        {
            // foreach (var behaviour in obj.Behaviours)
            //     behaviour.AddHandlers(_tickManager);
            // // Send obj created message
            // ...
        }
        return obj;
    }


    public async ValueTask<TBehaviour?> RegisterBehaviourAsync<TBehaviour>(
        NetworkObject networkObject, 
        IDependenciesInstaller? installer) 
        where TBehaviour : Realtime.Objects.INetworkBehaviour
    {
        var behaviour = await _matchObjectManager.RegisterBehaviourAsync<TBehaviour>(networkObject, installer);
        if (behaviour is not null)
        {
            // behaviour.AddHandlers(_tickManager);
            // // Send add behaviour message
            // ...
        }
        return behaviour;
    }
    
    
    public async ValueTask<TBehaviour?> RegisterBehaviourAsync<TBehaviour>(NetworkObject networkObject, 
        TBehaviour behaviour) where TBehaviour : Realtime.Objects.INetworkBehaviour
    {
        var result = await _matchObjectManager.RegisterBehaviourAsync(networkObject, behaviour);
        if (result is not null)
        {
            // behaviour.AddHandlers(_tickManager);
            // // Send add behaviour message
            // ...
        }
        return result;
    }

    public async ValueTask<TBehaviour?> RemoveBehaviourAsync<TBehaviour>(NetworkObject networkObject) 
        where TBehaviour : Realtime.Objects.INetworkBehaviour
    {
        var behaviour = await _matchObjectManager.RemoveBehaviourAsync<TBehaviour>(networkObject);
        if (behaviour is not null)
        {
            // behaviour.RemoveHandlers(_tickManager);
            // // Send remove behaviour message
            // ...
        }

        return behaviour;
    }


    public async ValueTask<NetworkObject?>
        DespawnAsync(NetworkObject networkObject)
    {
        var result = await _matchObjectManager.DespawnAsync(networkObject);
        if (result is not null)
        {
            // foreach (var behaviour in networkObject.Behaviours)
            //     behaviour.RemoveHandlers(_tickManager);
            // // Send destroy message
            // ...
        }
        return result;
    }

    public void RegisterInitializeHandler(IMatchInitHandler<TMatchData, TPlayerIndex, TAuthData, TPlayer> handler)
    {
        _initHandlers.Add(handler);
    }

    public void RegisterMatchShutdownHandler(IMatchShutdownHandler<TMatchData, TPlayerIndex, TAuthData, TPlayer> handler)
    {
        _shutdownHandlers.Add(handler);
    }

    public void RegisterMatchJoinHandler(IMatchJoinHandler<TMatchData, TPlayerIndex, TAuthData, TPlayer> handler)
    {
        _joinHandlers.Add(handler);
    }

    public void RegisterMatchLeaveHandler(IMatchLeaveHandler<TMatchData, TPlayerIndex, TAuthData, TPlayer> handler)
    {
        _leaveHandlers.Add(handler);
    }

    public void RegisterMatchTickHandler(IMatchTickHandler<TMatchData, TPlayerIndex, TAuthData, TPlayer> handler)
    {
        _tickManager.AddHandler(handler);
    }

    public async IAsyncEnumerable<TPlayer> BeginAccepting([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await foreach (var player in  _playerAcceptor.BeginAccepting(cancellationToken))
        {
            if (_configuration.MaxPlayers > 0 || MatchData.Players.Count > _configuration.MaxPlayers)
                continue;
            yield return player;
        }
    }
}