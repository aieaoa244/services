using System.ComponentModel.DataAnnotations;

namespace DepartmentService.Models;

public class DepartmentSaveDto
{
    public int? Id { get; set; }

    [Required]
    public string Name { get; set; } = null!;
}
