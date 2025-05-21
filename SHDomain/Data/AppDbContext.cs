using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SHDomain.Models;
using SHDomain.Models.Agent;
using SHDomain.Models.Apartment;
using SHDomain.Models.Employees;
using SHDomain.Models.User;
using SHDomain.Models.Vehicle;

namespace SHDomain.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Apartment> Apartments { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Audit> Audits { get; set; }
        public DbSet<Agent> Agents { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<EmployeeTravelHistory> EmployeeTravelHistory { get; set; }
        public DbSet<EmployeeApartmentHistory> EmployeeApartmentHistory { get; set; }
        public DbSet<EmployeeVehicleHistory> EmployeeVehicleHistory { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.ConfigureWarnings(warnings =>
                warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    FirstName = "Livhuwani",
                    LastName = "Masindi",
                    Email = "km@gmail.com",
                    Role = "SuperAdministrator",
                    Password = BCrypt.Net.BCrypt.HashPassword("123456"),
                    IsDeleted = false
                },
                new User
                {
                    Id = 2,
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "jd@gmail.com",
                    Role = "Administrator",
                    Password = BCrypt.Net.BCrypt.HashPassword("123456"),
                    IsDeleted = false
                },
                new User
                {
                    Id = 3,
                    FirstName = "Alice",
                    LastName = "Johnson",
                    Email = "alice@gmail.com",
                    Role = "Administrator",
                    Password = BCrypt.Net.BCrypt.HashPassword("123456"),
                    IsDeleted = false
                }
            );
        }
    }
}
