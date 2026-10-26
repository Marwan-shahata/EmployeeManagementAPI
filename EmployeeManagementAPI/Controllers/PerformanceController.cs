using EmployeeManagementAPI.DTOs;
using EmployeeManagementAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PerformanceController : ControllerBase
{
    private readonly IPerformanceService _performanceService;

    public PerformanceController(IPerformanceService performanceService)
    {
        _performanceService = performanceService;
    }

    // GET /api/performance/sequential -> about 2000 ms (A, then B)
    [HttpGet("sequential")]
    public async Task<ActionResult<PerformanceResultDto>> Sequential(CancellationToken cancellationToken)
        => Ok(await _performanceService.RunSequentialAsync(cancellationToken));

    // GET /api/performance/concurrent -> about 1000 ms (A and B together)
    [HttpGet("concurrent")]
    public async Task<ActionResult<PerformanceResultDto>> Concurrent(CancellationToken cancellationToken)
        => Ok(await _performanceService.RunConcurrentAsync(cancellationToken));

    // GET /api/performance/compare -> runs sequential, THEN concurrent, and reports the difference
    [HttpGet("compare")]
    public async Task<ActionResult<PerformanceComparisonDto>> Compare(CancellationToken cancellationToken)
        => Ok(await _performanceService.CompareAsync(cancellationToken));
}
