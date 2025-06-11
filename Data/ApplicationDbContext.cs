using EmployeePortal.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EmployeePortal.Data
{
    public class ApplicationDbContext: IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<EmployeeTypeEntity> EmployeeTypes { get; set; }
        public DbSet<DepartmentEntity> Departments { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<EmployeeTypeEntity>().HasData(
                  Enum.GetValues(typeof(EmployeeType))
                .Cast<EmployeeType>()
                .Select(e => new EmployeeTypeEntity
                {
                    Id = (int)e,
                    Name = e.ToString()
                }));
            builder.Entity<DepartmentEntity>().HasData(
                  Enum.GetValues(typeof(Department))
                .Cast<Department>()
                .Select(d => new DepartmentEntity
                {
                    Id = (int)d,
                    Name = d.ToString()
                }));
            base.OnModelCreating(builder);
        }
    }
}
