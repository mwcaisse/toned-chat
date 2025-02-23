using TonedChat.Web.Models.Messaging;

namespace TonedChat.Web.Services.Messaging.Processing;

public abstract class TypedMessageProcessor<T> : IMessageProcessor where T : Message
{
    public Task ProcessMessage(Message message, MessageMetadata metadata, CancellationToken cancellationToken = default)
    {
        if (message is T typedMessage)
        {
            return ProcessTypedMessage(typedMessage, metadata, cancellationToken);
        }

        throw new Exception($"Invalid message passed to message processor ({GetType()}) expecting type {typeof(T)} received {message.GetType()}");
    }

    protected abstract Task ProcessTypedMessage(T message, MessageMetadata metadata, CancellationToken cancellationToken = default);
}
