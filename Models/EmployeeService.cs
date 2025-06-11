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
                    .Where(p => p.FullName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
            }

            // Filter by department (enum)
            if (!string.IsNullOrEmpty(selectedDepartment) && Enum.TryParse<Department>(selectedDepartment, out var departmentEnum))
            {
                filteredEmployees = filteredEmployees
                    .Where(p => p.DepartmentId == (int)departmentEnum);
            }

            // Filter by employee type (enum)
            if (!string.IsNullOrEmpty(selectedType) && Enum.TryParse<EmployeeType>(selectedType, out var typeEnum))
            {
                filteredEmployees = filteredEmployees
                    .Where(p => p.EmployeeTypeId == (int)typeEnum);
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

        public async Task CreateEmployee(Employee employee)
        {
            await _context.Employees.AddAsync(employee);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateEmployee(Employee employee)
        {
            var existingEmployee = await GetEmployeeById(employee.Id);
            if (existingEmployee != null)
            {
                existingEmployee.FullName = employee.FullName;
                existingEmployee.Email = employee.Email;
                existingEmployee.Position = employee.Position;
                existingEmployee.DepartmentId = employee.DepartmentId;
                existingEmployee.HireDate = employee.HireDate;
                existingEmployee.DateOfBirth = employee.DateOfBirth;
                existingEmployee.EmployeeTypeId = employee.EmployeeTypeId;
                existingEmployee.Gender = employee.Gender;
                existingEmployee.Salary = employee.Salary;

                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteEmployee(int id)
        {
            var employee = await GetEmployeeById(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();
            }
        }
    }
}
