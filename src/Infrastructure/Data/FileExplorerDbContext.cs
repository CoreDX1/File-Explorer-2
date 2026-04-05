using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class FileExplorerDbContext : DbContext
{
    public FileExplorerDbContext(DbContextOptions<FileExplorerDbContext> options)
        : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<FileItem> FileItems { get; set; }
    public DbSet<FolderItem> FolderItems { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<FileSystemItem> FileSystemItems { get; set; }

    // Fluent
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Ignore(e => e.FullName);
            entity.Property(e => e.StorageQuotaBytes).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
            entity.Property(e => e.IsActive).IsRequired();
            entity.Property(e => e.LastLoginAt);

            entity.HasIndex(e => e.Email).IsUnique();
        });

        modelBuilder.Entity<FileSystemItem>(entity =>
        {
            entity.ToTable("FileSystemItems");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Name).IsRequired().HasMaxLength(250);
            entity.Property(e => e.Path).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Size).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.ModifiedAt).IsRequired();
            entity.Property(e => e.IsDirectory).IsRequired();
            entity.Property(e => e.ItemType).IsRequired();

            entity.Property(e => e.ParentFolderId).IsRequired(false);

            entity.HasIndex(e => e.ParentFolderId);

            // TPH Inheritance with discriminator
            entity
                .HasDiscriminator<FileSystemItemType>("ItemType")
                .HasValue<FileItem>(FileSystemItemType.File)
                .HasValue<FolderItem>(FileSystemItemType.Directory);
        });

        modelBuilder.Entity<FileItem>(entity =>
        {
            entity.Property(e => e.StorageFileName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.ContentType).HasMaxLength(100);
        });

        modelBuilder.Entity<FolderItem>(entity =>
        {
            entity
                .HasMany(e => e.Children)
                .WithOne(e => e.ParentFolder)
                .HasForeignKey(e => e.ParentFolderId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Token).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Expire).IsRequired();
            entity.Property(e => e.Created).IsRequired();

            entity
                .HasOne(rt => rt.User)
                .WithMany()
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.Token).IsUnique();
            entity.HasIndex(e => e.UserId);
        });
    }
}
