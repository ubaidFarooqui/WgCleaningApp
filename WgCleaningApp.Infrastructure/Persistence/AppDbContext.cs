using Microsoft.EntityFrameworkCore;
using WgCleaningApp.Domain.Entities;

namespace WgCleaningApp.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Wg> Wgs => Set<Wg>();
    public DbSet<User> Users => Set<User>();
    public DbSet<AppNotification> Notifications { get; set; }

    //public DbSet<TaskItem> CleaningTasks => Set<TaskItem>();
    // public DbSet<CleaningAssignment> CleaningAssignments => Set<CleaningAssignment>();

    public DbSet<TaskItem> Tasks => Set<TaskItem>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Primary Keys
        modelBuilder.Entity<Wg>().HasKey(w => w.Id);
        modelBuilder.Entity<User>().HasKey(u => u.Id);
       // modelBuilder.Entity<CleaningTask>().HasKey(t => t.Id);
        //modelBuilder.Entity<CleaningAssignment>().HasKey(a => a.Id);

        // Unique constraints
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<Wg>()
            .HasIndex(w => w.InviteCode)
            .IsUnique();
    }
}