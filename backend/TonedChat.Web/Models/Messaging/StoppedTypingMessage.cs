namespace TonedChat.Web.Models.Messaging;

public class StoppedTypingMessage : MessageWithPayload<TypingIndicator>
{
    public const string TYPE = "STOPPED_TYPING";

    public override string Type => TYPE;
}