using Microsoft.EntityFrameworkCore;
using NodaTime;
using TonedChat.Web.Data.Entities;
using TonedChat.Web.Data.EntityInterfaces;

namespace TonedChat.Web.Data;

public class TautDatabaseContext : DbContext
{
    public DbSet<ChatMessageEntity> ChatMessages { get; set; }

    private readonly IClock _clock;

    public TautDatabaseContext(DbContextOptions<TautDatabaseContext> contextOptions, IClock clock) : base(contextOptions)
    {
        _clock = clock;
    }

    public override int SaveChanges()
    {
        var now = _clock.GetCurrentInstant();

        foreach (var changedEntity in ChangeTracker.Entries())
        {
            if (changedEntity.Entity is ITrackedEntity trackedEntity)
            {
                switch (changedEntity.State)
                {
                    case EntityState.Added:
                        trackedEntity.CreateDate = now;
                        trackedEntity.UpdateDate = now;
                        break;
                    case EntityState.Modified:
                        Entry(trackedEntity).Property(x => x.CreateDate).IsModified = false;
                        trackedEntity.UpdateDate = now;
                        break;
                }
            }
        }

        return base.SaveChanges();
    }
}