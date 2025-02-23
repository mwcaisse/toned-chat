using System.Threading.Channels;
using TonedChat.Web.Models.Messaging;

namespace TonedChat.Web.Services;

public class MessageQueue
{

    private readonly Channel<QueuedMessage> _messageChannel;
    
    public MessageQueue()
    {
        _messageChannel = Channel.CreateUnbounded<QueuedMessage>(new UnboundedChannelOptions()
        {
            SingleReader = true,
            SingleWriter = false,
        });
    }

    public ValueTask AddMessage(Message message, CancellationToken cancellationToken, ISet<string>? includedClients = null, ISet<string>? excludedClients = null)
    {
        var queuedMessage = new QueuedMessage()
        {
            Message = message,
            IncludedClients = includedClients ?? new HashSet<string>(),
            ExcludedClients = excludedClients ?? new HashSet<string>()
        };
        
        return _messageChannel.Writer.WriteAsync(queuedMessage, cancellationToken);
    }

    public ValueTask<bool> WaitForMessage(CancellationToken cancellationToken)
    {
        return _messageChannel.Reader.WaitToReadAsync(cancellationToken);
    }

    public bool ReadMessage(out QueuedMessage? message)
    {
        return _messageChannel.Reader.TryRead(out message);
    }
    
}