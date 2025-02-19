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
        var m = new ChatMessageEntity()
        {
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
            Content = e.Content,
            Date = e.Date,
            UserName = e.UserName,
        };
    }
}