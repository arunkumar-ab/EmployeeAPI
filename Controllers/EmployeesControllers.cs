
using Employee.Data;
using Employee.Model.Entity;
using Microsoft.AspNetCore.Mvc;
using Employee.Model.DTO;
using System;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;


namespace Employee.Controllers
{
    [Route("api/")]
    [ApiController]
    public class EmployeesControllers : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly ILogger<EmployeesControllers> _logger;

        public EmployeesControllers(ApplicationDbContext dbContext, ILogger<EmployeesControllers> logger)
        {
            this.dbContext = dbContext;
            _logger = logger;
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
        [HttpPost("login")]
        public IActionResult Login([FromBody] Model.DTO.LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest(new { message = "Email and password are required" });

            try
            {
                // Fetch the user by Email and Password, and include only the necessary fields (EmpId)
                var user = dbContext.Users
                    .FirstOrDefault(u => u.Email == request.Email && u.Password == request.Password);

                if (user == null)
                    return Unauthorized(new { message = "Invalid credentials" });

                // Retrieve the employee details based on EmpId
                var employee = dbContext.EmployeeDetails.FirstOrDefault(e => e.EmpId == user.EmpId);

                return Ok(new
                {
                    message = "Login successful",
                    role = user.Role,
                    employeeDetails = employee
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error during login", error = ex.Message });
            }
        }
        [HttpPost("register")]
        public IActionResult RegisterUser([FromBody] RegisterUserRequest request)
        {
            
            _logger.LogInformation("RegisterUser Request Data: {RequestData}", JsonConvert.SerializeObject(request));

            Console.WriteLine("Request Data: " + System.Text.Json.JsonSerializer.Serialize(request));

            if (double.TryParse(request.Salary.ToString(), out double salary))
            {
                request.Salary = (decimal)salary;
            }
            else
            {
                return BadRequest(new { message = "Invalid Salary format" });
            }
            if (request == null)
                return BadRequest(new { message = "Invalid request data" });
            if (request.DepartmentId <= 0)
            {
                return BadRequest(new { message = "Invalid DepartmentId" });
            }

            if (request.Salary <= 0)
            {
                return BadRequest(new { message = "Salary must be a positive number" });
            }


            try
            {
                // Validate Employee Data
                if (string.IsNullOrWhiteSpace(request.FirstName) || string.IsNullOrWhiteSpace(request.LastName) ||
                    string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Phone) ||
                    string.IsNullOrWhiteSpace(request.JobTitle) || request.DepartmentId <= 0 || request.Salary <= 0)
                {
                    return BadRequest(new { message = "Missing required employee fields" });
                }

                // Validate User Data
                if (string.IsNullOrWhiteSpace(request.Password) || string.IsNullOrWhiteSpace(request.Role))
                {
                    return BadRequest(new { message = "Missing required user fields" });
                }

                // Check if email already exists
                if (dbContext.Users.Any(u => u.Email == request.Email))
                {
                    return Conflict(new { message = "Email already registered" });
                }

                // Create and save Employee
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
                dbContext.SaveChanges();  // Save Employee first to get the EmpId

                // Create and save User
                var newUser = new User
                {
                    Email = request.Email,
                    Password = request.Password,
                    Role = request.Role,
                    EmpId = newEmployee.EmpId  // Assign the EmpId to the Users
                };
                dbContext.Users.Add(newUser);
                dbContext.SaveChanges();  // Save User

                return Ok(new { message = "User registered successfully" });
            }
            catch (DbUpdateException ex)
            {
                // Log the full exception to get more details
                return StatusCode(500, new { message = "Error registering user", error = ex.InnerException?.Message ?? ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error registering user", error = ex.Message });
            }
        }

        [HttpGet("search")]
        public IActionResult SearchEmployees([FromQuery] string query)
        {
            try
            {
                var searchResults = dbContext.EmployeeDetails
                    .Where(e => e.FirstName.Contains(query) ||
                                e.LastName.Contains(query) ||
                                e.Email.Contains(query) ||
                                e.JobTitle.Contains(query) ||
                                e.DepartmentId.ToString().Contains(query))
                    .ToList();

                if (!searchResults.Any())
                {
                    return NotFound(new { message = "No employees match the search criteria." });
                }
                return Ok(searchResults);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while searching employees.", error = ex.Message });
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
