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
        return _db.ChatMessages.OrderBy(x => x.Date).Select(m => new ChatMessage()
        {
            Id = m.Id,
            Content = m.Content,
            Date = m.Date,
            UserName = m.UserName,
        }).ToList();
    }

    public async Task AddMessage(ChatMessage message)
    {
        var m = new ChatMessageEntity()
        {
            Content = message.Content,
            Date = message.Date,
            UserName = message.UserName,
        };
        _db.ChatMessages.Add(m);
        await _db.SaveChangesAsync();
    }
}