// See https://aka.ms/new-console-template for more information

using System.Net;
using System.Numerics;
using System.Runtime.CompilerServices;
using Autofac;
using Cysharp.Threading;
using Microsoft.Extensions.Configuration;
using Realtime.Attributes;
using Realtime.Controllers.Match.Impl;
using Realtime.Controllers.Match.Interfaces;
using Realtime.Controllers.Objects;
using Realtime.Controllers.Setup.Data;
using Realtime.Controllers.Setup.Interfaces;
using Realtime.Controllers.Transporters.Impl;
using Realtime.Controllers.Transporters.Interfaces;
using Realtime.Controllers.Transporters.Messages;
using Realtime.Controllers.Transporters.Payloads;
using Realtime.Data;
using Realtime.Utils.Buffers;
using Realtime.Utils.Factory;

namespace Sample;

public class MyPlayerData : PlayerData<int, int>
{ }
public class MyMatchData : MatchData<int, int, MyPlayerData>
{ }


public static class SummerNetApplication
{
    private class TaskQueueModule : Module
    {
        protected override void Load(ContainerBuilder moduleBuilder)
        {
            moduleBuilder.RegisterType<Factory<PoolableWrapper<DisposableQueue<Task>>>>()
                .As<IFactory<PoolableWrapper<DisposableQueue<Task>>>>()
                .InstancePerDependency();
            moduleBuilder.RegisterDecorator<PoolFactory<PoolableWrapper<DisposableQueue<Task>>>, 
                IFactory<PoolableWrapper<DisposableQueue<Task>>>>();
        }
    }
    
    private class AcceptorModule : Module
    {
        private readonly RealtimeConfiguration _configuration;
        public AcceptorModule(RealtimeConfiguration configuration)
        {
            _configuration = configuration;
        }
        protected override void Load(ContainerBuilder moduleBuilder)
        {
            var max = _configuration.MaxPlayers > 0 ? _configuration.MaxPlayers : int.MaxValue;
            var endpoint = new IPEndPoint(
                IPAddress.Parse(_configuration.Endpoint),
                _configuration.Port
            );
            moduleBuilder.RegisterType<RawSocketConnectionAcceptor>()
                .As<IConnectionAcceptor>()
                .WithParameters(new []
                {
                    TypedParameter.From(endpoint),
                    TypedParameter.From(max)
                })
                .SingleInstance()
                .OwnedByLifetimeScope();
            moduleBuilder.RegisterType<PlayerAcceptor<int, int, MyPlayerData>>()
                .As<IPlayerAcceptor<int, int, MyPlayerData>>()
                .SingleInstance();
        }
    }
    /// <summary>
    /// Pool for reusable UnmanagedMemoryManager
    /// </summary>
    private class MemoryManagerModule : Module
    {
        protected override void Load(ContainerBuilder moduleBuilder)
        {
            moduleBuilder.RegisterType<UnmanagedMemoryManager<byte>>()
                .AsSelf()
                .InstancePerDependency()
                .OwnedByLifetimeScope(); // Call dispose for all UnmanagedMemoryManager<byte>
            moduleBuilder
                .RegisterType<Factory<BufferPointer<byte>, 
                    PoolableWrapper<BufferPointer<byte>, UnmanagedMemoryManager<byte>>>>()
                .As<IFactory<BufferPointer<byte>, 
                    PoolableWrapper<BufferPointer<byte>, UnmanagedMemoryManager<byte>>>>()
                .InstancePerDependency();
            moduleBuilder
                .RegisterDecorator<PoolFactory<BufferPointer<byte>, 
                        PoolableWrapper<BufferPointer<byte>, UnmanagedMemoryManager<byte>>>, 
                    IFactory<BufferPointer<byte>, 
                        PoolableWrapper<BufferPointer<byte>, UnmanagedMemoryManager<byte>>>>();
            
        }
    }

    private class TransportModule : Module
    {
        protected override void Load(ContainerBuilder moduleBuilder)
        {
            moduleBuilder.RegisterType<MessageEncoder>()
                .As<IMessageEncoder>()
                .SingleInstance();
            moduleBuilder.RegisterType<MessageDecoder>()
                .As<IMessageDecoder>()
                .SingleInstance();
            moduleBuilder.RegisterType<ParallelBuffer<RawReceivedNetworkMessage<int>>>()
                .InstancePerDependency();
            moduleBuilder.RegisterType<ParallelBuffer<DecodedSentMessage<int>>>()
                .InstancePerDependency();
            moduleBuilder.RegisterType<SocketTransporter<int, int, MyPlayerData>>()
                .As<ITransporter<int, int, MyPlayerData>>()
                .SingleInstance().OwnedByLifetimeScope();
        }
    }
    private static MatchContext CreateMatchContext(ContainerBuilder containerBuilder, 
        RealtimeConfiguration configuration, 
        MyMatchData matchData)
    {
        containerBuilder
            .RegisterModule<TaskQueueModule>()
            .RegisterModule<MemoryManagerModule>()
            .RegisterModule<TransportModule>()
            .RegisterModule(new AcceptorModule(configuration));
        return containerBuilder.Build().Resolve<MatchContext>();
    }
    private static RealtimeConfiguration CreateMatchConfiguration(IConfiguration configuration)
    {
        var realtimeConfigure = configuration
            .GetSection(RealtimeConfiguration.ConfigureSection)
            .Get<RealtimeConfiguration>();
        if (realtimeConfigure is null)
            throw new ArgumentNullException(nameof(RealtimeConfiguration), "Did not detect any configure section whose name Realtime");
        return realtimeConfigure;
    }

    private static async ValueTask<MyMatchData> CreateMatchData()
    {
        IMatchDataProvider<MyMatchData, int, MyPlayerData> matchDataProvider = null; 
        var matchData = await matchDataProvider.ProvideMatchData();
        if (matchData is null)
            throw new ArgumentNullException(nameof(IMatchDataProvider<MyMatchData, int, MyPlayerData>), "The provider does not give any match data");
        return matchData;
    } 
    public static async ValueTask<MatchContext> Run(
        IConfiguration configuration,
        ContainerBuilder containerBuilder)
    {
        var realtimeConfigure = CreateMatchConfiguration(configuration);
        containerBuilder.RegisterInstance(realtimeConfigure)
            .SingleInstance();
        var matchData = await CreateMatchData();
        containerBuilder.RegisterInstance(matchData)
            .SingleInstance();
        return CreateMatchContext(containerBuilder, realtimeConfigure, matchData);
    }
}

// Generated
public class BasicMatchDataProvider : IMatchDataProvider<MyMatchData, int, MyPlayerData>
{
    public ValueTask<MyMatchData> ProvideMatchData()
    {
        return ValueTask.FromResult(new MyMatchData
        {
            Players = new Dictionary<int, MyPlayerData>()
        });
    }
}

public partial interface INetworkBehaviour : IDisposable
{
    MatchContext? MatchContext { get; }
    MyPlayerData? Author { get; }
    void AddHandlers(IMatchTickManager<MyMatchData, int, int, MyPlayerData> handlerManager);
}

/// <summary>
/// The implementations will be generated
/// Make sure the concrete class is partial
/// </summary>
public sealed partial class NetworkObject : IManagedNetwork
{
    // Generated parts
    private readonly List<INetworkBehaviour> _behaviours;
    public IReadOnlyList<INetworkBehaviour> Behaviours => _behaviours;
    public MatchContext? MatchContext { get; init; }
    public MyPlayerData? Author { get; set; }
    public NetworkObject(MatchContext context, List<INetworkBehaviour> behaviours, 
        NetworkIndex index, MyPlayerData? author)
    {
        MatchContext = context;
        NetworkIndex = index;
        Author = author;
        _behaviours = behaviours;
    }
    public void Dispose()
    {
        Scope?.Dispose();
        foreach (var behaviour in _behaviours)
            behaviour.Dispose();
    }

}
public partial class MoveBehaviour : INetworkBehaviour, 
    IMatchMessageHandler<NetworkedVector3, int>
{
    [SyncVar("Position")] private Vector3 _position;
    [Rpc] 
    private void RpcMove(in NetworkedVector3 networkedVector3)
    {
        Position += networkedVector3;
    }

    // Generated
    void IMatchMessageHandler<NetworkedVector3, int>.OnMessage(
        IMatchContext<MyMatchData, int, int, MyPlayerData> matchContext, 
        in ReceivedNetworkMessage<int, NetworkedVector3> message)
    {
        RpcMove(message.Payload);
    }
    public NetworkObject? NetworkObject { get; init; }
    public MatchContext? MatchContext => NetworkObject?.MatchContext;
    public MyPlayerData? Author => NetworkObject?.Author;
    public Vector3 Position
    {
        get => _position;
        set
        {
            if (value == _position) //Nothing changes
                return;
            var ctx = MatchContext;
            ctx?.SendMessage(new SentNetworkMessage<int, NetworkedVector3>
            {
                Opcode = 0,
                Payload = _position,
                MessageType = MessageType.Broadcast
            }, default);
        }
    }
    
    public void Dispose()
    {
        
    }

}

// Generated
public sealed class MatchContext : IMatchContext<MyMatchData, int, int, MyPlayerData>
{
    public MyMatchData MatchData { get; }
    private readonly RealtimeConfiguration _configuration;
    private readonly IMatchObjectManager<int> _matchObjectManager;
    private readonly ITransporter<int, int, MyPlayerData> _transporter;
    private readonly IPlayerAcceptor<int, int, MyPlayerData> _playerAcceptor;
    private readonly IMatchTickManager<MyMatchData, int, int, MyPlayerData> _tickManager;
    private readonly List<IMatchJoinHandler<MyMatchData, int, int, MyPlayerData>> _joinHandlers = new();
    private readonly List<IMatchLeaveHandler<MyMatchData, int, int, MyPlayerData>> _leaveHandlers = new();
    private readonly List<IMatchInitHandler<MyMatchData, int, int, MyPlayerData>> _initHandlers = new();
    private readonly List<IMatchShutdownHandler<MyMatchData, int, int, MyPlayerData>> _shutdownHandlers = new();
    public IEnumerable<Realtime.Controllers.Objects.NetworkObject> Objects => _matchObjectManager.Objects;
    public ValueTask<ReadOnlyMemory<RawReceivedNetworkMessage<int>>> FlushReceivedMessagesAsync(
        CancellationToken cancellationToken)
        => _transporter.FlushReceivedMessagesAsync(cancellationToken);
    public ValueTask StartReceivingMessagesAsync(CancellationToken cancellationToken)
        => _transporter.StartReceivingMessagesAsync(cancellationToken);
    public ValueTask SendMessageInline<T>(in SentNetworkMessage<int, T> msg, CancellationToken cancellationToken)
        => _transporter.SendMessageInline(msg, cancellationToken);
    public ValueTask SendMessage<T>(in SentNetworkMessage<int, T> msg, CancellationToken cancellationToken)
        => _transporter.SendMessage(msg, cancellationToken);
    public ValueTask FlushSentMessage(CancellationToken cancellationToken)
        => _transporter.FlushSentMessage(cancellationToken);
    public ValueTask RemovePlayerAsync(int target, CancellationToken cancellationToken)
    {
        MatchData.Players.Remove(target);
        return _transporter.RemovePlayerAsync(target, cancellationToken);
    }
    public ValueTask<MyPlayerData> AddPlayerAsync(MyPlayerData playerData, ISocket socket)
    {
        MatchData.Players.TryAdd(playerData.PlayerId, playerData);
        return _transporter.AddPlayerAsync(playerData, socket);
    }
    public void Clear()
    {
        _transporter.Clear();
    }

    public async ValueTask<MyMatchData> StartMatch(CancellationToken cancellationToken)
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

    public async ValueTask<MyMatchData> Shutdown()
    {
        MatchData.Status = MatchStatus.Ended;
        return MatchData;
    }

    public async ValueTask<Realtime.Controllers.Objects.NetworkObject?> SpawnAsync(Realtime.Controllers.Objects.NetworkObject? parent, int? author, 
        IDependenciesInstaller? installer)
    {
        var obj = await _matchObjectManager.SpawnAsync(parent, author, installer);
        if (obj is not null)
        {
            foreach (var behaviour in obj.Behaviours)
                behaviour.AddHandlers(_tickManager);
            // Send obj created message
            ...
        }
        return obj;
    }

    public async ValueTask<TBehaviour?> RegisterBehaviourAsync<TBehaviour>(
        Realtime.Controllers.Objects.NetworkObject networkObject, 
        IDependenciesInstaller? installer) 
        where TBehaviour : Realtime.Controllers.Objects.INetworkBehaviour
    {
        var behaviour = await _matchObjectManager.RegisterBehaviourAsync<TBehaviour>(networkObject, installer);
        if (behaviour is not null)
        {
            behaviour.AddHandlers(_tickManager);
            // Send add behaviour message
            ...
        }
        return behaviour;
    }
    
    
    public async ValueTask<TBehaviour?> RegisterBehaviourAsync<TBehaviour>(Realtime.Controllers.Objects.NetworkObject networkObject, 
        TBehaviour behaviour) where TBehaviour : Realtime.Controllers.Objects.INetworkBehaviour
    {
        behaviour = await _matchObjectManager.RegisterBehaviourAsync(networkObject, behaviour);
        if (behaviour is not null)
        {
            behaviour.AddHandlers(_tickManager);
            // Send add behaviour message
            ...
        }
        return behaviour;
    }

    public async ValueTask<TBehaviour?> RemoveBehaviourAsync<TBehaviour>(Realtime.Controllers.Objects.NetworkObject networkObject) 
        where TBehaviour : Realtime.Controllers.Objects.INetworkBehaviour
    {
        var behaviour = await _matchObjectManager.RemoveBehaviourAsync<TBehaviour>(networkObject);
        if (behaviour is not null)
        {
            behaviour.RemoveHandlers(_tickManager);
            // Send remove behaviour message
            ...
        }
    }


    public async ValueTask<Realtime.Controllers.Objects.NetworkObject?>
        DespawnAsync(Realtime.Controllers.Objects.NetworkObject networkObject)
    {
        networkObject = await _matchObjectManager.DespawnAsync(networkObject);
        if (networkObject is not null)
        {
            foreach (var behaviour in networkObject.Behaviours)
                behaviour.RemoveHandlers(_tickManager);
            // Send destroy message
            ...
        }
        return networkObject;
    }

    public void RegisterInitializeHandler(IMatchInitHandler<MyMatchData, int, int, MyPlayerData> handler)
    {
        _initHandlers.Add(handler);
    }

    public void RegisterMatchShutdownHandler(IMatchShutdownHandler<MyMatchData, int, int, MyPlayerData> handler)
    {
        _shutdownHandlers.Add(handler);
    }

    public void RegisterMatchJoinHandler(IMatchJoinHandler<MyMatchData, int, int, MyPlayerData> handler)
    {
        _joinHandlers.Add(handler);
    }

    public void RegisterMatchLeaveHandler(IMatchLeaveHandler<MyMatchData, int, int, MyPlayerData> handler)
    {
        _leaveHandlers.Add(handler);
    }

    public void RegisterMatchTickHandler(IMatchTickHandler<MyMatchData, int, int, MyPlayerData> handler)
    {
        _tickManager.AddHandler(handler);
    }

    public async IAsyncEnumerable<MyPlayerData> BeginAccepting([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await foreach (var player in  _playerAcceptor.BeginAccepting(cancellationToken))
        {
            if (_configuration.MaxPlayers > 0 || MatchData.Players.Count > _configuration.MaxPlayers)
                continue;
            yield return player;
        }
    }
}

// HttpListener httpListener = new HttpListener();
// httpListener.Prefixes.Add("http://localhost/");
// httpListener.Start();
//
// HttpListenerContext context = await httpListener.GetContextAsync();
// if (context.Request.IsWebSocketRequest)
// {
//     HttpListenerWebSocketContext webSocketContext = await context.AcceptWebSocketAsync(null);
//     WebSocket webSocket = webSocketContext.WebSocket;
//     while (webSocket.State == WebSocketState.Open)
//     {
//         await webSocket.SendAsync( ... );
//     }
// }


// // var myClass = host.Services.GetService<MyClass>();
// // await myClass!.DoStuff();
// //
// await host.RunAsync();

// Generate all code depending all 2 classes