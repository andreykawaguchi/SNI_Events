using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNI_Events.Domain.Entities;

namespace SNI_Events.Infraestructure.Mappings
{
    public class UserMap : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(u => u.Id);

            builder.Property(u => u.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            // Value Object: Email
            builder.Property(u => u.Email)
                   .HasConversion(
                       e => e.Address,
                       e => new SNI_Events.Domain.ValueObjects.Email(e))
                   .IsRequired()
                   .HasMaxLength(150);

            // Value Object: Password
            builder.Property(u => u.Password)
                   .HasConversion(
                       p => p.Hash,
                       p => new SNI_Events.Domain.ValueObjects.Password(p))
                   .IsRequired();

            // Value Object: PhoneNumber
            builder.Property(u => u.PhoneNumber)
                   .HasConversion(
                       pn => pn.Number,
                       pn => new SNI_Events.Domain.ValueObjects.PhoneNumber(pn))
                   .HasMaxLength(20);

            // Value Object: Cpf
            builder.Property(u => u.Cpf)
                   .HasConversion(
                       c => c.Number,
                       c => new SNI_Events.Domain.ValueObjects.Cpf(c))
                   .HasMaxLength(14);

            builder.Property(u => u.Role)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.HasMany(u => u.UserDinners)
                   .WithOne(ud => ud.User)
                   .HasForeignKey(ud => ud.UserId);
        }
    }
}
