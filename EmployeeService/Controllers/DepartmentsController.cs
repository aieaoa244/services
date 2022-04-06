using System.Text;
using System.Text.Json;
using DepartmentService.Models;
using EmployeeService.Data;
using EmployeeService.Models;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeService.Controllers;

[Route("api/e/[controller]")]
[ApiController]
public class DepartmentsController : ControllerBase
{
    private readonly IEmployeeRepository _repository;

    public DepartmentsController(IEmployeeRepository repository)
    {
        _repository = repository;
    }

    [HttpPost]
    public async Task<IActionResult> HttpPublishDepartment()
    {
        var requestBody = HttpContext.Request.Body;
        using var reader = new StreamReader(requestBody, Encoding.UTF8);
        var message = await reader.ReadToEndAsync();
        Console.WriteLine($">> Got http data: {message}");

        // var departmentPublishDto = JsonSerializer.Deserialize<DepartmentPublishDto>(message);
        // if (departmentPublishDto == null)
        // {
        //     Console.WriteLine(">> Http request failed");
        //     return BadRequest();
        // }

        // var department = departmentPublishDto.Adapt<Department>();

        // if (departmentPublishDto.Event == EventType.Department_created.ToString())
        // {
        //     if (await _repository.ExternalDepartmentExists(department.ExternalId))
        //     {
        //         Console.WriteLine(">> Department already exists");
        //         return BadRequest();
        //     }
        //     _repository.CreateDepartment(department);
        //     await _repository.SaveChangesAsync();
        // }
        // else if (departmentPublishDto.Event == EventType.Department_updated.ToString())
        // {
        //     var departmentFromDb = await _repository.GetExternalDepartment(department.ExternalId);
        //     if (departmentFromDb == null)
        //     {
        //         return NotFound();
        //     }
        //     departmentFromDb.Name = department.Name;
        //     await _repository.SaveChangesAsync();
        // }
        // else
        // {
        //     Console.WriteLine($">> Unknown event {departmentPublishDto.Event}");
        //     return BadRequest();
        // }

        return NoContent();
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DepartmentReadDto>>> GetDepartments()
    {
        var departments = await _repository.GetDepartments();
        return Ok(departments.Adapt<IEnumerable<DepartmentReadDto>>());
    }
}
