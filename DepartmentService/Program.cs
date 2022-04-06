using DepartmentService.AsyncDataClient.RabbitMQ;
using DepartmentService.Data;
using DepartmentService.SyncDataServices.Grpc;
using DepartmentService.SyncDataServices.Http;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

if (builder.Environment.IsProduction())
{
    Console.WriteLine(">> Using SQL Server");
    services.AddDbContext<AppDbContext>(o =>
        o.UseSqlServer(builder.Configuration.GetConnectionString("DepartmentsCn")));

    Console.WriteLine(">> Using k8s secret appsettings");
    builder.Configuration.AddJsonFile("secrets/appsettings.secret.json");
}
else
{
    Console.WriteLine(">> Using InMem db");
    services.AddDbContext<AppDbContext>(o =>
        o.UseInMemoryDatabase("Departments"));
}

services.AddScoped<IDepartmentsRepository, DepartmentsRepository>();
services.AddHttpClient<IDepartmentDataClient, HttpDepartmentDataClient>();
services.AddSingleton<IMessageBusService, RmqMessageBusService>();
services.AddGrpc();

services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.MapControllers();
app.MapGrpcService<GrpcDepartmentService>();
app.MapGet("protos/departments", async context =>
{
    await context.Response.WriteAsync(File.ReadAllText("Protos/departments.proto"));
});

await SetupDb.AddTestData(app, app.Environment.IsProduction());
Console.WriteLine($">> Employees service URI {app.Configuration["EmployeeServiceUri"]}");

app.Run();
