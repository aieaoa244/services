using EmployeeService.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeService.Data;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly AppDbContext _context;

    public EmployeeRepository(AppDbContext context)
    {
        _context = context;
    }

    public void CreateDepartment(Department department)
    {
        if (department == null)
        {
            throw new ArgumentNullException(nameof(department));
        }
        _context.Departments.Add(department);
    }

    public void CreateEmployee(Employee employee)
    {
        if (employee == null)
        {
            throw new ArgumentNullException(nameof(employee));
        }
        _context.Employees.Add(employee);
    }

    public void DeleteEmployee(Employee employee)
    {
        _context.Employees.Remove(employee);
    }

    public async Task<bool> DepartmentExists(int departmentId)
    {
        return await _context.Departments.AnyAsync(d => d.Id == departmentId);
    }

    public async Task<bool> ExternalDepartmentExists(int externalDepartmentId)
    {
        return await _context.Departments.AnyAsync(d => d.ExternalId == externalDepartmentId);
    }

    public async Task<Employee?> GetDepartmentEmployee(int departmentId, int employeeId)
    {
        return await _context.Employees.FirstOrDefaultAsync(e => e.DepartmentId == departmentId && e.Id == employeeId);
    }

    public async Task<IEnumerable<Employee>> GetDepartmentEmployees(int departmentId)
    {
        return await _context.Employees.Where(e => e.DepartmentId == departmentId).ToListAsync();
    }

    public async Task<IEnumerable<Department>> GetDepartments()
    {
        return await _context.Departments.ToListAsync();
    }

    public async Task<Department?> GetDepartment(int departmentId)
    {
        return await _context.Departments.FirstOrDefaultAsync(d => d.Id == departmentId);
    }

    public async Task<Department?> GetExternalDepartment(int externalDepartmentId)
    {
        return await _context.Departments.FirstOrDefaultAsync(d => d.ExternalId == externalDepartmentId);
    }

    public void UpdateEmployee(Employee employeeFromDb, Employee employee)
    {
        if (employeeFromDb == null)
        {
            throw new ArgumentNullException(nameof(employeeFromDb));
        }
        if (employee == null)
        {
            throw new ArgumentNullException(nameof(employee));
        }
        _context.Entry(employeeFromDb).CurrentValues.SetValues(employee);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return (await _context.SaveChangesAsync() > 0);
    }
}
