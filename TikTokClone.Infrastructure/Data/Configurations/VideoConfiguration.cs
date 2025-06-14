using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TikTokClone.Domain.Entities;

namespace TikTokClone.Infrastructure.Data.Configurations
{
    public class VideoConfiguration : IEntityTypeConfiguration<Video>
    {
        public void Configure(EntityTypeBuilder<Video> builder)
        {
            builder.ToTable("Videos");

            builder.HasIndex(v => v.Id);

            builder.Property(v => v.Title)
                .IsRequired(false)
                .HasMaxLength(Video.MaxTitleLength);

            builder.Property(v => v.Description)
                .IsRequired(false)
                .HasMaxLength(Video.MaxDescriptionLength);

            builder.Property(v => v.IsVisible)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(v => v.Url)
                .IsRequired()
                .HasMaxLength(2048);

            builder.Property(v => v.ThumbnailUrl)
                .IsRequired()
                .HasMaxLength(2048);

            builder.Property(v => v.UserId)
                .IsRequired();

            builder.Property(v => v.CreatedAt)
                .IsRequired()
                .HasColumnType("datetime2");

            builder.Property(v => v.UpdatedAt)
                .IsRequired()
                .HasColumnType("datetime2");

            builder.HasOne(v => v.User)
                .WithMany(u => u.Videos)
                .HasForeignKey(v => v.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
