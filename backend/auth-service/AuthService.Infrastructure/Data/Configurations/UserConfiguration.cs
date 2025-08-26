using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AuthService.Domain.Entities;

namespace AuthService.Infrastructure.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasIndex(u => u.UserName).IsUnique();
            builder.HasIndex(u => u.Email).IsUnique();

            builder.Ignore(u => u.DomainEvents);

            builder.Property(u => u.Name)
                .IsRequired()
                .HasMaxLength(User.MaxNameLength);

            builder.Property(u => u.UserName)
                .IsRequired()
                .HasMaxLength(24);

            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(u => u.Bio)
                .HasMaxLength(User.MaxBioLength)
                .IsRequired(false);

            builder.Property(u => u.AvatarURL)
                .HasMaxLength(2048)
                .IsRequired(false);

            builder.Property(u => u.BirthDate)
                .IsRequired()
                .HasColumnType("date");

            builder.Property(u => u.CreatedAt)
                .IsRequired()
                .HasColumnType("datetime2");

            builder.Property(u => u.LastUpdatedAt)
                .IsRequired()
                .HasColumnType("datetime2");

            builder.Property(u => u.LastLoginAt)
                .IsRequired(false)
                .HasColumnType("datetime2");

            builder.Property(u => u.IsVerified)
                .IsRequired();
        }
    }
}
