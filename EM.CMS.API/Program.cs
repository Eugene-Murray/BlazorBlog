using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using EM.CMS.API.Data;
using EM.CMS.API.Endpoints;
using EM.CMS.API.Models;
using EM.CMS.API.Models.UserManagement;
using EM.CMS.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Configure EF Core with SQL Server
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Add Identity API endpoints for SPA authentication
builder.Services.AddIdentityApiEndpoints<ApplicationUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register services
builder.Services.AddScoped<IWeatherForecastService, WeatherForecastService>();
builder.Services.AddScoped<IUserManagementService, UserManagementService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200") // Angular dev server
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

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
app.UseCors("AngularApp");

// Map Identity API endpoints for registration, login, etc.
app.MapIdentityApi<ApplicationUser>();

// Map feature endpoints
app.MapAuthEndpoints();
app.MapWeatherForecastEndpoints();
app.MapUserManagementEndpoints();
app.MapRoleManagementEndpoints();

app.Run();
