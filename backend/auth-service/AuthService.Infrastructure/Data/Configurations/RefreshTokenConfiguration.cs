using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AuthService.Domain.Entities;

namespace AuthService.Infrastructure.Data.Configurations
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("RefreshTokens");

            builder.HasKey(rt => rt.Id);

            builder.Property(rt => rt.Token)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(rt => rt.UserId)
                .IsRequired()
                .HasMaxLength(450);

            builder.Property(rt => rt.ExpiresAt)
                .IsRequired()
                .HasColumnType("datetime2");

            builder.Property(rt => rt.CreatedAt)
                .IsRequired()
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(rt => rt.RevokedAt)
                .IsRequired(false)
                .HasColumnType("datetime2");

            builder.Property(rt => rt.ReplacedByToken)
                .IsRequired(false)
                .HasMaxLength(256);

            builder.HasOne(rt => rt.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(rt => rt.Token)
                .IsUnique()
                .HasDatabaseName("IX_RefreshTokens_Token");

            builder.HasIndex(rt => rt.UserId)
                .HasDatabaseName("IX_RefreshTokens_UserId");

            builder.HasIndex(rt => rt.ExpiresAt)
                .HasDatabaseName("IX_RefreshTokens_ExpiresAt");

            builder.HasIndex(rt => new { rt.UserId, rt.RevokedAt, rt.ExpiresAt })
                .HasDatabaseName("IX_RefreshTokens_Active");

            builder.Ignore(rt => rt.IsExpired);
            builder.Ignore(rt => rt.IsRevoked);
            builder.Ignore(rt => rt.IsActive);
        }
    }
}
