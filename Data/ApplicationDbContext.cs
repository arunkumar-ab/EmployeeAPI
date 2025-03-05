using Microsoft.EntityFrameworkCore;
using Employee.Model.Entity;
using Employee.Model.DTO;
namespace Employee.Data
{
    public class ApplicationDbContext : DbContext
    {
        public  DbSet<EmployeeDetails> EmployeeDetails{ get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<User> Users { get; set; }

        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }
    }
}
