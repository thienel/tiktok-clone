
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using TikTokClone.Domain.Entities;

namespace TikTokClone.Infrastructure.Data
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public DbSet<User> Users { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>(entity =>
            {
                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Bio)
                    .HasMaxLength(500);

                entity.Property(e => e.AvatarURL)
                .HasMaxLength(2048);

                entity.HasIndex(e => e.UserName)
                .IsUnique();

                entity.HasIndex(e => e.Email)
                .IsUnique();

                entity.Ignore(e => e.DomainEvents);
            });

            builder.Entity<User>().ToTable("Users");
        }
    }
}
