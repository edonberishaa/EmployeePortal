using EmployeePortal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EmployeePortal.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly EmployeeService _employeeService;
        public EmployeeController()
        {
            _employeeService = new EmployeeService();
        }
        [HttpGet]
        public async Task<IActionResult> List(
            [FromQuery] string SearchTerm,
            [FromQuery] string SelectedDepartment,
            [FromQuery] string SelectedType,
            [FromQuery] int PageNumber = 1,
            [FromQuery] int PageSize = 5
            )
        {
            var (employees,totalCount) = await _employeeService.GetEmployees(SearchTerm
                ,SelectedDepartment, SelectedType, PageNumber, PageSize);

            var viewModel = new EmployeeListViewModel
            {
                Employees = employees,
                PageNumber = PageNumber,
                PageSize = PageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / PageSize),
                SearchTerm = SearchTerm,
                SelectedDeparment = SelectedDepartment,
                SelectedType = SelectedType
            };
            GetSelectLists();
            ViewBag.PageSizeOptions = new SelectList(new List<int>
            {
                3,5,10,15,20,25
            }, PageSize);
            return View(viewModel);
        }
        [HttpGet]
        public IActionResult Create()
        {
            GetSelectLists();
            return View();
        }

        [HttpPost]
        public IActionResult Create([FromForm] Employee employee)
        {
            if (ModelState.IsValid)
            {
                _employeeService.CreateEmployee(employee);
                return RedirectToAction("Success",new { id = employee.Id});
            }
            GetSelectLists();
            return View(employee);
        }
        public IActionResult Success([FromRoute] int id)
        {
            var employee = _employeeService.GetEmployeeById(id);
            if(employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }
        public IActionResult Details([FromRoute] int id)
        {
            var employee = _employeeService.GetEmployeeById(id);
            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }
        [HttpGet]
        public IActionResult Update([FromRoute] int id)
        {
            // Retrieve the employee by ID and prepare the Update view
            var employee = _employeeService.GetEmployeeById(id);
            if (employee == null)
            {
                return NotFound();
            }
            // Prepare dropdown options before rendering the Update view
            GetSelectLists();
            return View(employee);
        }

        [HttpPost]
        public IActionResult Update([FromForm] Employee employee)
        {
            if (ModelState.IsValid)
            {
                _employeeService.UpdateEmployee(employee);
                TempData["Message"] = $"Employee with ID {employee.Id} and Name {employee.FullName} has been updated.";
                return RedirectToAction("List");
            }
            GetSelectLists();
            return View(employee);
        }

        [HttpGet]
        public IActionResult Delete([FromRoute] int id)
        {
            var employee = _employeeService.GetEmployeeById(id);
            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed([FromRoute] int id)
        {
            var employee = _employeeService.GetEmployeeById(id);
            if (employee == null)
            {
                return NotFound();
            }
            _employeeService.DeleteEmployee(id);
            TempData["Message"] = $"Employee with ID {id} and Name {employee.FullName} has been deleted.";
            return RedirectToAction("List");
        }

        [HttpGet]
        public JsonResult GetPositions(Department department)
        {
            var positions = new Dictionary<Department, List<string>>
            {
                { Department.IT, new List<string> { "Software Developer", "System Administrator", "Network Engineer" } },
                { Department.HR, new List<string> { "HR Specialist", "HR Manager", "Talent Acquisition Coordinator" } },
                { Department.Sales, new List<string> { "Sales Executive", "Sales Manager", "Account Executive" } },
                { Department.Admin, new List<string> { "Office Manager", "Executive Assistant", "Receptionist" } }
            };
            var result = positions.ContainsKey(department) ? positions[department] : new List<string>();
            return Json(result);
        }
        private void GetSelectLists()
        {
            ViewBag.DepartmentOptions = new SelectList(Enum.GetValues(typeof(Department)).Cast<Department>());
            ViewBag.EmployeeTypeOptions = new SelectList(Enum.GetValues(typeof(EmployeeType)).Cast<EmployeeType>());
        }
    }
}
