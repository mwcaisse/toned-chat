namespace TonedChat.Web.Models;

public class TypingIndicator
{
    public Guid ChannelId { get; set; }
    
    public string User { get; set; }
}