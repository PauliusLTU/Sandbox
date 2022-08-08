using Microservices.PlatformService.Data;
using Microservices.PlatformService.SyncDataServices.Http;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var env = builder.Environment;
var services = builder.Services;


var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .Build();

// Add services to the container.

if (env.IsProduction())
{
    Console.WriteLine("--> Using SqlServer Db");
    services.AddDbContext<AppDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("PlatformsConn")));
}
else
{
    Console.WriteLine("--> Using InMem Db");
    services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("InMem"));
}

services.AddScoped<IPlatformRepo, PlatformRepo>();
services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();
services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

var app = builder.Build();

PrepDb.PrepPopulation(app, env.IsProduction());

// Configure the HTTP request pipeline.
if (env.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

Console.WriteLine($"--> CommandService Endpoint {configuration["CommandService"]}");

app.Run();
