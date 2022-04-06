namespace EmployeeService.Models;

public class DepartmentReadDto
{
    public int Id { get; set; }
    public int ExternalId { get; set; }
    public string Name { get; set; } = null!;
}
