using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TikTokClone.Domain.Entities;

namespace TikTokClone.Infrastructure.Data.Configurations
{
    public class EmailCodeConfiguration : IEntityTypeConfiguration<EmailCode>
    {
        public void Configure(EntityTypeBuilder<EmailCode> builder)
        {
            builder.ToTable("EmailVerifications");

            builder.HasIndex(e => e.Id);

            builder.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(e => e.Code)
                .IsRequired()
                .HasMaxLength(6)
                .IsFixedLength();

            builder.Property(e => e.Expiry)
                .IsRequired()
                .HasColumnType("datetime2");

            builder.Property(e => e.LastTimeGenerateCode)
                .IsRequired()
                .HasColumnType("datetime2");

            builder.HasIndex(e => e.Email)
                .IsUnique()
                .HasDatabaseName("IX_EmailVerifications_Email");
        }
    }
}
