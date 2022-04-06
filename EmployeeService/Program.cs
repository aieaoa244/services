using DepartmentService;
using DepartmentService.Models;
using EmployeeService.AsyncDataServices;
using EmployeeService.Data;
using EmployeeService.EventProcessing;
using EmployeeService.Models;
using EmployeeService.SyncDataServices.Grpc;
using Google.Protobuf.Collections;
using Mapster;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

Console.WriteLine(">> Using InMem db");
services.AddDbContext<AppDbContext>(o =>
    o.UseInMemoryDatabase("Employees"));

if (builder.Environment.IsProduction())
{
    Console.WriteLine($">> Using k8s secret appsettings");
    builder.Configuration.AddJsonFile("secrets/appsettings.secret.json");
}

services.AddScoped<IEmployeeRepository, EmployeeRepository>();
services.AddSingleton<IEventProcessor, EventProcessor>();
services.AddHostedService<MessageBusListener>();
services.AddScoped<IDepartmentDataClient, GrpcDepartmentDataClient>();
services.AddControllers();
var app = builder.Build();

#region mapster config
TypeAdapterConfig<DepartmentPublishDto, Department>
    .NewConfig()
    .Map(d => d.ExternalId, s => s.Id)
    .Ignore(d => d.Id);

TypeAdapterConfig<GrpcDepartmentModel, Department>
    .NewConfig()
    .Map(d => d.ExternalId, s => s.Id)
    .Ignore(d => d.Id).Ignore(d => d.Employees);
#endregion

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.MapControllers();

await GrpcDepartmentHelper.PullDepartments(app);

app.Run();
