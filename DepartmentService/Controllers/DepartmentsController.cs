using DepartmentService.AsyncDataClient.RabbitMQ;
using DepartmentService.Data;
using DepartmentService.Models;
using DepartmentService.SyncDataServices.Http;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace DepartmentService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DepartmentsController : ControllerBase
{
    private readonly IDepartmentsRepository _repository;
    private readonly IDepartmentDataClient _departmentDataClient;
    private readonly IConfiguration _configuration;
    private readonly IMessageBusService _messageBusService;

    public DepartmentsController(IDepartmentsRepository repository,
        IDepartmentDataClient departmentDataClient,
        IConfiguration configuration,
        IMessageBusService messageBusService)
    {
        _repository = repository;
        _departmentDataClient = departmentDataClient;
        _configuration = configuration;
        _messageBusService = messageBusService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DepartmentReadDto>>> GetDepartments()
    {
        var departments = await _repository.GetDepartments();
        return Ok(departments.Adapt<IEnumerable<DepartmentReadDto>>());
    }

    [HttpGet("{id:int}", Name = nameof(GetDepartment))]
    public async Task<ActionResult<DepartmentReadDto>> GetDepartment(int id)
    {
        var department = await _repository.GetDepartmentById(id);
        if (department == null)
        {
            return NotFound();
        }

        return Ok(department.Adapt<DepartmentReadDto>());
    }

    [HttpPost]
    public async Task<ActionResult<DepartmentReadDto>> SaveDepartment(DepartmentSaveDto departmentSaveDto)
    {
        if (departmentSaveDto.Id == null)
        {
            var department = departmentSaveDto.Adapt<Department>();
            _repository.CreateDepartment(department);
            await _repository.SaveChangesAsync();

            var departmentPublishDto = department.Adapt<DepartmentPublishDto>();
            departmentPublishDto.Event = EventType.Department_created.ToString();

            try
            {
                await _departmentDataClient.SendDepartmentToEmployee(departmentPublishDto);
            }
            catch (Exception e)
            {
                Console.WriteLine($">> Http post create to employee service failed: {e.Message}");
            }

            try
            {
                _messageBusService.PublishDepartment(departmentPublishDto);
            }
            catch (Exception e)
            {
                Console.WriteLine($">> Message bus post failed: {e.Message}");
            }

            var departmentReadDto = department.Adapt<DepartmentReadDto>();
            return CreatedAtRoute(nameof(GetDepartment), new { id = departmentReadDto.Id }, departmentReadDto);
        }
        else
        {
            var departmentFromDb = await _repository.GetDepartmentById((int)departmentSaveDto.Id);
            if (departmentFromDb == null)
            {
                return NotFound();
            }

            var department = departmentSaveDto.Adapt<Department>();
            _repository.UpdateDepartment(departmentFromDb, department);
            await _repository.SaveChangesAsync();

            var departmentPublishDto = department.Adapt<DepartmentPublishDto>();
            departmentPublishDto.Event = EventType.Department_updated.ToString();

            try
            {
                await _departmentDataClient.SendDepartmentToEmployee(departmentPublishDto);
            }
            catch (Exception e)
            {
                Console.WriteLine($">> Http post update to employee service failed: {e.Message}");
            }

            try
            {
                _messageBusService.PublishDepartment(departmentPublishDto);
            }
            catch (Exception e)
            {
                Console.WriteLine($">> Message bus post failed: {e.Message}");
            }

            return Ok(departmentFromDb.Adapt<DepartmentReadDto>());
        }
    }

    [HttpPost("{id:int}/delete")]
    public async Task<IActionResult> DeleteDepartment(int id)
    {
        var departmentFromDb = await _repository.GetDepartmentById(id);
        if (departmentFromDb == null)
        {
            return NotFound();
        }

        _repository.DeleteDepartment(departmentFromDb);
        await _repository.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("emppoint")]
    public IActionResult GetEmployeesEndpoint()
    {
        return Ok(new { emppoint = _configuration["EmployeeServiceUri"] });
    }
}
