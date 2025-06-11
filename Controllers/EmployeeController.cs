using EmployeePortal.Models;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace EmployeePortal.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly EmployeeService _employeeService;

        public EmployeeController(EmployeeService employeeService)
        {
            _employeeService = employeeService;
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
            GetSelectLists();
            return View(new Employee());
        }

        [HttpPost]
        public async Task<IActionResult> AddEmployee([FromForm] Employee employee)
        {
            if (ModelState.IsValid)
            {
                await _employeeService.CreateEmployee(employee);
                return RedirectToAction("Success", new { id = employee.Id });
            }

            GetSelectLists();
            return View(employee);
        }

        public async Task<IActionResult> Update(int id)
        {
            var employee = await _employeeService.GetEmployeeById(id);
            if (employee == null)
            {
                return NotFound();
            }

            GetSelectLists();
            return View(employee);
        }

        [HttpPost]
        public async Task<IActionResult> Update([FromForm] Employee employee)
        {
            if (ModelState.IsValid)
            {
                await _employeeService.UpdateEmployee(employee);
                return RedirectToAction("Success", new { id = employee.Id });
            }

            GetSelectLists();
            return View(employee);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var employee = await _employeeService.GetEmployeeById(id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _employeeService.DeleteEmployee(id);
            return RedirectToAction("Index");
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

        private void GetSelectLists()
        {
            ViewData["Departments"] = Enum.GetValues(typeof(Department)).Cast<Department>();
            ViewData["Types"] = Enum.GetValues(typeof(EmployeeType)).Cast<EmployeeType>();
        }
    }
}
