// See https://aka.ms/new-console-template for more information
using System.Numerics;
using Autofac;
using Realtime.Attributes;
using Realtime.Controllers.Match.Interfaces;
using Realtime.Controllers.Setup.Interfaces;
using Realtime.Controllers.Transporters.Interfaces;
using Realtime.Controllers.Transporters.Messages;
using Realtime.Controllers.Transporters.Payloads;
using Realtime.Data;

public static class SummerNetApplication
{
    public static ValueTask<MatchContext> Run(ContainerBuilder containerBuilder)
    {
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
public class MatchSetup : IMatchSetupRunner<MatchContext, MyMatchData, int, int, MyPlayerData>
{
    private readonly IMatchDataProvider<MyMatchData, int, MyPlayerData> _matchDataProvider;
    public async ValueTask<MatchContext> Setup()
    {
        var matchData = await _matchDataProvider.ProvideMatchData();
        ConfigurationManager.AppSettings["occupation"];
        return new MatchContext
    }
}



// Create your builder.
// // Usually you're only interested in exposing the type
// // via its interface:
// rootBuilder.RegisterType<SomeType>().As<IService>();
// // However, if you want BOTH services (not as common)
// // you can say so:
// rootBuilder.RegisterType<SomeType>().AsSelf().As<IService>().Wi;
// var container  = rootBuilder.Build();
// container.Resolve<>();

public class MyPlayerData : PlayerData<int, int>
{ }
public class MyMatchData : MatchData<int, int, MyPlayerData>
{ }

public interface IDependenciesInstaller : IDisposable
{
    ILifetimeScope? Install<T>(in T injectable, ILifetimeScope scope) where T : IManagedNetwork;
}

public interface IManagedNetwork : IDisposable
{
    public ILifetimeScope? Scope { set; get; }
}

public readonly struct NetworkInt : INetworkPayload
{
    public int Value { get; init; }
}

public readonly struct NetworkIndex : INetworkPayload
{
    public ulong Value { get; init; }
    public bool IsValid => Value > 0;
    public static NetworkIndex Invalid => default;
    public ulong SequenceIndex => Value - 1;
}

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

/// <summary>
/// The implementations will be generated
/// Make sure the concrete class is partial
/// </summary>
public sealed class NetworkObject : IManagedNetwork
{
    private readonly List<INetworkBehaviour> _behaviours;
    public IReadOnlyList<INetworkBehaviour> Behaviours => _behaviours;
    public bool IsValid => NetworkIndex.IsValid && MatchContext != null;
    public NetworkIndex NetworkIndex { get; internal set; } //Auto-incremental id
    public MatchContext? MatchContext { get; init; }
    public MyPlayerData? Author { get; init; }
    public void Dispose()
    {
        Scope?.Dispose();
        foreach (var behaviour in _behaviours)
            behaviour.Dispose();
    }
    public void AddBehaviour(INetworkBehaviour behaviour)
    {
        _behaviours.Add(behaviour);
    }
    public ILifetimeScope? Scope { get; set; }
}

public interface INetworkBehaviour : IDisposable
{
    NetworkObject? NetworkObject { get; }
    MatchContext? MatchContext { get; }
    MyPlayerData? Author { get; }
}

public partial class MoveBehaviour : INetworkBehaviour, 
    IMatchMessageHandler<NetworkedVector3, MyMatchData, int, int, MyPlayerData>
{
    [SyncVar("Position")] private Vector3 _position;
    [Rpc] 
    private void RpcMove(in NetworkedVector3 networkedVector3)
    {
        Position += networkedVector3;
    }

    // Generated
    void IMatchMessageHandler<NetworkedVector3, MyMatchData, int, int, MyPlayerData>.OnMessage(
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
public abstract class MatchContext : IMatchContext<MyMatchData, int, int, MyPlayerData>
{
    public MyMatchData MatchData { get; }

    public ValueTask<ReadOnlyMemory<RawReceivedNetworkMessage<int>>> FlushReceivedMessagesAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public ValueTask StartReceivingMessagesAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public ValueTask SendMessageInline<T>(in SentNetworkMessage<int, T> msg, CancellationToken cancellationToken) where T : unmanaged, INetworkPayload
    {
        throw new NotImplementedException();
    }

    public ValueTask SendMessage<T>(in SentNetworkMessage<int, T> msg, CancellationToken cancellationToken) where T : unmanaged, INetworkPayload
    {
        throw new NotImplementedException();
    }

    public ValueTask FlushSentMessage(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public ValueTask RemovePlayerAsync(int target, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public ValueTask<MyPlayerData> AddPlayerAsync(MyPlayerData playerData, ISocket socket)
    {
        throw new NotImplementedException();
    }

    public void Clear()
    {
        throw new NotImplementedException();
    }

    public void RegisterInitializeHandler(IMatchInitHandler<MyMatchData, int, int, MyPlayerData> handler)
    {
        throw new NotImplementedException();
    }

    public void RegisterMatchJoinHandler(IMatchJoinHandler<MyMatchData, int, int, MyPlayerData> handler)
    {
        throw new NotImplementedException();
    }

    public void RegisterMatchLeaveHandler(IMatchLeaveHandler<MyMatchData, int, int, MyPlayerData> handler)
    {
        throw new NotImplementedException();
    }

    public void RegisterMatchTickHandler(IMatchTickHandler<MyMatchData, int, int, MyPlayerData> handler)
    {
        throw new NotImplementedException();
    }

    public void RegisterMatchShutdownHandler(IMatchShutdownHandler<MyMatchData, int, int, MyPlayerData> handler)
    {
        throw new NotImplementedException();
    }

    public ValueTask<MyMatchData> StartMatch()
    {
        throw new NotImplementedException();
    }

    public ValueTask<MyMatchData> Shutdown()
    {
        throw new NotImplementedException();
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
