using Microsoft.EntityFrameworkCore;
using UserManagementSystem.Models.Entities;

namespace UserManagementSystem.Data
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }
        public DbSet<User> Users { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<UserDocument> UserDocuments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Only seed data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<State>().HasData(
                new State { Id = 1, Name = "Gujarat" },
                new State { Id = 2, Name = "Maharashtra" },
                new State { Id = 3, Name = "Karnataka" }
            );

            modelBuilder.Entity<City>().HasData(
                new City { Id = 1, Name = "Ahmedabad", StateId = 1 },
                new City { Id = 2, Name = "Surat", StateId = 1 },
                new City { Id = 3, Name = "Vadodara", StateId = 1 },
                new City { Id = 4, Name = "Mumbai", StateId = 2 },
                new City { Id = 5, Name = "Pune", StateId = 2 },
                new City { Id = 6, Name = "Bengaluru", StateId = 3 },
                new City { Id = 7, Name = "Mysuru", StateId = 3 }
            );
        }

    }
}
