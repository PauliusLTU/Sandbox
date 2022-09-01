using Microservices.CommandsService.AsynDataServices;
using Microservices.CommandsService.Data;
using Microservices.CommandsService.EventProcessing;
using Microservices.CommandsService.SyncDataServices.Grpc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
// Add services to the container.

services.AddControllers();
services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("InMem"));
services.AddScoped<ICommandRepo, CommandRepo>();
services.AddHostedService<MessageBusSubscriber>();
services.AddScoped<IPlatformDataClient, PlatformDataClient>();
services.AddSingleton<IEventProcessor, EventProcessor>();
services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

PrepDb.PrepPopulution(app);

app.Run();