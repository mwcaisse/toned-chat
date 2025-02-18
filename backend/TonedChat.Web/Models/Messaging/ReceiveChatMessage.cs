namespace TonedChat.Web.Models.Messaging;

public class ReceiveChatMessage : MessageWithPayload<ChatMessage>
{
    public const string TYPE = "RECEIVE_CHAT_MESSAGE";

    public override string Type => TYPE;
}