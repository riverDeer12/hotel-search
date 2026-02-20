using HotelSearch.API.Constants;
using HotelSearch.API.Database.Entities;
using HotelSearch.API.Database.Entities.Abstract;
using Microsoft.EntityFrameworkCore;

namespace HotelSearch.API.Database;

public class HotelSearchContext : DbContext
{
    
    public HotelSearchContext(DbContextOptions<HotelSearchContext> options)
        : base(options)
    {
    }
    
    public DbSet<Hotel> Hotels { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(Program).Assembly);

        base.OnModelCreating(modelBuilder);
    }
    
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
    {
        var entries = ChangeTracker
            .Entries()
            .Where(e => e is
            {
                Entity: BaseEntity, State: EntityState.Added or
                EntityState.Modified or
                EntityState.Deleted
            });

        foreach (var entry in entries)
        {
            var entity = (BaseEntity)entry.Entity;

            switch (entry.State)
            {
                case EntityState.Added:
                    entity.CreatedAt = DateTimeOffset.UtcNow;
                    break;
                case EntityState.Detached:
                case EntityState.Unchanged:
                case EntityState.Modified:
                case EntityState.Deleted:
                    break;
                default:
                    throw new Exception(ErrorCodes.SavingError);
            }

            entity.UpdatedAt = DateTimeOffset.UtcNow;
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}