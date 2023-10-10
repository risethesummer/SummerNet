using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using Realtime.Controllers.Filters.Interfaces;
using Realtime.Data;
using Realtime.Networks;
using Realtime.Utils.Buffers;
using Realtime.Utils.Extensions;
using Realtime.Utils.Factory;

namespace Realtime.Controllers.Transporters.Interfaces;

public class PlayerAcceptor<TPlayerIndex, TAuthData, TPlayer> : IPlayerAcceptor<TPlayerIndex, TAuthData, TPlayer> 
    where TPlayer : PlayerData<TPlayerIndex, TAuthData>, new() 
    where TPlayerIndex : unmanaged, INetworkIndex
{
    private readonly IConnectionAcceptor _wrappedConnectionAcceptor;
    private readonly IPlayerAuthenticator<TPlayerIndex, TAuthData, TPlayer> _authenticator;
    private readonly IFactory<BufferPointer<byte>, PoolableWrapper<BufferPointer<byte>, UnmanagedMemoryManager<byte>>>
        _memoryManagerPool;
    public PlayerAcceptor(IConnectionAcceptor wrappedConnectionAcceptor, 
        IFactory<BufferPointer<byte>, PoolableWrapper<BufferPointer<byte>, UnmanagedMemoryManager<byte>>> memoryManagerPool, 
        IPlayerAuthenticator<TPlayerIndex, TAuthData, TPlayer> authenticator)
    {
        _wrappedConnectionAcceptor = wrappedConnectionAcceptor;
        _memoryManagerPool = memoryManagerPool;
        _authenticator = authenticator;
    }
    public void Dispose()
    {
        _wrappedConnectionAcceptor.Dispose();
    }

    public async IAsyncEnumerable<TPlayer> BeginAccepting([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await foreach (var socket in _wrappedConnectionAcceptor.BeginAccepting(cancellationToken))
        {
            var authData = await socket.GetRawMessageAsync<TAuthData>(_memoryManagerPool, cancellationToken);
            if (authData is not null && socket.EndPoint is IPEndPoint remoteIpEndPoint)
            {
                var playerData = new TPlayer
                {
                    Address = remoteIpEndPoint.Address,
                    JoinedTime = DateTime.Now,
                    Status = PlayerStatus.Setup,
                    AuthData = authData
                };
                playerData = await _authenticator.Authenticate(playerData);
                if (playerData is null)
                    continue;
                yield return playerData;
            }
        }
    }
}

public class ConnectionAcceptor : IConnectionAcceptor
{
    private readonly ISocket _acceptSocket;
    public ConnectionAcceptor(ISocket acceptSocket)
    {
        _acceptSocket = acceptSocket;
    }
    public void Dispose()
    {
        _acceptSocket.Dispose();
        GC.SuppressFinalize(this);
    }
    public async IAsyncEnumerable<ISocket> BeginAccepting([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var accept = await _acceptSocket.AcceptAsync(cancellationToken).ConfigureAwait(false);
            yield return accept;
        }
    }
}