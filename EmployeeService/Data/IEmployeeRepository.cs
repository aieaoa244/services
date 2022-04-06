using EmployeeService.Models;

namespace EmployeeService.Data;

public interface IEmployeeRepository
{
    Task<IEnumerable<Department>> GetDepartments();
    Task<Department?> GetDepartment(int departmentId);
    Task<Department?> GetExternalDepartment(int externalDepartmentId);
    Task<bool> DepartmentExists(int departmentId);
    Task<bool> ExternalDepartmentExists(int externalDepartmentId);
    void CreateDepartment(Department department);

    Task<IEnumerable<Employee>> GetDepartmentEmployees(int departmentId);
    Task<Employee?> GetDepartmentEmployee(int departmentId, int employeeId);
    void CreateEmployee(Employee employee);
    void UpdateEmployee(Employee employeeFromDb, Employee employee);
    void DeleteEmployee(Employee employee);

    Task<bool> SaveChangesAsync();
}
