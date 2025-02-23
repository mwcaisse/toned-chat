using TonedChat.Web.Models.Messaging;

namespace TonedChat.Web.Services.Messaging.Processing;

public class SendChatMessageProcessor : TypedMessageProcessor<SendChatMessage>
{
    private readonly ChatMessageService _chatMessageService;

    private readonly MessageQueue _messageQueue;

    public SendChatMessageProcessor(ChatMessageService chatMessageService, MessageQueue messageQueue)
    {
        _chatMessageService = chatMessageService;
        _messageQueue = messageQueue;
    }

    protected override async Task ProcessTypedMessage(SendChatMessage message, MessageMetadata metadata, CancellationToken cancellationToken = default)
    {
        var createdChatMessage = await _chatMessageService.AddMessage(message.Payload);

        var notifyMessage = new ReceiveChatMessage()
        {
            Id = Guid.NewGuid(),
            Payload = createdChatMessage
        };
        await _messageQueue.AddMessage(notifyMessage, cancellationToken);
    }
}