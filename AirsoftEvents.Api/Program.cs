using AirsoftEvents.Domain.Services;
using AirsoftEvents.Domain.Services.Interfaces;
using AirsoftEvents.Persistance;
using AirsoftEvents.Persistance.Entities;
using AirsoftEvents.Persistance.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Scalar.AspNetCore; // <-- Belangrijk: Deze import bovenaan!

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

var connectionstring = builder.Configuration.GetConnectionString("DefaultConnection");
var serverVersion = new MySqlServerVersion(ServerVersion.AutoDetect(connectionstring));

services.AddDbContext<AirsoftEventsAppDbContext>(options =>
options.UseMySql(connectionstring, serverVersion));

services.AddOpenApi();

services.AddScoped<IEventRepo, EventRepo>()
        .AddScoped<IEventService, EventService>()
        .AddScoped<IFieldRepo, FieldRepo>()
        .AddScoped<IFieldService,FieldService>()
        .AddScoped<IReservationRepo, ReservationRepo>()
        .AddScoped<IReservationService, ReservationService>()
        .AddScoped<IUserRepo, UserRepo>()
        .AddScoped<IUserService, UserService>();

services.AddControllers();

services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.WithOrigins("Localhost");
            policy.AllowAnyHeader();
            policy.AllowAnyMethod();
        });
});

var app = builder.Build();


app.MapOpenApi();
app.MapScalarApiReference();

app.UseHttpsRedirection();
app.MapControllers();
app.UseCors();

app.Run();