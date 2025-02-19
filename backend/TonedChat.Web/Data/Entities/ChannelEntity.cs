using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NodaTime;
using TonedChat.Web.Data.EntityInterfaces;

namespace TonedChat.Web.Data.Entities;

[Table("channel")]
public class ChannelEntity : ITrackedEntity
{
    public ChannelEntity()
    {
        Id = Guid.NewGuid();
    }
    
    [Key]
    public Guid Id { get; set; }
    
    [StringLength(100)]
    public string Name { get; set; }
    
    public Instant CreateDate { get; set; }
    
    public Instant UpdateDate { get; set; }
}