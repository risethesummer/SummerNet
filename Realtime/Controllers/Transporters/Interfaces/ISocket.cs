using System.Net;

namespace Realtime.Controllers.Transporters.Interfaces;

public interface ISocket : IDisposable
{
    ValueTask<ISocket> AcceptAsync(CancellationToken cancellationToken);
    ValueTask<int> ReceiveAsync(Memory<byte> buffer, CancellationToken cancellationToken);
    ValueTask<int> SendAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken);
    ValueTask DisconnectAsync(bool reuse, CancellationToken cancellationToken);
    EndPoint? EndPoint { get; }
}