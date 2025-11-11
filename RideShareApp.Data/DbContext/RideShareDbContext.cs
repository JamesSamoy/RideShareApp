using Microsoft.EntityFrameworkCore;
using RideShareApp.Data.Entities;

namespace RideShareApp.Data.DbContext;

public class RideShareDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public RideShareDbContext(DbContextOptions<RideShareDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.HasIndex(e => e.Email).IsUnique();
        });
    }
}

