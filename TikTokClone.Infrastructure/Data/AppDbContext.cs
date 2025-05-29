using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TikTokClone.Domain.Entities;

namespace TikTokClone.Infrastructure.Data
{
    public class AppDbContext : IdentityDbContext<User>
    {
#pragma warning disable CS0114
        public DbSet<User> Users { get; set; }
#pragma warning restore CS0114

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>(entity =>
            {
                entity.ToTable("Users");

                entity.HasIndex(u => u.UserName).IsUnique();
                entity.HasIndex(u => u.Email).IsUnique();

                entity.Ignore(u => u.DomainEvents);

                entity.Property(u => u.Name)
                    .IsRequired()
                    .HasMaxLength(User.MaxNameLength);

                entity.Property(u => u.UserName)
                    .IsRequired()
                    .HasMaxLength(24);

                entity.Property(u => u.Email)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(u => u.Bio)
                    .HasMaxLength(User.MaxBioLength)
                    .IsRequired(false);

                entity.Property(u => u.AvatarURL)
                    .HasMaxLength(2048)
                    .IsRequired(false);

                entity.Property(u => u.BirthDate)
                    .IsRequired()
                    .HasColumnType("date");

                entity.Property(u => u.CreatedAt)
                    .IsRequired()
                    .HasColumnType("datetime2");

                entity.Property(u => u.LastUpdatedAt)
                    .IsRequired()
                    .HasColumnType("datetime2");

                entity.Property(u => u.LastLoginAt)
                    .IsRequired(false)
                    .HasColumnType("datetime2");

                entity.Property(u => u.IsVerified)
                    .IsRequired();
            });

            builder.Entity<RefreshToken>(entity =>
            {
                entity.ToTable("RefreshTokens");
                entity.HasKey(rf => rf.Id);

                entity.Property(rf => rf.Token)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(rf => rf.UserId)
                    .IsRequired();

                entity.Property(rf => rf.ExpiresAt)
                    .IsRequired()
                    .HasColumnType("datetime2");

                entity.Property(rf => rf.CreatedAt)
                    .IsRequired()
                    .HasColumnType("datetime2");

                entity.HasOne(rf => rf.User)
                    .WithMany()
                    .HasForeignKey(rf => rf.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(rf => rf.Token).IsUnique();
            });
        }
    }
}
