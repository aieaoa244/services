using DepartmentService.Models;
using Microsoft.EntityFrameworkCore;

namespace DepartmentService.Data;

public class DepartmentsRepository : IDepartmentsRepository
{
    private readonly AppDbContext _context;
    public DepartmentsRepository(AppDbContext context)
    {
        _context = context;
    }

    public void CreateDepartment(Department department)
    {
        if(department == null)
        {
            throw new ArgumentNullException(nameof(department));
        }
        _context.Departments.Add(department);
    }

    public void DeleteDepartment(Department department)
    {
        _context.Remove(department);
    }

    public void UpdateDepartment(Department departmentFromDb, Department department)
    {
        _context.Entry(departmentFromDb).CurrentValues.SetValues(department);
    }

    public async Task<Department?> GetDepartmentById(int id)
    {
        return await _context.Departments.FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<IEnumerable<Department>> GetDepartments()
    {
        return await _context.Departments.ToListAsync();
    }

    public async Task<bool> SaveChangesAsync()
    {
        return (await _context.SaveChangesAsync() > 0);
    }
}
