using Microsoft.EntityFrameworkCore;
using ShelfLife.Shared.Models;

namespace ShelfLife.Api.Data;

public class ShelfContext : DbContext
{
    public ShelfContext(DbContextOptions<ShelfContext> options) : base(options) { }

    public DbSet<Book> Books => Set<Book>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Author).IsRequired().HasMaxLength(300);
            entity.Property(e => e.Isbn).HasMaxLength(20);
            entity.Property(e => e.Genre).HasMaxLength(100);
            entity.Property(e => e.Status).HasConversion<string>();
            entity.Property(e => e.CoverUrl).HasMaxLength(500);
        });
    }
}
