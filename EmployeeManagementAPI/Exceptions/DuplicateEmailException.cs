namespace EmployeeManagementAPI.Exceptions;

/// <summary>Thrown by the service when an email is already used by another employee.</summary>
public class DuplicateEmailException : Exception
{
    public DuplicateEmailException(string email)
        : base($"An employee with email '{email}' already exists.")
    {
    }
}
