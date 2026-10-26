using System.Diagnostics;
using EmployeeManagementAPI.DTOs;
using EmployeeManagementAPI.Interfaces;

namespace EmployeeManagementAPI.Services;

/// <summary>
/// Compares two independent async operations (each = Task.Delay(1000)):
///   Sequential : await A; await B;                  -> about 2000 ms
///   Concurrent : start A and B, await Task.WhenAll  -> about 1000 ms
/// Task.Delay is used so the demo is reproducible and needs no database.
/// (If real EF Core queries were used, each would need its OWN DbContext instance.)
/// </summary>
public class PerformanceService : IPerformanceService
{
    private const int OperationCount = 2;
    private const int DelayMs = 1000;

    // One simulated "I/O" operation. Passing the token lets a client cancel it early.
    private static Task SimulatedOperationAsync(CancellationToken cancellationToken)
        => Task.Delay(DelayMs, cancellationToken);

    public async Task<PerformanceResultDto> RunSequentialAsync(CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();

        await SimulatedOperationAsync(cancellationToken); // A finishes first...
        await SimulatedOperationAsync(cancellationToken); // ...then B starts

        stopwatch.Stop();
        return new PerformanceResultDto("Sequential", OperationCount, DelayMs, stopwatch.Elapsed.TotalMilliseconds);
    }

    public async Task<PerformanceResultDto> RunConcurrentAsync(CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();

        // Both tasks START immediately (no await yet), so their waits overlap.
        var taskA = SimulatedOperationAsync(cancellationToken);
        var taskB = SimulatedOperationAsync(cancellationToken);

        await Task.WhenAll(taskA, taskB);

        stopwatch.Stop();
        return new PerformanceResultDto("Concurrent", OperationCount, DelayMs, stopwatch.Elapsed.TotalMilliseconds);
    }

    public async Task<PerformanceComparisonDto> CompareAsync(CancellationToken cancellationToken)
    {
        // Tiny warm-up so first-call JIT cost is not counted in the sequential result.
        await Task.Delay(1, cancellationToken);

        // The scenarios run one AFTER the other, never together, so they cannot disturb each other.
        var sequential = await RunSequentialAsync(cancellationToken);
        var concurrent = await RunConcurrentAsync(cancellationToken);

        var difference = sequential.ElapsedMilliseconds - concurrent.ElapsedMilliseconds;
        var speedup = concurrent.ElapsedMilliseconds > 0
            ? sequential.ElapsedMilliseconds / concurrent.ElapsedMilliseconds
            : 0;

        const string explanation =
            "The two operations are independent, so their waiting time can overlap. " +
            "Sequential time is roughly A + B; concurrent time is roughly max(A, B). " +
            "Task.WhenAll is asynchronous concurrency (no thread is blocked while waiting), " +
            "not necessarily CPU parallelism.";

        return new PerformanceComparisonDto(sequential, concurrent, difference, Math.Round(speedup, 2), explanation);
    }
}
