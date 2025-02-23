using Employee.Data;
using Employee.Model.Entity;
using Microsoft.AspNetCore.Mvc;
using Employee.Model.DTO;
namespace Employee.Controllers
{
    [Route("api/")]
    [ApiController]
    public class EmployeesControllers : ControllerBase
    {
        private ApplicationDbContext dbContext;

        public EmployeesControllers(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }


        [HttpGet]
        [Route("employees")]
        public IActionResult GetEmployees()
        {
            var EmployeesList = dbContext.EmployeeDetails.ToList();
            return Ok(EmployeesList);
        }

        [HttpGet]
        [Route("employees/{id:int}")]
        public IActionResult GetEmployeeById(int id)
        {
            var Employee = dbContext.EmployeeDetails.Find(id);
            if (Employee is null)
            {
                return NotFound();
            }
            return Ok(Employee);
        }

       
        [HttpPost]
        public IActionResult AddEmployee([FromBody] EmployeeCreateRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { message = "Invalid employee data" });
            }

            // Map DTO to Entity
            var newEmployee = new EmployeeDetails
            {
                
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Phone = request.Phone,
                HireDate = request.HireDate,
                JobTitle = request.JobTitle,
                DepartmentId = request.DepartmentId,
                Salary = request.Salary
            };

            dbContext.EmployeeDetails.Add(newEmployee);
            dbContext.SaveChanges(); // Save to DB

            return CreatedAtAction(nameof(GetEmployeeById), new { id = newEmployee.EmpId }, newEmployee);
        }

        [HttpPut("{id:int}")]
        public IActionResult UpdateEmployee(int id, [FromBody] Update request)
        {
            if (request == null)
            {
                return BadRequest(new { message = "Invalid employee data" });
            }

            // Find the employee by EmpId
            var existingEmployee = dbContext.EmployeeDetails.Find(id);

            // If employee does not exist, return 404 Not Found
            if (existingEmployee == null)
            {
                return NotFound(new { message = "Employee not found" });
            }

            // Update the employee's properties with the new data
            existingEmployee.FirstName = request.FirstName ?? existingEmployee.FirstName;
            existingEmployee.LastName = request.LastName ?? existingEmployee.LastName;
            existingEmployee.Email = request.Email ?? existingEmployee.Email;
            existingEmployee.Phone = request.Phone ?? existingEmployee.Phone;
            existingEmployee.HireDate = request.HireDate;
            existingEmployee.JobTitle = request.JobTitle ?? existingEmployee.JobTitle;
            existingEmployee.DepartmentId = request.DepartmentId ?? existingEmployee.DepartmentId;
            existingEmployee.Salary = request.Salary ?? existingEmployee.Salary;

            // Save changes to the database
            dbContext.SaveChanges();

            return Ok(existingEmployee);  // Return the updated employee
        }

    }
}
