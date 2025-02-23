using System.ComponentModel.DataAnnotations.Schema;

namespace Employee.Model.DTO
{
    [Table("Employee")]
    public class EmployeeCreateRequest
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public string? Phone { get; set; }
        public required DateTime HireDate { get; set; }
        public string? JobTitle { get; set; }
        public int? DepartmentId { get; set; }
        public decimal? Salary { get; set; }
    }
}
