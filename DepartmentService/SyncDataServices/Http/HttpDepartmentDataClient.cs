using System.Text;
using System.Text.Json;
using DepartmentService.Models;

namespace DepartmentService.SyncDataServices.Http;

public class HttpDepartmentDataClient : IDepartmentDataClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    
    public HttpDepartmentDataClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task SendDepartmentToEmployee(DepartmentPublishDto department)
    {
        var httpContent = new StringContent(
            JsonSerializer.Serialize(department),
            Encoding.UTF8,
            "application/json"
        );

        var response = await _httpClient.PostAsync($"{_configuration["EmployeeServiceUri"]}", httpContent);

        if(response.IsSuccessStatusCode)
        {
            Console.WriteLine(">> Http post to employee service OK");
        }
        else
        {
            Console.WriteLine($">> Http post to employee service NOT OK: {response.ReasonPhrase}");
        }
    }
}
