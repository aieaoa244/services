using DepartmentService.Models;

namespace DepartmentService.Data;

public interface IDepartmentsRepository
{
    Task<IEnumerable<Department>> GetDepartments();
    Task<Department?> GetDepartmentById(int id);
    void CreateDepartment(Department department);
    void UpdateDepartment(Department departmentFromDb, Department department);
    void DeleteDepartment(Department department);
    Task<bool> SaveChangesAsync();
}
