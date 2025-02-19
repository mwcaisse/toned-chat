using NodaTime;

namespace TonedChat.Web.Models;

public class ChatMessage
{   
    public Guid Id { get; set; }
    
    public Guid ChannelId { get; set; }
    
    public string UserName { get; set; }
    
    public string Content { get; set; }
    
    public Instant Date { get; set; }
}
