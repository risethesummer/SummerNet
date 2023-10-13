// // See https://aka.ms/new-console-template for more information
//

using System.Runtime.CompilerServices;
using Autofac;
using Cysharp.Threading;
using Microsoft.Extensions.Configuration;
using Realtime.Data;
using Realtime.Handlers.Impl;
using Realtime.Handlers.Interfaces;
using Realtime.Modules;
using Realtime.Objects;
using Realtime.Setup.Data;
using Realtime.Setup.Interfaces;
using Realtime.Transporters.Interfaces;
using Realtime.Transporters.Messages;
using Realtime.Utils.Buffers;
using Realtime.Utils.Factory;
using Sample;
using NetworkObject = Sample.NetworkObject;

Console.WriteLine("Hello world");

public class TPlayer : PlayerData<int, int>
{ }
public class MyMatchData : MatchData<int, int, TPlayer>
{ }

public static partial class SummerNetApplication
{
    private static RealtimeConfiguration CreateMatchConfiguration(IConfiguration configuration)
    {
        var realtimeConfigure = configuration
            .GetSection(RealtimeConfiguration.ConfigureSection)
            .Get<RealtimeConfiguration>();
        if (realtimeConfigure is null)
            throw new ArgumentNullException(nameof(RealtimeConfiguration), "Did not detect any configure section whose name Realtime");
        return realtimeConfigure;
    }
 

    private static MatchContext CreateMatchContext(ContainerBuilder containerBuilder, 
        RealtimeConfiguration configuration)
    {
        containerBuilder
            .RegisterModule<TaskQueueModule>()
            .RegisterModule<MemoryManagerModule>()
            .RegisterModule(new TransportModule(configuration));
        return containerBuilder.Build().Resolve<MatchContext>();
    }
    private static async ValueTask<MyMatchData> CreateMatchData(
        IMatchDataProvider<MyMatchData, int, TPlayer>? dataProvider)
    {
        var matchData = await dataProvider.ProvideMatchData();
        if (matchData is null)
            throw new ArgumentNullException(nameof(IMatchDataProvider<MyMatchData, int, TPlayer>), "The provider does not give any match data");
        return matchData;
    }
    public static async ValueTask<MatchContext> Run(IConfiguration configuration,
        IMatchDataProvider<MyMatchData, int, TPlayer>? matchDataProvider,
        ContainerBuilder containerBuilder)
    {
        var realtimeConfigure = CreateMatchConfiguration(configuration);
        containerBuilder.RegisterInstance(realtimeConfigure)
            .SingleInstance();
        var matchData = await CreateMatchData(matchDataProvider);
        containerBuilder.RegisterInstance(matchData)
            .SingleInstance();
        return CreateMatchContext(containerBuilder, realtimeConfigure);
    }
}



// Generated
// public class BasicMatchDataProvider : IMatchDataProvider<MyMatchData, int, TPlayer>
// {
//     public ValueTask<MyMatchData> ProvideMatchData()
//     {
//         return ValueTask.FromResult(new MyMatchData
//         {
//             Players = new Dictionary<int, TPlayer>()
//         });
//     }
// }

// public partial interface INetworkBehaviour
// {
//     MatchContext? MatchContext { get; }
//     TPlayer? Author { get; }
//     void AddHandlers(IMatchTickManager<MyMatchData, int, int, TPlayer> handlerManager);
// }

// public partial class MoveBehaviour : INetworkBehaviour, 
//     IMatchMessageHandler<NetworkedVector3, int>
// {
//     [SyncVar("Position")] private Vector3 _position;
//     [Rpc] 
//     private void RpcMove(in NetworkedVector3 networkedVector3)
//     {
//         Position += networkedVector3;
//     }
//
//     // Generated
//     void IMatchMessageHandler<NetworkedVector3, int>.OnMessage(
//         IMatchContext<MyMatchData, int, int, TPlayer> matchContext, 
//         in ReceivedNetworkMessage<int, NetworkedVector3> message)
//     {
//         RpcMove(message.Payload);
//     }
//     public NetworkObject? NetworkObject { get; init; }
//     public MatchContext? MatchContext => NetworkObject?.MatchContext;
//     public TPlayer? Author => NetworkObject?.Author;
//     public Vector3 Position
//     {
//         get => _position;
//         set
//         {
//             if (value == _position) //Nothing changes
//                 return;
//             var ctx = MatchContext;
//             ctx?.SendMessage(new SentNetworkMessage<int, NetworkedVector3>
//             {
//                 Opcode = 0,
//                 Payload = _position,
//                 MessageType = MessageType.Broadcast
//             }, default);
//         }
//     }
//     
//     public void Dispose()
//     {
//         
//     }
//
// }

// Generated


//
// // HttpListener httpListener = new HttpListener();
// // httpListener.Prefixes.Add("http://localhost/");
// // httpListener.Start();
// //
// // HttpListenerContext context = await httpListener.GetContextAsync();
// // if (context.Request.IsWebSocketRequest)
// // {
// //     HttpListenerWebSocketContext webSocketContext = await context.AcceptWebSocketAsync(null);
// //     WebSocket webSocket = webSocketContext.WebSocket;
// //     while (webSocket.State == WebSocketState.Open)
// //     {
// //         await webSocket.SendAsync( ... );
// //     }
// // }
//
//
// // // var myClass = host.Services.GetService<MyClass>();
// // // await myClass!.DoStuff();
// // //
// // await host.RunAsync();
//
// // Generate all code depending all 2 classes


// using Autofac;
// using Autofac.Extensions.DependencyInjection;
// using Microsoft.Extensions.DependencyInjection;
// using Microsoft.Extensions.Logging;
//
// var serviceCollection = new ServiceCollection();
// serviceCollection.AddLogging();
// var containerBuilder = new ContainerBuilder();
// containerBuilder.Populate(serviceCollection);
// containerBuilder.RegisterType<A>().AsSelf().SingleInstance();
//
// var container = containerBuilder.Build();
// var a = container.Resolve<A>();
// a.Print("Hello");
//
// public class A
// {
//     private readonly ILogger<A> _logger;
//     public A(ILogger<A> logger)
//     {
//         _logger = logger;
//     }
//     public void Print(string print)
//     {
//         Console.WriteLine(print);
//         _logger.Log(LogLevel.Debug, print);   
//     }
// }

