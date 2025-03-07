using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Employee.Model.Entity
{
    [Table("Employee")]
    public class EmployeeDetails
    {
        [Key]
        public int  EmpId { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public string? Phone { get; set; }
        public required DateTime HireDate { get; set; }
        public  string? JobTitle { get; set; }
        public int? DepartmentId { get; set; }
        public decimal? Salary { get; set; }

    }
    [Table("Department")]
    public class Department
    {
        [Key]
        public required int DepartmentId { get; set; }
        public required String DepName { get; set; }
    }


    [Table("User")]
    public class User
    {
        public int Id { get; set; }  // Primary Key
        public int EmpId { get; set; }  // Foreign Key
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }

    }



}
