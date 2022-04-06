namespace EmployeeService.Models;

public class EmployeeReadDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public bool IsActive { get; set; } = true;
    public int DepartmentId { get; set; }
}
