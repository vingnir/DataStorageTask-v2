using Business.Dtos;
using Business.Interfaces;
using Data.Entities;
using Data.Interfaces;

namespace Business.Services
{
    public class ProjectService(IProjectRepository projectRepo,
                          IStaffService staffService,
                          IServiceService serviceService, 
                          ICustomerService customerService) : IProjectService
    {
        private readonly IProjectRepository _projectRepo = projectRepo;
        private readonly IStaffService _staffService = staffService;
        private readonly IServiceService _serviceService = serviceService;
        private readonly ICustomerService _customerService = customerService;



        public async Task<IEnumerable<ProjectDto>> GetAllProjectsAsync()
        {
            var projects = await _projectRepo.GetAllAsync();
            return projects.Select(p => new ProjectDto
            {
                ProjectNumber = p.ProjectNumber,
                Name = p.Name,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                CustomerName = p.Customer != null ? p.Customer.Name : "Ingen kund",
                ContactPerson = p.Customer != null ? p.Customer.ContactPerson : "Ingen kontaktperson",
                ServiceId = p.ServiceId,
                StaffId = p.StaffId,
                StatusName = p.Status.Name != null ? p.Status.Name : "Ingen status",
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

        public async Task<ProjectDto> GetProjectByNumberAsync(string projectNumber)
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

            int serviceId = await _serviceService.EnsureServiceAsync(dto.Service);

            int staffId = await _staffService.EnsureStaffAsync(dto.Staff);

            // Insert project entity directly
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

            // 1) Determine the final CustomerId
            int finalCustomerId = 0;

            // If user picked an existing customer from the dropdown
            if (dto.CustomerId > 0)
            {
                finalCustomerId = dto.CustomerId; // Use the existing ID
            }
            else if (dto.Customer != null)
            {
                // They want to create a new customer
                finalCustomerId = await _customerService.EnsureCustomerAsync(
                    dto.Customer.Name,
                    dto.Customer.ContactPerson
                );
            }
            else
            {
                // Possibly error or let them continue with no customer
                throw new ArgumentException("Either CustomerId > 0 or Customer data is required.");
            }

            // 2) Ensure/Find the Service
            int serviceId = await _serviceService.EnsureServiceAsync(dto.Service);

            // 3) Ensure/Find the Staff
            int staffId = await _staffService.EnsureStaffAsync(dto.Staff);

            // 4) Construct the new Project
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

            // 5) Insert into DB
            await _projectRepo.AddAsync(projectEntity);

            return dto.ProjectNumber;
        }






        // -----------------------------
        // UPDATE
        public async Task UpdateProjectAsync(ProjectDto dto)
        {
            // Get the existing entity from the repository
            var existing = await _projectRepo.GetAsync(dto.ProjectNumber);
            if (existing == null)
                throw new InvalidOperationException($"Project '{dto.ProjectNumber}' not found.");

            // Update top-level fields
            existing.Name = dto.Name;
            existing.StartDate = dto.StartDate;
            existing.EndDate = dto.EndDate;
            existing.CustomerId = dto.CustomerId;
            existing.ServiceId = dto.ServiceId;
            existing.StaffId = dto.StaffId;
            existing.StatusId = dto.StatusId;
            existing.TotalPrice = dto.TotalPrice;
            existing.Description = dto.Description;

            // If your code uses "Ensure" logic, it might look like this:
            if (dto.Service != null)
            {
                // This checks by Service name; if not found, create it:
                var serviceId = await _serviceService.EnsureServiceAsync(dto.Service);
                existing.ServiceId = serviceId;
            }

            if (dto.Staff != null)
            {
                // This checks by Staff name + role; if not found, create it:
                var staffId = await _staffService.EnsureStaffAsync(dto.Staff);
                existing.StaffId = staffId;
            }
            if (dto.Customer != null)
            {
                // This checks by Customer name; if not found, create it:
                var customerId = await _customerService.EnsureCustomerAsync(dto.Customer.Name, dto.Customer.ContactPerson);
                existing.CustomerId = customerId;
            }

            // Finally save
            await _projectRepo.UpdateAsync(existing);
        }


        public async Task<bool> DeleteProjectAsync(string projectNumber)
        {
            await _projectRepo.DeleteAsync(projectNumber);
            return true;
        }
    }
}
