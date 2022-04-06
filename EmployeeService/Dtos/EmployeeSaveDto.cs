using System.ComponentModel.DataAnnotations;

namespace EmployeeService.Models;

public class EmployeeSaveDto
{
    public int? Id { get; set; }

    [Required]
    public string Name { get; set; } = null!;

    public bool IsActive { get; set; } = true;
}
