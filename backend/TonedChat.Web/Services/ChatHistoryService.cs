using TonedChat.Web.Models;

namespace TonedChat.Web.Services;

// Keeps track of historical chat messages. Messages that have been sent since the server is up
public class ChatHistoryService
{

    private List<ChatMessage> _messages;

    public ChatHistoryService()
    {
        _messages = new List<ChatMessage>();
    }

    public List<ChatMessage> GetHistoricalMessages()
    {
        return _messages;
    }

    public void AddMessage(ChatMessage message)
    {
        _messages.Add(message);
    }

}