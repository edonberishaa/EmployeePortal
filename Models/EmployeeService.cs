using EmployeePortal.Data;
using Microsoft.EntityFrameworkCore;

namespace EmployeePortal.Models
{
    public class EmployeeService
    {
        private readonly ApplicationDbContext _context;


        public EmployeeService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(List<Employee> Employees, int TotalCount)> GetEmployees(
            string searchTerm,
            string selectedDepartment,
            string selectedType,
            int pageNumber,
            int pageSize)
        {

            var filteredEmployees = _context.Employees.AsQueryable();

            // Search by name
            if (!string.IsNullOrEmpty(searchTerm))
            {
                filteredEmployees = filteredEmployees
                    .Where(p => p.FullName.Contains(searchTerm.ToLower()));
            }

            // Filter by department (enum)
            if (int.TryParse(selectedDepartment, out int departmentId))
            {
                filteredEmployees = filteredEmployees
                    .Where(p => p.DepartmentId == departmentId);
            }

            if (int.TryParse(selectedType, out int typeId))
            {
                filteredEmployees = filteredEmployees
                    .Where(p => p.EmployeeTypeId == typeId);
            }

            int totalCount = await filteredEmployees.CountAsync();

            var pagedEmployees = await filteredEmployees
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (pagedEmployees, totalCount);
        }

        public async Task<Employee?> GetEmployeeById(int id)
        {
            return await _context.Employees
                .Include(e => e.Department)         // optional: eager loading
                .Include(e => e.Type)         // optional: if needed
                .FirstOrDefaultAsync(e => e.Id == id);
        }
    }
}
