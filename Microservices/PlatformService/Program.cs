using PlatformService.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

// Add services to the container.

services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("InMem"));
services.AddScoped<IPlatformRepo, PlatformRepo>();
services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

PrepDb.PrepPopulation(app);

app.UseAuthorization();

app.MapControllers();

app.Run();
