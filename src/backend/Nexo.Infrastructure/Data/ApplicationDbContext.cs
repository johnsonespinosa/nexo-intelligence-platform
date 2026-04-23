using Microsoft.EntityFrameworkCore;
using Nexo.Domain.Entities;

namespace Nexo.Infrastructure.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Document> Documents => Set<Document>();
    public DbSet<Query> Queries => Set<Query>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Email).HasMaxLength(255).IsRequired();
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.FullName).HasMaxLength(255);
            entity.Property(e => e.Role).HasConversion<int>();
            entity.Property(e => e.Tier).HasConversion<int>();
        });

        // Document configuration
        modelBuilder.Entity<Document>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.Status);
            entity.Property(e => e.Filename).HasMaxLength(255).IsRequired();
            entity.Property(e => e.S3Key).HasMaxLength(500).IsRequired();

            entity.HasOne(e => e.User)
                .WithMany(u => u.Documents)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Query configuration
        modelBuilder.Entity<Query>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.CreatedAt);
            entity.Property(e => e.Question).IsRequired();

            entity.HasOne(e => e.User)
                .WithMany(u => u.Queries)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }
}
