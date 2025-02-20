using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NodaTime;
using TonedChat.Web.Data.EntityInterfaces;

namespace TonedChat.Web.Data.Entities;

[Table("chat_message")]
public class ChatMessageEntity : ITrackedEntity
{
    public ChatMessageEntity()
    {
        Id = Guid.NewGuid().ToString();
    }
    
    [Key]
    public string Id { get; set; }
    
    [StringLength(100)]
    public string UserName { get; set; }
    
    public string Content { get; set; }
    
    [ForeignKey("Channel")]
    public string ChannelId { get; set; }
    
    public Instant Date { get; set; }

    public Instant CreateDate { get; set; }
    
    public Instant UpdateDate { get; set; }
    
    public virtual ChannelEntity Channel { get; set; }
}