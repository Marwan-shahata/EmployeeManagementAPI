using EmployeeManagementAPI.DTOs;

namespace EmployeeManagementAPI.Interfaces;

public interface IPerformanceService
{
    Task<PerformanceResultDto> RunSequentialAsync(CancellationToken cancellationToken);
    Task<PerformanceResultDto> RunConcurrentAsync(CancellationToken cancellationToken);
    Task<PerformanceComparisonDto> CompareAsync(CancellationToken cancellationToken);
}
