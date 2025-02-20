namespace TonedChat.Web.Models.Messaging;

public class StartedTypingMessage : MessageWithPayload<TypingIndicator>
{
    public const string TYPE = "STARTED_TYPING";

    public override string Type => TYPE;
}