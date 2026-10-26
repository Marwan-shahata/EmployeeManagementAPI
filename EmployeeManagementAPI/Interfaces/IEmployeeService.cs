using EmployeeManagementAPI.DTOs;

namespace EmployeeManagementAPI.Interfaces;

public interface IEmployeeService
{
    Task<EmployeeDto> CreateAsync(CreateEmployeeDto dto, CancellationToken cancellationToken);
    Task<List<EmployeeDto>> GetAllAsync(CancellationToken cancellationToken);
    Task<EmployeeDto?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<EmployeeDto?> UpdateAsync(int id, UpdateEmployeeDto dto, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
}
