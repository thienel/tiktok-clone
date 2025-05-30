using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TikTokClone.Domain.Entities;

namespace TikTokClone.Infrastructure.Data.Configurations
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("RefreshTokens");

            // Primary Key
            builder.HasKey(rt => rt.Id);

            // Token - Required, Unique, Max Length
            builder.Property(rt => rt.Token)
                .IsRequired()
                .HasMaxLength(256);

            // UserId - Required, Foreign Key
            builder.Property(rt => rt.UserId)
                .IsRequired()
                .HasMaxLength(450); // Standard ASP.NET Identity User Id length

            // DateTime Properties
            builder.Property(rt => rt.ExpiresAt)
                .IsRequired()
                .HasColumnType("datetime2");

            builder.Property(rt => rt.CreatedAt)
                .IsRequired()
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETUTCDATE()"); // Auto-set creation time

            // Nullable DateTime for RevokedAt
            builder.Property(rt => rt.RevokedAt)
                .IsRequired(false)
                .HasColumnType("datetime2");

            // Optional Properties
            builder.Property(rt => rt.ReplacedByToken)
                .IsRequired(false)
                .HasMaxLength(256);

            builder.Property(rt => rt.RevokedByIp)
                .IsRequired(false)
                .HasMaxLength(45); // IPv6 max length

            builder.Property(rt => rt.CreatedByIp)
                .IsRequired(false)
                .HasMaxLength(45);

            // Relationships
            builder.HasOne(rt => rt.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes for Performance
            builder.HasIndex(rt => rt.Token)
                .IsUnique()
                .HasDatabaseName("IX_RefreshTokens_Token");

            builder.HasIndex(rt => rt.UserId)
                .HasDatabaseName("IX_RefreshTokens_UserId");

            builder.HasIndex(rt => rt.ExpiresAt)
                .HasDatabaseName("IX_RefreshTokens_ExpiresAt");

            // Composite index for active tokens query optimization
            builder.HasIndex(rt => new { rt.UserId, rt.RevokedAt, rt.ExpiresAt })
                .HasDatabaseName("IX_RefreshTokens_Active");

            // Computed Properties - Ignore from EF mapping
            builder.Ignore(rt => rt.IsExpired);
            builder.Ignore(rt => rt.IsRevoked);
            builder.Ignore(rt => rt.IsActive);
        }
    }
}
