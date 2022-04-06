using System.ComponentModel.DataAnnotations;

namespace EmployeeService.Models;

public class Employee
{
    [Key]
    [Required]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = null!;

    [Required]
    public bool IsActive { get; set; } = true;

    [Required]
    public int DepartmentId { get; set; }

    public Department Department { get; set; } = null!;
}
