using Business.Dtos;
using Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/projects")]
public class ProjectsController(
    IProjectService projectService,
    IStaffService staffService,
    IServiceService serviceService,
    ICustomerService customerService) : ControllerBase
{
    private readonly IProjectService _projectService = projectService;
    private readonly IStaffService _staffService = staffService;
    private readonly IServiceService _serviceService = serviceService;
    private readonly ICustomerService _customerService = customerService;

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
            return CreatedAtAction(nameof(Get), new { projectNumber = newNumber }, new { message = "Project created successfully!", projectNumber = newNumber });
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

    [HttpPut("{projectNumber}")]
    public async Task<IActionResult> UpdateProject(string projectNumber, [FromBody] ProjectDto dto)
    {
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
                dto.CustomerId = await _customerService.EnsureCustomerAsync(dto.Customer.Name, dto.Customer.ContactPerson);
            }
            else if (dto.CustomerId > 0)
            {
                var customerExists = await _customerService.CheckCustomerExistsAsync(dto.CustomerId);
                if (!customerExists)
                {
                    return BadRequest(new { message = $"CustomerId {dto.CustomerId} does not exist." });
                }
            }

            dto.ProjectNumber = projectNumber;
            await _projectService.UpdateProjectAsync(dto);

            return NoContent();
        }
        catch (Exception ex)
        {
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
        var statuses = await _projectService.GetProjectStatusesAsync();
        return Ok(statuses ?? new List<StatusDto>());
    }

    [HttpGet("services")]
    public async Task<IActionResult> GetServices()
    {
        var services = await _serviceService.GetAllServicesAsync();
        return Ok(services ?? new List<ServiceDto>());
    }

    [HttpGet("staff")]
    public async Task<IActionResult> GetStaff()
    {
        try
        {
            var staff = await _staffService.GetAllStaffAsync();
            return staff.Any() ? Ok(staff) : NotFound(new { message = "No staff found in the database." });
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
        var customers = await _customerService.GetAllCustomersAsync();
        return Ok(customers ?? new List<CustomerDto>());
    }
}
