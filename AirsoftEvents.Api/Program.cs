using AirsoftEvents.Domain.Services;
using AirsoftEvents.Domain.Services.Interfaces;
using AirsoftEvents.Persistance;
using AirsoftEvents.Persistance.Entities;
using AirsoftEvents.Persistance.Interface;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.HttpOverrides;
using Mollie.Api.Client.Abstract;
using Mollie.Api.Client;
using AirsoftEvents.Api.Options;
using Serilog;
using AirsoftEvents.Api.Extensions;

Serilog.Debugging.SelfLog.Enable(msg => Console.WriteLine($"[SERILOG ERROR] {msg}"));

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;


Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.AzureCosmosDB(
        endpointUrl: new Uri(builder.Configuration["Cosmos:EndpointUrl"]!),
        authorizationKey: builder.Configuration["Cosmos:AuthorizationKey"],
        databaseName: builder.Configuration["Cosmos:DatabaseName"] ?? "airsoft",
        collectionName: builder.Configuration["Cosmos:ContainerName"] ?? "logs"
    )
    .CreateLogger();


builder.Host.UseSerilog();


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
        .AddScoped<IFieldService, FieldService>()
        .AddScoped<IReservationRepo, ReservationRepo>()
        .AddScoped<IReservationService, ReservationService>()
        .AddScoped<IFieldImageRepo, FieldImageRepo>();


services.Configure<FieldImageStorageOptions>(
    builder.Configuration.GetSection(nameof(FieldImageStorageOptions)));

services.AddControllers()
        .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

services.AddHttpContextAccessor();

services.AddScoped<IPaymentClient, PaymentClient>(x =>
    new PaymentClient(builder.Configuration["MollieOptions:ApiKey"]!));

services.Configure<MollieOptions>(builder.Configuration.GetSection("MollieOptions"));

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
        policy.RequireClaim("scope", "airsoftevents.api.write");
        policy.RequireRole("Admin", "FieldOwner");
    })
    .AddPolicy("ApiUserWritePolicy", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("scope", "airsoftevents.api.write");
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


app.UseSerilogRequestLogging(options =>
{
    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        if (httpContext.User.Identity?.IsAuthenticated == true)
        {
            var userId = httpContext.User.GetUserId();

            if (userId != Guid.Empty)
            {
                diagnosticContext.Set("UserId", userId);
            }
        }
    };
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();