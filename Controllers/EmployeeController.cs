using EmployeePortal.Data;
using EmployeePortal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace EmployeePortal.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly EmployeeService _employeeService;
        private readonly ApplicationDbContext _context;

        public EmployeeController(EmployeeService employeeService,ApplicationDbContext context)
        {
            _employeeService = employeeService;
            _context = context;
        }

        public async Task<IActionResult> List(string searchTerm, string selectedDepartment, string selectedType, int page = 1)
        {
            int pageSize = 5;

            var (employees, totalCount) = await _employeeService.GetEmployees(
                searchTerm, selectedDepartment, selectedType, page, pageSize);

            ViewData["TotalPages"] = (int)Math.Ceiling((double)totalCount / pageSize);
            ViewData["CurrentPage"] = page;
            ViewData["SearchTerm"] = searchTerm;
            ViewData["SelectedDepartment"] = selectedDepartment;
            ViewData["SelectedType"] = selectedType;

            ViewBag.DepartmentOptions = new SelectList(_context.Departments.ToList(), "Id", "Name", selectedDepartment);
            ViewBag.EmployeeTypeOptions = new SelectList(_context.EmployeeTypes.ToList(), "Id", "Name", selectedType);

            ViewBag.PageSizeOptions = new SelectList(new List<int> { 5, 10, 20, 50 }, pageSize);



            return View(new EmployeeListViewModel
            {
                Employees = employees,
                PageNumber = page,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                SearchTerm = searchTerm,
                SelectedDeparment = selectedDepartment,
                SelectedType = selectedType
            });
        }

        public IActionResult AddEmployee()
        {
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name");
            ViewData["EmployeeTypeId"] = new SelectList(_context.EmployeeTypes, "Id", "Name");
            return View(new Employee());
        }

        // POST: Employees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEmployee([Bind("Id,FullName,Email,Position,DepartmentId,HireDate,DateOfBirth,EmployeeTypeId,Gender,Salary")] Employee employee)
        {
            if (!ModelState.IsValid)
            {
                _context.Add(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(List));
            }
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name", employee.DepartmentId);
            ViewData["EmployeeTypeId"] = new SelectList(_context.EmployeeTypes, "Id", "Name", employee.EmployeeTypeId);
            return View(employee);
        }


        public async Task<IActionResult> Update(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name", employee.DepartmentId);
            ViewData["EmployeeTypeId"] = new SelectList(_context.EmployeeTypes, "Id", "Name", employee.EmployeeTypeId);
            return View(employee);
        }
        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.Id == id);
        }
        // POST: Employees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, [Bind("Id,FullName,Email,Position,DepartmentId,HireDate,DateOfBirth,EmployeeTypeId,Gender,Salary")] Employee employee)
        {
            if (id != employee.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                try
                {
                    _context.Update(employee);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(List));
            }
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name", employee.DepartmentId);
            ViewData["EmployeeTypeId"] = new SelectList(_context.EmployeeTypes, "Id", "Name", employee.EmployeeTypeId);
            return View(employee);
        }


        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var employee = await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Type)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(List));
        }

        public async Task<IActionResult> Details(int id)
        {
            var employee = await _employeeService.GetEmployeeById(id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        public async Task<IActionResult> Success(int id)
        {
            var employee = await _employeeService.GetEmployeeById(id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }
    }
}
