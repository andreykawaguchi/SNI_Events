using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNI_Events.Domain.Entities;
using System.Reflection.Emit;

namespace SNI_Events.Infraestructure.Mappings
{
    public class ScheduledEventMap : IEntityTypeConfiguration<ScheduledEvent>
    {
        public void Configure(EntityTypeBuilder<ScheduledEvent> builder)
        {
            builder.ToTable("ScheduledEvent");

            builder.HasKey(u => u.Id);


            builder
                .HasOne(e => e.Event)
                .WithMany(ev => ev.ScheduledEvents)
                .HasForeignKey(e => e.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasOne(se => se.Dinner)
                .WithMany(d => d.ScheduledEvents)
                .HasForeignKey(se => se.DinnerId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
