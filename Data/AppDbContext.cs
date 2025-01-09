using BlazorApp.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace BlazorApp.Data
{
  public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
  {
    /// <summary>
    /// Represents the Users table.
    /// </summary>
    public DbSet<User> Users { get; set; }

    /// <summary>
    /// Represents the Passkeys table.
    /// </summary>
    public DbSet<Passkey> Passkeys { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      // Define the relationship between User and Passkey.
      modelBuilder.Entity<Passkey>()
        .HasOne<User>()
        .WithMany()
        .HasForeignKey(p => p.UserId)
        .OnDelete(DeleteBehavior.Cascade);
    }
  }
}