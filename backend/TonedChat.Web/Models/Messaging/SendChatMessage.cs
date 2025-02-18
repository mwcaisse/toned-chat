namespace TonedChat.Web.Models.Messaging;

public class SendChatMessage : MessageWithPayload<ChatMessage>
{
    public const string TYPE = "SEND_CHAT_MESSAGE";

    public override string Type => TYPE;
}