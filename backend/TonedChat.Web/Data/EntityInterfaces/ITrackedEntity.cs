using NodaTime;

namespace TonedChat.Web.Data.EntityInterfaces;

public interface ITrackedEntity
{
    Instant CreateDate { get; set; }
    
    Instant UpdateDate { get; set; }
}