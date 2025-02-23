using TonedChat.Web.Models.Messaging;

namespace TonedChat.Web.Services.Messaging.Processing;

public class CreateChannelMessageProcessor : TypedMessageProcessor<CreateChannelMessage>
{
    private readonly ChatChannelService _channelService;

    private readonly MessageQueue _messageQueue;
    
    public CreateChannelMessageProcessor(ChatChannelService channelService, MessageQueue messageQueue)
    {
        _channelService = channelService;
        _messageQueue = messageQueue;
    }

    protected override async Task ProcessTypedMessage(CreateChannelMessage message, CancellationToken cancellationToken = default)
    {
        var createdChannel = await _channelService.Create(message.Payload);

        var notifyChannelMessage = new ChannelCreatedMessage()
        {
            Payload = createdChannel
        };
        await _messageQueue.AddMessage(notifyChannelMessage, cancellationToken);
    }
}