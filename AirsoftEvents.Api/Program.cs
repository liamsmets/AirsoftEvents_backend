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
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

var frontendBaseUrl = builder.Configuration["Frontend:BaseUrl"] ?? "http://localhost:5173";
var idpAuthority = builder.Configuration["Identity:Authority"] ?? "https://localhost:5001";

var connectionstring = builder.Configuration.GetConnectionString("DefaultConnection");

services.AddDbContext<AirsoftEventsAppDbContext>(options =>
    options.UseSqlServer(connectionstring)
);

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

services.Configure<MollieOptions>(
    builder.Configuration.GetSection("Mollie"));
builder.Services.AddHttpContextAccessor();

services.AddSingleton<MockMollieStore>();

services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(frontendBaseUrl.TrimEnd('/'))
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = idpAuthority.TrimEnd('/');
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            NameClaimType = "sub",
            RoleClaimType = "role",
        };

        // In prod moet dit TRUE zijn (want je IS draait op https)
        options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
        options.MapInboundClaims = false;
    });

services.AddAuthorizationBuilder()
    .AddPolicy("ApiReadPolicy", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("scope", "airsoftevents.api.read");
    })
    .AddPolicy("ApiAdminPolicy", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("scope", "airsoftevents.api.admin");
        policy.RequireRole("Admin");
    })
    .AddPolicy("ApiWritePolicy", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireAssertion(ctx =>
        {
            var scopes = ctx.User.FindAll("scope").Select(c => c.Value);
            return scopes.Any(v =>
                v.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Contains("airsoftevents.api.write"));
        });
        policy.RequireRole("Admin", "FieldOwner");
    })
    .AddPolicy("ApiUserWritePolicy", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireAssertion(ctx =>
        {
            var scopes = ctx.User.FindAll("scope").Select(c => c.Value);
            return scopes.Any(v =>
                v.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                 .Contains("airsoftevents.api.write"));
        });
    });
    
services.AddHttpClient("NotificationApi", client =>
{
    client.BaseAddress = new Uri(
        builder.Configuration["NotificationService:BaseUrl"]!
    );
});

var app = builder.Build();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost
});

app.MapOpenApi();
app.MapScalarApiReference();

app.UseHttpsRedirection();
app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<AirsoftEvents.Api.Middleware.CosmosLogMiddleware>();

app.MapControllers();


app.Run();