using ApplicationsChallenge.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ApplicationsChallenge.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Application> Applications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure the relationship between User and Application
            modelBuilder.Entity<Application>()
                .HasOne(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Seed initial data (optional)
            modelBuilder.Entity<User>().HasData(
                new User 
                { 
                    Id = 1, 
                    Username = "admin", 
                    // In production, you would never store plain passwords
                    // These are hashed values (for demo purposes)
                    PasswordHash = "AQAAAAIAAYagAAAAEAGnKUTF2h3sMHiOXEoueZBaYlHx7vvv6M0qLDQPrj6SH7apaJlZ/FMbT3q8XzjQdA==", 
                    PasswordSalt = "4vSAYDkoK5PHeNUzYC0kXAuKAu6qImnRhdUrUlGnS8Y="
                }
            );
        }
    }
}
