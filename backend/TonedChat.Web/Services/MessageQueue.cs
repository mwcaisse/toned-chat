using System.Threading.Channels;
using TonedChat.Web.Models.Messaging;

namespace TonedChat.Web.Services;

public class MessageQueue
{

    private readonly Channel<Message> _messageChannel;
    
    public MessageQueue()
    {
        _messageChannel = Channel.CreateUnbounded<Message>(new UnboundedChannelOptions()
        {
            SingleReader = true,
            SingleWriter = false,
        });
    }

    public ValueTask AddMessage(Message message, CancellationToken cancellationToken)
    {
        return _messageChannel.Writer.WriteAsync(message, cancellationToken);
    }

    public ValueTask<bool> WaitForMessage(CancellationToken cancellationToken)
    {
        return _messageChannel.Reader.WaitToReadAsync(cancellationToken);
    }

    public bool ReadMessage(out Message? message)
    {
        return _messageChannel.Reader.TryRead(out message);
    }
    
}