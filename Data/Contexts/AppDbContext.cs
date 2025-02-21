using Microsoft.EntityFrameworkCore;
using Data.Entities;

namespace Data.Contexts;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<Project> Projects { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Service> Services { get; set; }
    public DbSet<Staff> Staff { get; set; }
    public DbSet<Status> Statuses { get; set; }
    public DbSet<Role> Roles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Project>()
            .Property(p => p.TotalPrice)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Service>()
            .Property(s => s.HourlyPrice)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Project>()
            .HasKey(p => p.ProjectNumber); 

        modelBuilder.Entity<Project>()
            .HasIndex(p => p.ProjectNumber)
            .IsUnique(); 

        modelBuilder.Entity<Role>().HasData(
           new Role { Id = 1, Name = "Project Manager" },
           new Role { Id = 2, Name = "Developer" },
           new Role { Id = 3, Name = "Designer" }
       );

        base.OnModelCreating(modelBuilder);
    }
}
