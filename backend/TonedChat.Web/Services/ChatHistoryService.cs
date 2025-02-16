namespace TonedChat.Web.Services;

// Keeps track of historical chat messages. Messages that have been sent since the server is up
public class ChatHistoryService
{

    private List<byte[]> _messages;

    public ChatHistoryService()
    {
        _messages = new List<byte[]>();
    }

    public List<byte[]> GetHistoricalMessages()
    {
        return _messages;
    }

    public void AddMessage(byte[] message)
    {
        _messages.Add(message);
    }

}