namespace TaskManager.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;
using TaskManager.Core.Entities;

public class TaskManagerDbContext : DbContext
{
    public DbSet<Task> Tasks { get; set; } = null!;

    public TaskManagerDbContext(DbContextOptions<TaskManagerDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Task configuration
        modelBuilder.Entity<Task>()
            .HasKey(t => t.Id);

        modelBuilder.Entity<Task>()
            .Property(t => t.Title)
            .IsRequired()
            .HasMaxLength(200);

        modelBuilder.Entity<Task>()
            .Property(t => t.Description)
            .HasMaxLength(2000);

        modelBuilder.Entity<Task>()
            .Property(t => t.Status)
            .HasConversion<int>();

        modelBuilder.Entity<Task>()
            .Property(t => t.CreatedAt)
            .HasDefaultValue(DateTime.UtcNow);
    }
}
