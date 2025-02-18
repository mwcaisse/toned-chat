namespace TonedChat.Web.Models.Messaging;

public class ChannelCreatedMessage : MessageWithPayload<ChatChannel>
{
    public const string TYPE = "CHANNEL_CREATED";

    public override string Type => TYPE;
    
}