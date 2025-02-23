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

// Register DbContext as scoped for repositories and Unit of Work
builder.Services.AddDbContext<AppDbContext>((provider, options) =>
{
    var dbContextOptions = provider.GetRequiredService<DbContextOptions<AppDbContext>>();
    options.UseSqlServer(dbContextOptions.Extensions.OfType<RelationalOptionsExtension>().First().ConnectionString);
});

// Register DbContextFactory for short-lived queries (optional)
builder.Services.AddDbContextFactory<AppDbContext>((provider, options) =>
{
    var dbContextOptions = provider.GetRequiredService<DbContextOptions<AppDbContext>>();
    options.UseSqlServer(dbContextOptions.Extensions.OfType<RelationalOptionsExtension>().First().ConnectionString);
});

// ✅ Register UnitOfWork to manage all database transactions
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// ✅ Register Repositories that use UnitOfWork instead of injecting DbContext directly
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IStaffRepository, StaffRepository>();
builder.Services.AddScoped<IServiceRepository, ServiceRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();

// ✅ Register Services that rely on repositories (which are managed via UnitOfWork)
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IStaffService, StaffService>();
builder.Services.AddScoped<IServiceService, ServiceService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();

var app = builder.Build();

//// ✅ Ensure Database is Migrated (Ensures database schema updates before starting the app)
//using (var scope = app.Services.CreateScope())
//{
//    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
//    dbContext.Database.Migrate();
//}

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
