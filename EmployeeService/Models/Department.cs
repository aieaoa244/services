using System.ComponentModel.DataAnnotations;

namespace EmployeeService.Models;

public class Department
{
    [Key]
    [Required]
    public int Id { get; set; }

    [Required]
    public int ExternalId { get; set; }

    [Required]
    public string Name { get; set; } = null!;

    public IEnumerable<Employee> Employees { get; set; } = new List<Employee>();
}
