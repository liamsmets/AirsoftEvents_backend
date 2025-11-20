using Scalar.AspNetCore; // <-- Belangrijk: Deze import bovenaan!

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddOpenApi();

var app = builder.Build();


app.MapOpenApi();
app.MapScalarApiReference();

app.UseHttpsRedirection();

app.Run();