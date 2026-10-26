using EmployeeManagementAPI.Data;
using EmployeeManagementAPI.DTOs;
using EmployeeManagementAPI.Exceptions;
using EmployeeManagementAPI.Interfaces;
using EmployeeManagementAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementAPI.Services;

/// <summary>
/// All database work lives here. Every method receives a CancellationToken and
/// passes it to the EF Core async method it calls. If the client cancels,
/// EF Core throws OperationCanceledException - we deliberately do NOT catch it.
/// </summary>
public class EmployeeService : IEmployeeService
{
    private readonly AppDbContext _context;

    public EmployeeService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<EmployeeDto> CreateAsync(CreateEmployeeDto dto, CancellationToken cancellationToken)
    {
        var email = dto.Email.Trim();

        // Reject duplicates with a friendly error (the unique index is the final safety net).
        var emailExists = await _context.Employees
            .AnyAsync(e => e.Email == email, cancellationToken);
        if (emailExists)
        {
            throw new DuplicateEmailException(email);
        }

        var employee = new Employee
        {
            Name = dto.Name.Trim(),
            Email = email,
            Department = dto.Department.Trim(),
            Salary = dto.Salary,
            HireDate = dto.HireDate!.Value
        };

        _context.Employees.Add(employee);
        await _context.SaveChangesAsync(cancellationToken);

        return ToDto(employee);
    }

    public async Task<List<EmployeeDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        // AsNoTracking = faster for read-only queries. Select = only the columns we return.
        return await _context.Employees
            .AsNoTracking()
            .OrderBy(e => e.Id)
            .Select(e => new EmployeeDto
            {
                Id = e.Id,
                Name = e.Name,
                Email = e.Email,
                Department = e.Department,
                Salary = e.Salary,
                HireDate = e.HireDate
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<EmployeeDto?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var employee = await _context.Employees
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        return employee is null ? null : ToDto(employee);
    }

    public async Task<EmployeeDto?> UpdateAsync(int id, UpdateEmployeeDto dto, CancellationToken cancellationToken)
    {
        // Tracked query: EF Core notices the changes we make and saves them.
        var employee = await _context.Employees
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (employee is null)
        {
            return null; // controller turns this into 404
        }

        var email = dto.Email.Trim();

        var emailUsedByAnother = await _context.Employees
            .AnyAsync(e => e.Email == email && e.Id != id, cancellationToken);
        if (emailUsedByAnother)
        {
            throw new DuplicateEmailException(email);
        }

        employee.Name = dto.Name.Trim();
        employee.Email = email;
        employee.Department = dto.Department.Trim();
        employee.Salary = dto.Salary;
        employee.HireDate = dto.HireDate!.Value;

        await _context.SaveChangesAsync(cancellationToken);

        return ToDto(employee);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var employee = await _context.Employees
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (employee is null)
        {
            return false; // controller turns this into 404
        }

        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    // Entity -> DTO mapping, so EF entities never leave the service layer.
    private static EmployeeDto ToDto(Employee e) => new()
    {
        Id = e.Id,
        Name = e.Name,
        Email = e.Email,
        Department = e.Department,
        Salary = e.Salary,
        HireDate = e.HireDate
    };
}
