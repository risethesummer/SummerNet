using System.Net;
using System.Net.Sockets;

namespace Realtime.Controllers.Transporters.Interfaces;

public class RawSocketAdapter : ISocket
{
    private readonly Socket _socket;
    public RawSocketAdapter(Socket socket)
    {
        _socket = socket;
    }
    public static RawSocketAdapter CreateTcpListener(IPEndPoint endPoint, int maxListen)
    {
        using Socket listener = new(
            endPoint.AddressFamily, 
            SocketType.Stream, 
            ProtocolType.Tcp);
        listener.Bind(endPoint);
        listener.Listen(maxListen);
        return new RawSocketAdapter(listener);
    }
    public async ValueTask<ISocket> AcceptAsync(CancellationToken cancellationToken)
    {
        var accepted = await _socket.AcceptAsync(cancellationToken);
        return new RawSocketAdapter(accepted);
    }
    public ValueTask<int> ReceiveAsync(Memory<byte> buffer, CancellationToken cancellationToken)
        => _socket.ReceiveAsync(buffer, cancellationToken);
    public ValueTask<int> SendAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken)
        => _socket.SendAsync(buffer, cancellationToken);
    public ValueTask DisconnectAsync(bool reuse, CancellationToken cancellationToken) =>
        _socket.DisconnectAsync(reuse, cancellationToken);

    public EndPoint? EndPoint => _socket.RemoteEndPoint;
}

public interface ISocket : IDisposable
{
    ValueTask<ISocket> AcceptAsync(CancellationToken cancellationToken);
    ValueTask<int> ReceiveAsync(Memory<byte> buffer, CancellationToken cancellationToken);
    ValueTask<int> SendAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken);
    ValueTask DisconnectAsync(bool reuse, CancellationToken cancellationToken);
    EndPoint? EndPoint { get; }
}