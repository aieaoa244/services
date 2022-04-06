namespace DepartmentService.Models;

public class DepartmentReadDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public bool IsReadDto { get; set; } = true;
}
