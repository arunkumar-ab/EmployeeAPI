//using Employee.Data;
//using Employee.Model.Entity;
//using Microsoft.AspNetCore.Mvc;
//using Employee.Model.DTO;

//namespace Employee.Controllers
//{
//    [Route("api/")] // Base route for the API
//    [ApiController] // Specifies that this controller responds to API requests
//    public class EmployeesControllers : ControllerBase
//    {
//        private readonly ApplicationDbContext dbContext;

//        // Constructor to inject the database context
//        public EmployeesControllers(ApplicationDbContext dbContext)
//        {
//            this.dbContext = dbContext;
//        }

//        // GET: api/employees
//        // Retrieves all employees from the database
//        [HttpGet]
//        [Route("employees")]
//        public IActionResult GetEmployees()
//        {
//            var EmployeesList = dbContext.EmployeeDetails.ToList();
//            return Ok(EmployeesList); // Returns 200 OK with the list of employees
//        }

//        // GET: api/employees/{id}
//        // Retrieves a single employee by ID
//        [HttpGet]
//        [Route("employees/{id:int}")]
//        public IActionResult GetEmployeeById(int id)
//        {
//            var Employee = dbContext.EmployeeDetails.Find(id);
//            if (Employee is null)
//            {
//                return NotFound(new { message = "Employee not found" }); // Returns 404 if the employee is not found
//            }
//            return Ok(Employee); // Returns 200 OK with the employee details
//        }

//        // POST: api/employees
//        // Creates a new employee
//        [HttpPost]
//        public IActionResult AddEmployee([FromBody] EmployeeCreateRequest request)
//        {
//            if (request == null)
//            {
//                return BadRequest(new { message = "Invalid employee data" }); // Returns 400 Bad Request if request is null
//            }

//            // Map DTO to Entity and create a new Employee record
//            var newEmployee = new EmployeeDetails
//            {
//                FirstName = request.FirstName,
//                LastName = request.LastName,
//                Email = request.Email,
//                Phone = request.Phone,
//                HireDate = request.HireDate,
//                JobTitle = request.JobTitle,
//                DepartmentId = request.DepartmentId,
//                Salary = request.Salary
//            };

//            dbContext.EmployeeDetails.Add(newEmployee); // Add employee to database
//            dbContext.SaveChanges(); // Save changes to DB

//            return CreatedAtAction(nameof(GetEmployeeById), new { id = newEmployee.EmpId }, newEmployee);
//            // Returns 201 Created with the new employee's details
//        }

//        // PUT: api/employees/{id}
//        // Updates an existing employee with new data (partial updates allowed)
//        [HttpPut("{id:int}")]
//        public IActionResult UpdateEmployee(int id, [FromBody] Update request)
//        {
//            if (request == null)
//            {
//                return BadRequest(new { message = "Invalid employee data" }); // Returns 400 Bad Request if request is null
//            }

//            // Find the employee by ID in the database
//            var existingEmployee = dbContext.EmployeeDetails.Find(id);

//            // If employee does not exist, return 404 Not Found
//            if (existingEmployee == null)
//            {
//                return NotFound(new { message = "Employee not found" });
//            }

//            // Update the employee's properties only if valid data is provided
//            existingEmployee.FirstName = (request.FirstName != "string") ? request.FirstName : existingEmployee.FirstName;
//            existingEmployee.LastName = (request.LastName != "string") ? request.LastName : existingEmployee.LastName;
//            existingEmployee.Email = (request.Email != "string") ? request.Email : existingEmployee.Email;
//            existingEmployee.Phone = (request.Phone != "string") ? request.Phone : existingEmployee.Phone;
//            existingEmployee.HireDate = request.HireDate != DateTime.MinValue ? request.HireDate : existingEmployee.HireDate;
//            existingEmployee.JobTitle = (request.JobTitle != "string") ? request.JobTitle : existingEmployee.JobTitle;
//            existingEmployee.DepartmentId = (request.DepartmentId != 0) ? request.DepartmentId : existingEmployee.DepartmentId;
//            existingEmployee.Salary = (request.Salary != 0) ? request.Salary : existingEmployee.Salary;

//            // Save the updated employee record to the database
//            dbContext.SaveChanges();

//            return Ok(existingEmployee);  // Returns 200 OK with updated employee data
//        }

//        // DELETE: api/employees/{id}
//        // Deletes an employee by ID
//        [HttpDelete("{id:int}")]
//        public IActionResult DeleteEmployee(int id)
//        {
//            // Find the employee by ID in the database
//            var existingEmployee = dbContext.EmployeeDetails.Find(id);

//            // If employee does not exist, return 404 Not Found
//            if (existingEmployee == null)
//            {
//                return NotFound(new { message = "Employee not found" });
//            }

//            // Remove the employee from the database
//            dbContext.EmployeeDetails.Remove(existingEmployee);
//            dbContext.SaveChanges(); // Save changes to DB

//            return Ok(new { message = "Employee deleted successfully" });
//            // Returns 200 OK with confirmation message
//        }
//    }
//}


using Employee.Data;
using Employee.Model.Entity;
using Microsoft.AspNetCore.Mvc;
using Employee.Model.DTO;
using System;
using Microsoft.EntityFrameworkCore;

namespace Employee.Controllers
{
    [Route("api/")]
    [ApiController]
    public class EmployeesControllers : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;

        public EmployeesControllers(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        [Route("employees")]
        public IActionResult GetEmployees()
        {
            try
            {
                var EmployeesList = dbContext.EmployeeDetails.ToList();
                return Ok(EmployeesList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching employees.", error = ex.Message });
            }
        }

        [HttpGet]
        [Route("employees/{id:int}")]
        public IActionResult GetEmployeeById(int id)
        {
            try
            {
                var Employee = dbContext.EmployeeDetails.Find(id);
                if (Employee is null)
                {
                    return NotFound(new { message = "Employee not found" });
                }
                return Ok(Employee);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching the employee.", error = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult AddEmployee([FromBody] EmployeeCreateRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { message = "Invalid employee data" });
            }

            // Use ValidationHelper for validation
            var validationError = ValidationHelper.ValidateEmployeeData(request);
            if (validationError != null)
            {
                return BadRequest(new { message = validationError });
            }

            try
            {
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
                dbContext.SaveChanges();

                return CreatedAtAction(nameof(GetEmployeeById), new { id = newEmployee.EmpId }, newEmployee);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while adding the employee.", error = ex.Message });
            }
        }

        [HttpPut("{id:int}")]
        public IActionResult UpdateEmployee(int id, [FromBody] Update request)
        {
            if (request == null)
            {
                return BadRequest(new { message = "Invalid employee data" });
            }

            try
            {
                var existingEmployee = dbContext.EmployeeDetails.Find(id);
                if (existingEmployee == null)
                {
                    return NotFound(new { message = "Employee not found" });
                }

                var validationError = ValidationHelper.ValidateEmployeeData(request);
                if (validationError != null)
                {
                    return BadRequest(new { message = validationError });
                }

                // Update fields only if valid data is provided
                existingEmployee.FirstName = request.FirstName;
                existingEmployee.LastName = request.LastName;
                existingEmployee.Email = request.Email ;
                existingEmployee.Phone = request.Phone ;
                existingEmployee.HireDate = request.HireDate; 
                existingEmployee.JobTitle = request.JobTitle ;
                existingEmployee.DepartmentId = request.DepartmentId ;
                existingEmployee.Salary = request.Salary;

                dbContext.SaveChanges();

                return Ok(existingEmployee);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the employee.", error = ex.Message });
            }
        }

        [HttpDelete("{id:int}")]
        public IActionResult DeleteEmployee(int id)
        {
            try
            {
                var existingEmployee = dbContext.EmployeeDetails.Find(id);
                if (existingEmployee == null)
                {
                    return NotFound(new { message = "Employee not found" });
                }

                dbContext.EmployeeDetails.Remove(existingEmployee);
                dbContext.SaveChanges();

                return Ok(new { message = "Employee deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the employee.", error = ex.Message });
            }
        }
    }
}
