namespace TonedChat.Web.Models.Messaging;

public class CreateChannelMessage : MessageWithPayload<ChatChannel>
{
    public const string TYPE = "CREATE_CHANNEL";

    public override string Type => TYPE;
}