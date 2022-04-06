using EmployeeService.Data;
using EmployeeService.Models;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeService.Controllers;

[Route("api/e/departments/{departmentId:int}/[controller]")]
[ApiController]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeRepository _repository;

    public EmployeesController(IEmployeeRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<EmployeeReadDto>>> GetDepartmentEmployees(int departmentId)
    {
        if (!await _repository.DepartmentExists(departmentId))
        {
            return NotFound();
        }

        var employees = await _repository.GetDepartmentEmployees(departmentId);
        return Ok(employees.Adapt<IEnumerable<EmployeeReadDto>>());
    }

    [HttpGet("{EmployeeId:int}", Name = nameof(GetDepartmentEmployee))]
    public async Task<ActionResult<IEnumerable<EmployeeReadDto>>> GetDepartmentEmployee(int departmentId, int employeeId)
    {
        if (!await _repository.DepartmentExists(departmentId))
        {
            return NotFound();
        }

        var employee = await _repository.GetDepartmentEmployee(departmentId, employeeId);
        if (employee == null)
        {
            return NotFound();
        }
        return Ok(employee.Adapt<EmployeeReadDto>());
    }

    [HttpPost]
    public async Task<ActionResult<EmployeeReadDto>> SaveEmployee(int departmentId, EmployeeSaveDto employeeSaveDto)
    {
        if (!await _repository.DepartmentExists(departmentId))
        {
            return NotFound();
        }

        var employee = employeeSaveDto.Adapt<Employee>();
        employee.DepartmentId = departmentId;
        if (employeeSaveDto.Id == null)
        {
            _repository.CreateEmployee(employee);
            await _repository.SaveChangesAsync();
            return CreatedAtRoute(nameof(GetDepartmentEmployee),
                new { departmentId, employeeId = employee.Id },
                employee.Adapt<EmployeeReadDto>());
        }
        else
        {
            var employeeFromDb = await _repository.GetDepartmentEmployee(departmentId, (int)employeeSaveDto.Id);
            if (employeeFromDb == null)
            {
                return NotFound();
            }

            _repository.UpdateEmployee(employeeFromDb, employee);
            await _repository.SaveChangesAsync();
            return Ok(employee.Adapt<EmployeeReadDto>());
        }
    }
}