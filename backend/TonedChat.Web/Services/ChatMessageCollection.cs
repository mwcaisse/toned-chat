namespace TonedChat.Web.Services;

public class ChatMessageCollection
{
    private readonly List<string> _messages;

    public ChatMessageCollection()
    {
        _messages = new List<string>();
    }

    public void Add(string message)
    {
        _messages.Add(message);
    }

    public int LastMessageIndex()
    {
        return _messages.Count - 1;
    }

    public List<string> GetMessageSinceLast(int lastMessageIndex)
    {
        if (lastMessageIndex < 0)
        {
            lastMessageIndex = -1;
        }
        return _messages.Skip(lastMessageIndex + 1).ToList();
    }
    
}