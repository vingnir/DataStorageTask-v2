
using Business.Dtos;
using Business.Interfaces;
using Data.Contexts;
using Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Controllers;

[ApiController]
[Route("api/projects")]
public class ProjectsController(
    IProjectService projectService,
    IStaffService staffService,
    IServiceService serviceService,
    ICustomerService customerService,
    AppDbContext context) : ControllerBase
{
    private readonly IProjectService _projectService = projectService;
    private readonly IStaffService _staffService = staffService;
    private readonly IServiceService _serviceService = serviceService;
    private readonly AppDbContext _context = context;  

   
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var projects = await _projectService.GetAllProjectsAsync();
        return Ok(projects);
    }

    
    [HttpGet("{projectNumber}")]
    public async Task<IActionResult> Get(string projectNumber)
    {
        var project = await _projectService.GetProjectByNumberAsync(projectNumber);
        return project == null ? NotFound() : Ok(project);
    }

    
    [HttpPost]
    public async Task<IActionResult> CreateProject([FromBody] ProjectDto dto)
    {
        try
        {
            var newNumber = await _projectService.CreateProjectAsync(dto);
            return CreatedAtAction(nameof(Get), new { projectNumber = newNumber }, newNumber);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("create-details")]
    public async Task<IActionResult> CreateProjectWithDetails([FromBody] ProjectCreateDetailedDto model)
    {
        if (model == null)
            return BadRequest("No data provided.");

        try
        {
            var newNumber = await _projectService.CreateProjectWithDetailsAsync(model);
            // Got help from chatGpt 4o to figure out how to return a JSON object
            return CreatedAtAction(
                nameof(Get),
                new { projectNumber = newNumber },
                new { message = "Project created successfully!", projectNumber = newNumber }
            );
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    // Create by me but with help from chatGpt 4o
    // Updates a project with the given project number.
    // Checks if project, staff, customer exists in the database.
    // updates the project with the new details.
    // Returns no content.
    [HttpPut("{projectNumber}")]
    public async Task<IActionResult> UpdateProject(string projectNumber, [FromBody] ProjectDto dto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var existingProject = await _projectService.GetProjectByNumberAsync(projectNumber);
            if (existingProject == null)
            {
                return NotFound(new { message = $"Project '{projectNumber}' not found." });
            }
            if (dto.StaffId <= 0)
            {
                return BadRequest(new { message = "Invalid StaffId." });
            }

            var staffExists = await _staffService.CheckStaffExistsAsync(dto.StaffId);
            if (!staffExists)
            {
                return BadRequest(new { message = $"StaffId {dto.StaffId} does not exist." });
            }

            if (dto.CustomerId <= 0 && dto.Customer != null)
            {
                var newCustomer = new Customer
                {
                    Name = dto.Customer.Name ?? string.Empty,
                    ContactPerson = dto.Customer.ContactPerson ?? string.Empty
                };

                _context.Customers.Add(newCustomer);
                await _context.SaveChangesAsync();

                dto.CustomerId = newCustomer.CustomerId; 
            }
            else if (dto.CustomerId > 0)
            {
                var customerExists = await customerService.CheckCustomerExistsAsync(dto.CustomerId);
                if (!customerExists)
                {
                    return BadRequest(new { message = $"CustomerId {dto.CustomerId} does not exist." });
                }
            }

            dto.ProjectNumber = projectNumber;
            await _projectService.UpdateProjectAsync(dto);

            await transaction.CommitAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return StatusCode(500, new { message = "An error occurred while updating the project.", error = ex.Message });
        }
    }




    [HttpDelete("{projectNumber}")]
    public async Task<IActionResult> DeleteProject(string projectNumber)
    {
        var success = await _projectService.DeleteProjectAsync(projectNumber);
        return success ? NoContent() : NotFound();
    }

    [HttpGet("statuses")]
    public async Task<IActionResult> GetStatuses()
    {
        var statuses = await _context.Statuses.ToListAsync();
        return Ok(statuses ?? new List<Status>());
    }

    [HttpGet("services")]
    public async Task<IActionResult> GetServices()
    {
        var services = await _context.Services.ToListAsync();
        return Ok(services ?? new List<Service>());
    }

    [HttpGet("staff")]
    public async Task<IActionResult> GetStaff()
    {
        try
        {
            var staff = await _context.Staff.Include(s => s.Role).ToListAsync();
            if (staff == null || !staff.Any())
            {
                return NotFound(new { message = "No staff found in the database." });
            }
            return Ok(staff);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching staff: {ex.Message}");
            return StatusCode(500, new { message = "An error occurred while fetching staff." });
        }
    }


    [HttpGet("customers")]
    public async Task<IActionResult> GetCustomers()
    {
        var customers = await _context.Customers.ToListAsync();
        return Ok(customers ?? new List<Customer>());
    }
}
