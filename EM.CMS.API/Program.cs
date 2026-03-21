using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using EM.CMS.API.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Configure EF Core with SQL Server
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Add Identity API endpoints for SPA authentication
builder.Services.AddIdentityApiEndpoints<IdentityUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "EM.CMS API v1");
        options.RoutePrefix = string.Empty; // Serve Swagger UI at the app's root
    });
}

app.UseHttpsRedirection();

// Map Identity API endpoints for registration, login, etc.
app.MapIdentityApi<IdentityUser>();

// Add logout endpoint
app.MapPost("/logout", async (SignInManager<IdentityUser> signInManager) =>
{
    await signInManager.SignOutAsync();
    return Results.Ok();
}).RequireAuthorization();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.RequireAuthorization();

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
