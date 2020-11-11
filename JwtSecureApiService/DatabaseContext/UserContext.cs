using JwtSecureApiService.Models;
using Microsoft.EntityFrameworkCore;

namespace JwtSecureApiService.DatabaseContext
{
    public class UserContext: DbContext
    {
        public UserContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {

        }

        public DbSet<LoginModel> LoginModels { get; set; }
        public DbSet<Employee> Employees { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) 
        {
            modelBuilder.Entity<LoginModel>().HasData(new LoginModel { 
                Id = 1,
                UserName = "Alex",
                Password = "alex@123"
            });

            modelBuilder.Entity<Employee>().HasData(
                new Employee { Id = 1, Name = "Manoj", Address = "Colombo, Sri Lanka", DepartmentName = "Development" },
                new Employee { Id = 2, Name = "Manee", Address = "Colombo, Sri Lanka", DepartmentName = "Marketing" },
                new Employee { Id = 3, Name = "Malin", Address = "Colombo, Sri Lanka", DepartmentName = "Development" },
                new Employee { Id = 4, Name = "Harshi", Address = "Colombo, Sri Lanka", DepartmentName = "HR" },
                new Employee { Id = 5, Name = "Mahinda", Address = "Colombo, Sri Lanka", DepartmentName = "Finance" });
        }
    }
}
