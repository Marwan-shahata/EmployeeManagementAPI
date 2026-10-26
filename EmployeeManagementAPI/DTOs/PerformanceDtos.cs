namespace EmployeeManagementAPI.DTOs;

/// <summary>Result of ONE scenario (sequential OR concurrent).</summary>
public record PerformanceResultDto(
    string Scenario,
    int OperationCount,
    int DelayPerOperationMs,
    double ElapsedMilliseconds);

/// <summary>Result of running both scenarios one after the other (never at the same time).</summary>
public record PerformanceComparisonDto(
    PerformanceResultDto Sequential,
    PerformanceResultDto Concurrent,
    double DifferenceMilliseconds,
    double SpeedupFactor,
    string Explanation);
