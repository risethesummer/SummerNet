using System.Net;
using System.Runtime.CompilerServices;
using Realtime.Data;
using Realtime.Filters.Interfaces;
using Realtime.Transporters.Interfaces;
using Realtime.Utils.Buffers;
using Realtime.Utils.Extensions;
using Realtime.Utils.Factory;

namespace Realtime.Transporters.Impl;

public class PlayerAcceptor<TPlayerIndex, TAuthData, TPlayer> : IPlayerAcceptor<TPlayerIndex, TAuthData, TPlayer> 
    where TPlayer : PlayerData<TPlayerIndex, TAuthData>, new() 
    where TPlayerIndex : unmanaged
{
    private readonly IConnectionAcceptor _wrappedConnectionAcceptor;
    private readonly IPlayerAuthenticator<TPlayerIndex, TAuthData, TPlayer>? 
        _authenticator;
    private readonly IFactory<BufferPointer<byte>, PoolableWrapper<BufferPointer<byte>, UnmanagedMemoryManager<byte>>>
        _memoryManagerPool;
    public PlayerAcceptor(IConnectionAcceptor wrappedConnectionAcceptor, 
        IFactory<BufferPointer<byte>, PoolableWrapper<BufferPointer<byte>, UnmanagedMemoryManager<byte>>> memoryManagerPool, 
        IPlayerAuthenticator<TPlayerIndex, TAuthData, TPlayer>? authenticator)
    {
        _wrappedConnectionAcceptor = wrappedConnectionAcceptor;
        _memoryManagerPool = memoryManagerPool;
        _authenticator = authenticator;
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
                if (_authenticator is not null)
                    playerData = await _authenticator.Authenticate(playerData);
                if (playerData is null)
                    continue;
                yield return playerData;
            }
        }
    }
}