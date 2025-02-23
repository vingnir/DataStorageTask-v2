using Business.Interfaces;
using Business.Services;
using Data.Contexts;
using Data.Interfaces;
using Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Register OpenAPI for API documentation
builder.Services.AddOpenApi();

// Configure CORS policy to allow Next.js frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowNextJs", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:5127", "https://localhost:7277")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Configure JSON serialization settings
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    });

// Register DbContextOptions<AppDbContext> as a singleton to avoid Scoped-to-Singleton issues
builder.Services.AddSingleton(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
    optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("Data"));
    return optionsBuilder.Options;
});

// Register DbContext as scoped for repositories and services
builder.Services.AddDbContext<AppDbContext>((provider, options) =>
{
    var dbContextOptions = provider.GetRequiredService<DbContextOptions<AppDbContext>>();
    options.UseSqlServer(dbContextOptions.Extensions.OfType<RelationalOptionsExtension>().First().ConnectionString);
});

// Register DbContextFactory for short-lived queries
builder.Services.AddDbContextFactory<AppDbContext>((provider, options) =>
{
    var dbContextOptions = provider.GetRequiredService<DbContextOptions<AppDbContext>>();
    options.UseSqlServer(dbContextOptions.Extensions.OfType<RelationalOptionsExtension>().First().ConnectionString);
});

// Register UnitOfWork as scoped to manage transactions
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Register services with dependency on UnitOfWork instead of injecting DbContext directly, find out more @ https://learn.microsoft.com/en-us/aspnet/mvc/overview/older-versions/getting-started-with-ef-5-using-mvc-4/implementing-the-repository-and-unit-of-work-patterns-in-an-asp-net-mvc-application

builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IStaffService, StaffService>();
builder.Services.AddScoped<IServiceService, ServiceService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();

var app = builder.Build();

// Enable OpenAPI and Swagger UI in Development mode
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "OpenAPI V1");
    });
    app.UseDeveloperExceptionPage();
}

// Enable CORS policy
app.UseCors("AllowNextJs");

// Configure middleware pipeline
app.UseRouting();
app.UseAuthorization();
app.MapControllers(); // Ensure AddControllers() is registered above

app.Run();
