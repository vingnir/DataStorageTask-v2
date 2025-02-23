using Business.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Business.Interfaces;

public interface IProjectService
{
    Task<IEnumerable<ProjectDto>> GetAllProjectsAsync();
    Task<ProjectDto?> GetProjectByNumberAsync(string projectNumber);
    Task<string> CreateProjectAsync(ProjectDto dto);
    Task<string> CreateProjectWithDetailsAsync(ProjectCreateDetailedDto dto);
    Task UpdateProjectAsync(ProjectDto dto);
    Task<bool> DeleteProjectAsync(string projectNumber);

    Task<IEnumerable<StatusDto>> GetProjectStatusesAsync();
    Task<IEnumerable<ServiceDto>> GetAllServicesAsync();
}
