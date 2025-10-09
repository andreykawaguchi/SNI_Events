using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNI_Events.Domain.Entities;

namespace SNI_Events.Infraestructure.Mappings
{
    public class UserDinnerMap : IEntityTypeConfiguration<UserDinner>
    {
        public void Configure(EntityTypeBuilder<UserDinner> builder)
        {
            builder.ToTable("UserDinners");

            builder.HasKey(ud => ud.Id);

            builder.Property(ud => ud.IsPresent).IsRequired();

            builder.Property(ud => ud.Status).IsRequired();

            builder.HasOne(ud => ud.User)
                   .WithMany(u => u.UserDinners)
                   .HasForeignKey(ud => ud.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ud => ud.Dinner)
                   .WithMany(d => d.UserDinners)
                   .HasForeignKey(ud => ud.DinnerId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
