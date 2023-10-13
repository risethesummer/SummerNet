using Realtime.Transporters.Messages;

namespace Realtime.Transporters.Interfaces;

public interface IMessageDistributor<TPlayerIndex>
{
    /// <summary>
    /// Send a message to the message buffer
    /// The message will not be sent promptly but going out in the end of a tick
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    ValueTask SendMessageInline<T>(in SentNetworkMessage<TPlayerIndex, T> msg,
        CancellationToken cancellationToken);

    /// <summary>
    /// Send a message to the client spontaneously
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    ValueTask SendMessage<T>(in SentNetworkMessage<TPlayerIndex, T> msg,
        CancellationToken cancellationToken);
    ValueTask FlushSentMessage(CancellationToken cancellationToken);
}
