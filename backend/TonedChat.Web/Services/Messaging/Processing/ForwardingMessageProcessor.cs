using TonedChat.Web.Models.Messaging;

namespace TonedChat.Web.Services.Messaging.Processing;

public class ForwardingMessageProcessor<T> : TypedMessageProcessor<T> where T : Message
{
    private readonly MessageQueue _messageQueue;

    public ForwardingMessageProcessor(MessageQueue messageQueue)
    {
        _messageQueue = messageQueue;
    }

    protected override async Task ProcessTypedMessage(T message, MessageMetadata metadata, CancellationToken cancellationToken = default)
    {
        await _messageQueue.AddMessage(message, cancellationToken, excludedClients: new HashSet<string>([metadata.SenderClientId]));
    }
}
