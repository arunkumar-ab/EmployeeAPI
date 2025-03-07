using Employee.Model.DTO;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

public static class ValidationHelper
{
    public static string ValidateEmployeeData(EmployeeCreateRequest request)
    {
        // Name validation (At least 2 characters, only letters)
        if (string.IsNullOrWhiteSpace(request.FirstName) || !Regex.IsMatch(request.FirstName, @"^[A-Za-z]{2,}$"))
        {
            return "First name must contain only letters and be at least 2 characters long.";
        }

        if (string.IsNullOrWhiteSpace(request.LastName) || !Regex.IsMatch(request.LastName, @"^[A-Za-z]{2,}$"))
        {
            return "Last name must contain only letters and be at least 2 characters long.";
        }

        // Email validation
        if (!new EmailAddressAttribute().IsValid(request.Email))
        {
            return "Invalid email format.";
        }

        // India-specific phone number validation (Starts with 7, 8, or 9 followed by 9 digits)
        if (!Regex.IsMatch(request.Phone, @"^[6789]\d{9}$"))
        {
            return "Phone number must be a valid 10-digit number starting with 7, 8, or 9.";
        }

        // Salary validation
        if (request.Salary < 0)
        {
            return "Salary cannot be negative.";
        }

        // Hire Date validation (Cannot be in the future)
        if (request.HireDate > DateTime.UtcNow)
        {
            return "Hire date cannot be in the future.";
        }

        return null; // No errors
    }

}