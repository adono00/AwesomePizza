using AwesomePizza.Repository.Models;
using Microsoft.EntityFrameworkCore;

namespace AwesomePizza.Repository.Data;

public class PizzaDbContext : DbContext
{
    public PizzaDbContext(DbContextOptions<PizzaDbContext> options) : base(options) { }

    public DbSet<Order> Orders => Set<Order>();
    public DbSet<LogEntry> Logs => Set<LogEntry>();

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>(order =>
        {
            order.Property(o => o.Code).HasMaxLength(20);
            order.Property(o => o.CustomerName).HasMaxLength(100);
            order.Property(o => o.Status).HasConversion<string>().HasMaxLength(20);
            order.HasIndex(o => o.Code).IsUnique();

            order.OwnsMany(o => o.Items, items =>
            {
                items.Property(i => i.PizzaName).HasMaxLength(100);
                items.Property(i => i.Notes).HasMaxLength(200);
            });
        });

        modelBuilder.Entity<User>(user =>
        {
            user.Property(u => u.UserName).HasMaxLength(50); 
            user.Property(u => u.PasswordHash).HasMaxLength(200);
            user.HasIndex(u => u.UserName).IsUnique();
        });

        modelBuilder.Entity<LogEntry>(log =>
        {
            log.ToTable("Logs");
            log.Property(l => l.Level).HasMaxLength(20);
            log.Property(l => l.Category).HasMaxLength(200);
        });
    }
}
