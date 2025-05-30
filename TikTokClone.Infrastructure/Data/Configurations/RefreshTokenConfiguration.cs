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
            builder.HasKey(rf => rf.Id);

            builder.Property(rf => rf.Token)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(rf => rf.UserId)
                .IsRequired();

            builder.Property(rf => rf.ExpiresAt)
                .IsRequired()
                .HasColumnType("datetime2");

            builder.Property(rf => rf.CreatedAt)
                .IsRequired()
                .HasColumnType("datetime2");

            builder.HasOne(rf => rf.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(rf => rf.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(rf => rf.Token).IsUnique();
        }
    }
}
