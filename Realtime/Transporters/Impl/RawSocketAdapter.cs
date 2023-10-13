using System.Net;
using System.Net.Sockets;
using Realtime.Transporters.Interfaces;

namespace Realtime.Transporters.Impl;

public class RawSocketAdapter : ISocket
{
    private readonly Socket _socket;
    public RawSocketAdapter(Socket socket)
    {
        _socket = socket;
    }
    public ValueTask<int> ReceiveAsync(Memory<byte> buffer, CancellationToken cancellationToken)
        => _socket.ReceiveAsync(buffer, cancellationToken);
    public ValueTask<int> SendAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken)
        => _socket.SendAsync(buffer, cancellationToken);
    public ValueTask DisconnectAsync(bool reuse, CancellationToken cancellationToken) =>
        _socket.DisconnectAsync(reuse, cancellationToken);
    public EndPoint? EndPoint => _socket.RemoteEndPoint;
    public void Dispose()
    {
        _socket.Dispose();
        GC.SuppressFinalize(this);
    }
}