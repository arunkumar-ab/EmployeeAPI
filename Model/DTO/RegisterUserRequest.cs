namespace Employee.Model.DTO
{
    public class RegisterUserRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string JobTitle { get; set; }
        public int DepartmentId { get; set; }
        public decimal Salary { get; set; }
        public DateTime HireDate { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }
}
