using TonedChat.Web.Data;
using TonedChat.Web.Data.Entities;
using TonedChat.Web.Models;

namespace TonedChat.Web.Services;

public class ChatChannelService
{
    private readonly TautDatabaseContext _db;

    public ChatChannelService(TautDatabaseContext db)
    {
        _db = db;
    }

    public List<ChatChannel> GetAll()
    {
        return _db.Channels.OrderBy(c => c.Name).Select(ToViewModel).ToList();
    }
    
    public async Task<ChatChannel> Create(ChatChannel channel)
    {
        // if a channel with this name already exists, don't create it
        // TODO: We should disallow re-creating the same channel with different case
        var existingChannel = _db.Channels.FirstOrDefault(c => c.Name == channel.Name);
        if (existingChannel != null)
        {
            return ToViewModel(existingChannel);
        }
        
        var e = new ChannelEntity()
        {
            Name = channel.Name,
        };
        _db.Channels.Add(e);
        await _db.SaveChangesAsync();
        return ToViewModel(e);
    }

    private static ChatChannel ToViewModel(ChannelEntity e)
    {
        return new ChatChannel()
        {
            Id = Guid.Parse(e.Id),
            Name = e.Name
        };
    }
}