using AirsoftEvents.Domain.Services;
using AirsoftEvents.Domain.Services.Interfaces;
using AirsoftEvents.Persistance;
using AirsoftEvents.Persistance.Entities;
using AirsoftEvents.Persistance.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Scalar.AspNetCore;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using AirsoftEvents.Api.Options;
using AirsoftEvents.Api.Payments;


var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

var connectionstring = builder.Configuration.GetConnectionString("DefaultConnection");
var serverVersion = new MySqlServerVersion(ServerVersion.AutoDetect(connectionstring));

services.AddDbContext<AirsoftEventsAppDbContext>(options =>
options.UseMySql(connectionstring, serverVersion));

services.AddOpenApi();

services.AddHttpClient();
services.AddHttpClient<IWeatherService, WeatherService>();

services.AddScoped<IEventRepo, EventRepo>()
        .AddScoped<IEventService, EventService>()
        .AddScoped<IFieldRepo, FieldRepo>()
        .AddScoped<IFieldService,FieldService>()
        .AddScoped<IReservationRepo, ReservationRepo>()
        .AddScoped<IReservationService, ReservationService>()
        .AddScoped<IUserRepo, UserRepo>()
        .AddScoped<IUserService, UserService>()
        
        .AddScoped<IFieldImageRepo, FieldImageRepo>()
        .AddScoped<ILogRepo, LogRepo>();

services.Configure<FieldImageStorageOptions>(
    builder.Configuration.GetSection(nameof(FieldImageStorageOptions)));

services.AddControllers()
        .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

services.Configure<MollieOptions>(builder.Configuration.GetSection("Mollie"));

services.AddSingleton<MockMollieStore>();

services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.WithOrigins("http://localhost:5173");
            policy.AllowAnyHeader();
            policy.AllowAnyMethod();
        });
});

services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
       options.Authority = "https://localhost:5001";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            NameClaimType = "sub",
            RoleClaimType = "role", // <<< BELANGRIJK
        };
        options.RequireHttpsMetadata = false;
        options.MapInboundClaims = false;
    });

services.AddAuthorizationBuilder()
    .AddPolicy("ApiReadPolicy", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("scope", "airsoftevents.api.read");
    })
    .AddPolicy("ApiWritePolicy", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("scope", "airsoftevents.api.write");
        policy.RequireRole("Admin", "FieldOwner"); 
    })
    .AddPolicy("ApiAdminPolicy", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("scope", "airsoftevents.api.admin");
        policy.RequireRole("Admin");
    })
    .AddPolicy("ApiUserWritePolicy", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("scope", "airsoftevents.api.write");
        policy.RequireRole("Admin", "FieldOwner", "Player");
    });
    

var app = builder.Build();


app.MapOpenApi();
app.MapScalarApiReference();

app.UseHttpsRedirection();
app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<AirsoftEvents.Api.Middleware.CosmosLogMiddleware>();

app.MapControllers();


app.Run();