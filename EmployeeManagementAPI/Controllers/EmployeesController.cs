using System.ComponentModel.DataAnnotations;
using EmployeeManagementAPI.DTOs;
using EmployeeManagementAPI.Exceptions;
using EmployeeManagementAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _employeeService;

    public EmployeesController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    // POST /api/employees  -> 201 Created | 400 Bad Request | 409 Conflict
    // ASP.NET Core fills "cancellationToken" with HttpContext.RequestAborted automatically.
    [HttpPost]
    [ProducesResponseType(typeof(EmployeeDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<EmployeeDto>> Create(
        [FromBody] CreateEmployeeDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            var created = await _employeeService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (DuplicateEmailException ex)
        {
            return Conflict(new ProblemDetails { Status = 409, Title = "Duplicate email", Detail = ex.Message });
        }
    }

    // GET /api/employees  -> 200 OK
    [HttpGet]
    [ProducesResponseType(typeof(List<EmployeeDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<EmployeeDto>>> GetAll(CancellationToken cancellationToken)
    {
        var employees = await _employeeService.GetAllAsync(cancellationToken);
        return Ok(employees);
    }

    // GET /api/employees/5  -> 200 OK | 404 Not Found | 400 Bad Request (id <= 0)
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(EmployeeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EmployeeDto>> GetById(
        [Range(1, int.MaxValue, ErrorMessage = "Id must be greater than 0.")] int id,
        CancellationToken cancellationToken)
    {
        var employee = await _employeeService.GetByIdAsync(id, cancellationToken);
        if (employee is null)
        {
            return NotFound(NotFoundProblem(id));
        }

        return Ok(employee);
    }

    // PUT /api/employees/5  -> 200 OK | 400 | 404 | 409
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(EmployeeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<EmployeeDto>> Update(
        [Range(1, int.MaxValue, ErrorMessage = "Id must be greater than 0.")] int id,
        [FromBody] UpdateEmployeeDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            var updated = await _employeeService.UpdateAsync(id, dto, cancellationToken);
            if (updated is null)
            {
                return NotFound(NotFoundProblem(id));
            }

            return Ok(updated);
        }
        catch (DuplicateEmailException ex)
        {
            return Conflict(new ProblemDetails { Status = 409, Title = "Duplicate email", Detail = ex.Message });
        }
    }

    // DELETE /api/employees/5  -> 204 No Content | 404 Not Found
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        [Range(1, int.MaxValue, ErrorMessage = "Id must be greater than 0.")] int id,
        CancellationToken cancellationToken)
    {
        var deleted = await _employeeService.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            return NotFound(NotFoundProblem(id));
        }

        return NoContent();
    }

    private static ProblemDetails NotFoundProblem(int id) => new()
    {
        Status = StatusCodes.Status404NotFound,
        Title = "Employee not found",
        Detail = $"No employee with id {id} exists."
    };
}
