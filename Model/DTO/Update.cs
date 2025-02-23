using System.ComponentModel.DataAnnotations.Schema;

namespace Employee.Model.DTO
{
    public class Update
    {
        public  string FirstName { get; set; }
        public  string LastName { get; set; }
        public  string Email { get; set; }
        public string? Phone { get; set; }
        public  DateTime HireDate { get; set; }
        public string? JobTitle { get; set; }
        public int? DepartmentId { get; set; }
        public decimal? Salary { get; set; }
    }
}


        
  