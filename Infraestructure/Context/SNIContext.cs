using Microsoft.EntityFrameworkCore;
using SNI_Events.Domain.Entities;
using SNI_Events.Infraestructure.Mappings;
using SNI_Events.Domain.Entities.Base;
using SNI_Events.Domain.Interfaces.Services;
using SNI_Events.Domain.Enum;

namespace SNI_Events.Infraestructure.Context
{
    public class SNIContext : DbContext
    {
        private readonly ICurrentUserService _currentUserService;

        public SNIContext(DbContextOptions<SNIContext> options, ICurrentUserService currentUserService) : base(options)
        {
            _currentUserService = currentUserService;
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Dinner> Dinners { get; set; }
        public DbSet<UserDinner> UserDinners { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserMap());
            modelBuilder.ApplyConfiguration(new EventMap());
            modelBuilder.ApplyConfiguration(new UserDinnerMap());

            base.OnModelCreating(modelBuilder);
        }

        public override int SaveChanges()
        {
            ApplyAudit();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ApplyAudit();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void ApplyAudit()
        {
            if (_currentUserService == null) return;

            var entries = ChangeTracker.Entries<EntityBase>();

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.SetCreationAudit(_currentUserService.UserId);
                }
                else if (entry.State == EntityState.Modified)
                {
                    // If entity was marked as Deleted in domain, set deletion audit
                    if (entry.Entity.Status == EStatus.Deleted)
                    {
                        entry.Entity.SetDeletionAudit(_currentUserService.UserId);
                    }
                    else
                    {
                        entry.Entity.SetModificationAudit(_currentUserService.UserId);
                    }
                }
            }
        }
    }
}
