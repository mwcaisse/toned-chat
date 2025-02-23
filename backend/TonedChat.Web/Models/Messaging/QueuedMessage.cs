namespace TonedChat.Web.Models.Messaging;

public class QueuedMessage
{
    public required Message Message { get; init; }

    public ISet<string> IncludedClients { get; init ; } = new HashSet<string>();
    
    public ISet<string> ExcludedClients { get; init; } = new HashSet<string>();
}