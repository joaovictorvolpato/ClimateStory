using Microsoft.EntityFrameworkCore;

namespace ClimateStory.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Add DbSet for your models
        public DbSet<User> Users { get; set; }

        // Define any additional DbSets (tables) you need here

    public class User
    {
        public int id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string password { get; set; }
    }
}