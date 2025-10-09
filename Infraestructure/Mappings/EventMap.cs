using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNI_Events.Domain.Entities;

namespace SNI_Events.Infraestructure.Mappings
{
    public class EventMap : IEntityTypeConfiguration<Event>
    {
        public void Configure(EntityTypeBuilder<Event> builder)
        {
            builder.ToTable("Event");

            builder.HasKey(u => u.Id);


            builder.HasMany(e => e.ScheduledEvents)
                   .WithOne(se => se.Event)
                   .HasForeignKey(se => se.EventId)
                   .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
