using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementAPI.DTOs;

/// <summary>
/// Shared fields + validation rules for Create and Update requests.
/// [ApiController] automatically returns 400 Bad Request when a rule fails.
/// </summary>
public abstract class EmployeeInputDto : IValidatableObject
{
    [Required(ErrorMessage = "Name is required.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Email format is invalid.")]
    [StringLength(150, ErrorMessage = "Email cannot exceed 150 characters.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Department is required.")]
    [StringLength(100, ErrorMessage = "Department cannot exceed 100 characters.")]
    public string Department { get; set; } = string.Empty;

    [Range(0.01, 100000000, ErrorMessage = "Salary must be greater than 0.")]
    public decimal Salary { get; set; }

    [Required(ErrorMessage = "HireDate is required.")]
    public DateTime? HireDate { get; set; }

    // Extra rule that attributes cannot express: hire date cannot be in the future.
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (HireDate.HasValue && HireDate.Value.Date > DateTime.Today)
        {
            yield return new ValidationResult(
                "HireDate cannot be in the future.",
                new[] { nameof(HireDate) });
        }
    }
}
