using TonedChat.Web.Data;
using TonedChat.Web.Data.Entities;
using TonedChat.Web.Models;

namespace TonedChat.Web.Services;

public class ChatMessageService
{

    private readonly TautDatabaseContext _db;
    
    public ChatMessageService(TautDatabaseContext db)
    {
        _db = db;
    }

    public List<ChatMessage> GetAll()
    {
        return _db.ChatMessages.OrderBy(x => x.Date).Select(ToViewModel).ToList();
    }

    public List<ChatMessage> GetAllForChannel(Guid channelId)
    {
        return _db.ChatMessages.Where(m => m.ChannelId == channelId)
            .OrderBy(m => m.Date)
            .Select(ToViewModel)
            .ToList();
    }

    public async Task<ChatMessage> AddMessage(ChatMessage message)
    {
        // TODO: do I really want to make two databae queries to validate a message before we send it?
        // ideally we can optimize this / send it without making a database query each time
        if (!_db.Channels.Any(c => c.Id == message.ChannelId))
        {
            throw new Exception($"Channel with id {message.ChannelId} does not exist!");
        }
        
        var m = new ChatMessageEntity()
        {
            ChannelId = message.ChannelId,
            Content = message.Content,
            Date = message.Date,
            UserName = message.UserName,
        };
        _db.ChatMessages.Add(m);
        await _db.SaveChangesAsync();
        return ToViewModel(m);
    }

    private static ChatMessage ToViewModel(ChatMessageEntity e)
    {
        return new ChatMessage()
        {
            Id = e.Id,
            ChannelId = e.ChannelId,
            Content = e.Content,
            Date = e.Date,
            UserName = e.UserName,
        };
    }
}