using Business.Dtos;
using Business.Interfaces;
using Data.Entities;
using Data.Interfaces;

namespace Business.Services;

public class ProjectService(IProjectRepository projectRepo,
                      IStaffService staffService,
                      IRoleService roleService,
                      IServiceService serviceService, 
                      ICustomerService customerService) : IProjectService
{
    private readonly IProjectRepository _projectRepo = projectRepo;
    private readonly IStaffService _staffService = staffService;
    private readonly IServiceService _serviceService = serviceService;
    private readonly ICustomerService _customerService = customerService;
    private readonly IRoleService _roleService = roleService;




    public async Task<IEnumerable<ProjectDto>> GetAllProjectsAsync()
    {
        var projects = await _projectRepo.GetAllAsync();
        return projects.Select(p => new ProjectDto
        {
            ProjectNumber = p.ProjectNumber,
            Name = p.Name,
            StartDate = p.StartDate,
            EndDate = p.EndDate,
            CustomerName = p.Customer != null ? p.Customer.Name : "No Customer",
            ContactPerson = p.Customer != null ? p.Customer.ContactPerson : "No Contact Person",
            ServiceId = p.ServiceId,
            StaffId = p.StaffId,
            StatusName = p.Status.Name != null ? p.Status.Name : "No Status",
            TotalPrice = p.TotalPrice,
            Description = p.Description,

            Service = p.Service != null ? new ServiceDto
            {
                Name = p.Service.Name,
                HourlyPrice = p.Service.HourlyPrice
            } : null,

            Staff = p.Staff != null
                ? new StaffDto
                {
                    Name = p.Staff.Name,
                    RoleName = p.Staff.Role.Name ?? "Unknown role"
                }
                : null,
        });
    }

    public async Task<ProjectDto?> GetProjectByNumberAsync(string projectNumber)
    {
        var project = await _projectRepo.GetAsync(projectNumber);
        if (project == null) return null;

        return new ProjectDto
        {
            ProjectNumber = project.ProjectNumber,
            Name = project.Name,
            StartDate = project.StartDate,
            EndDate = project.EndDate,
            CustomerName = project.Customer?.Name ?? "N/A",
            ServiceId = project.ServiceId,
            StaffId = project.StaffId,
            StatusId = project.StatusId,
            TotalPrice = project.TotalPrice,
            Description = project.Description
        };
    }

    public async Task<string> CreateProjectAsync(ProjectDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        if (dto.Service == null)
            throw new ArgumentException("Service cannot be null or empty.");

        int serviceId = await _serviceService.EnsureServiceAsync(dto.Service);

        if (dto.Staff == null)
            throw new ArgumentException("Service cannot be null or empty.");

        int staffId = await _staffService.EnsureStaffAsync(dto.Staff);

        var project = new Project
        {
            ProjectNumber = dto.ProjectNumber,
            Name = dto.Name,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            CustomerId = dto.CustomerId,
            ServiceId = dto.ServiceId,
            StaffId = dto.StaffId,
            StatusId = dto.StatusId,
            TotalPrice = dto.TotalPrice,
            Description = dto.Description
        };

        await _projectRepo.AddAsync(project);
        return dto.ProjectNumber;
    }
    // Created base by chatGpt 4o and modified by me
    // Checks if a projectnr, service, staff, customer exists in the database. If not, creates a new one.
    // Checks if the customer is null or not.
    // Check service and staff exists or not.If not, creates a new one.
    // Adds the project to the database.
    // Returns the project number.
    public async Task<string> CreateProjectWithDetailsAsync(ProjectCreateDetailedDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));
        if (string.IsNullOrWhiteSpace(dto.ProjectNumber))
            throw new ArgumentException("ProjectNumber is required.");
        if (dto.Service == null)
            throw new ArgumentNullException(nameof(dto.Service), "Service is required.");
        if (dto.Staff == null)
            throw new ArgumentNullException(nameof(dto.Staff), "Staff is required.");

        await _projectRepo.ExecuteInTransactionAsync(async () =>
        {

            int finalCustomerId = 0;


            if (dto.CustomerId > 0)
            {
                finalCustomerId = dto.CustomerId;
            }
            else if (dto.Customer != null)
            {

                finalCustomerId = await _customerService.EnsureCustomerAsync(
                    dto.Customer.Name ?? string.Empty,
                    dto.Customer.ContactPerson ?? string.Empty
                );
            }
            else
            {
                throw new ArgumentException("Either CustomerId > 0 or Customer data is required.");
            }

            int serviceId = await _serviceService.EnsureServiceAsync(dto.Service);

            int staffId = await _staffService.EnsureStaffAsync(dto.Staff);
            int roleId = await _roleService.EnsureRoleAsync(dto.Staff.RoleName ?? "Unknown Role");


            var projectEntity = new Project
        {
            ProjectNumber = dto.ProjectNumber,
            Name = dto.Name,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            CustomerId = finalCustomerId,
            ServiceId = serviceId,
            StaffId = staffId,
            StatusId = dto.StatusId,
            TotalPrice = dto.TotalPrice,
            Description = dto.Description
        };

        await _projectRepo.AddAsync(projectEntity);
    });

        return dto.ProjectNumber;
    }

    // Based it by CreateProjectWithDetailsAsync but change so it fits update. So modified by me.

    public async Task UpdateProjectAsync(ProjectDto dto)
    {
        var existing = await _projectRepo.GetAsync(dto.ProjectNumber);
        if (existing == null)
            throw new InvalidOperationException($"Project '{dto.ProjectNumber}' not found.");

        existing.Name = dto.Name;
        existing.StartDate = dto.StartDate;
        existing.EndDate = dto.EndDate;
        existing.CustomerId = dto.CustomerId;
        existing.ServiceId = dto.ServiceId;
        existing.StaffId = dto.StaffId;
        existing.StatusId = dto.StatusId;
        existing.TotalPrice = dto.TotalPrice;
        existing.Description = dto.Description;

      
        if (dto.Service != null)
        {
            var serviceId = await _serviceService.EnsureServiceAsync(dto.Service);
            existing.ServiceId = serviceId;
        }

        if (dto.Staff != null)
        {
            var staffId = await _staffService.EnsureStaffAsync(dto.Staff);
            existing.StaffId = staffId;
        }
        if (dto.Customer != null)
        {
            var customerId = await _customerService.EnsureCustomerAsync(dto.Customer.Name, dto.Customer.ContactPerson);
            existing.CustomerId = customerId;
        }

        await _projectRepo.UpdateAsync(existing);
    }


    public async Task<bool> DeleteProjectAsync(string projectNumber)
    {
        await _projectRepo.DeleteAsync(projectNumber);
        return true;
    }


    public async Task<IEnumerable<StatusDto>> GetProjectStatusesAsync()
    {
        var statuses = await _projectRepo.GetProjectStatusesAsync();


        return statuses.Select(s => new StatusDto
        {
            StatusId = s.StatusId,
            Name = s.Name
        }).ToList();  
    }

    public async Task<IEnumerable<ServiceDto>> GetAllServicesAsync()
    {
        var services = await _serviceService.GetAllServicesAsync(); 

        return services.Select(s => new ServiceDto
        {
            Name = s.Name,
            HourlyPrice = s.HourlyPrice
        }).ToList();  // ✅ Convert to DTOs
    }
}
