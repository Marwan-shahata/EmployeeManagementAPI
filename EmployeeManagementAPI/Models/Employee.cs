namespace EmployeeManagementAPI.Models;

/// <summary>
/// The database entity. It maps to the "Employees" table.
/// It is NOT returned to API clients directly - DTOs are used instead.
/// </summary>
public class Employee
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public decimal Salary { get; set; }
    public DateTime HireDate { get; set; }
}
