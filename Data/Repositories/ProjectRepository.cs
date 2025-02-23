using Data.Entities;
using Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public class ProjectRepository : BaseRepository<Project>, IProjectRepository
    {
        private readonly ILogger<ProjectRepository> _logger;

        public ProjectRepository(IUnitOfWork unitOfWork, ILogger<ProjectRepository> logger)
            : base(unitOfWork, logger) 
        {
            _logger = logger;
        }

        public override async Task<IEnumerable<Project>> GetAllAsync()
        {
            _logger.LogDebug("Fetching all projects with related entities.");

            return await _unitOfWork.GetDbSet<Project>()
                .Include(p => p.Customer)
                .Include(p => p.Status)
                .Include(p => p.Service)
                .Include(p => p.Staff)
                .ThenInclude(s => s.Role)
                .ToListAsync() ?? Enumerable.Empty<Project>();
        }

        public override async Task<Project> GetAsync(object id)
        {
            var projectNumber = id as string;
            if (string.IsNullOrEmpty(projectNumber))
            {
                _logger.LogWarning("Invalid project number provided.");
                return new Project();
            }

            _logger.LogDebug("Fetching project with ProjectNumber: {ProjectNumber}", projectNumber);

            return await _unitOfWork.GetDbSet<Project>()
                .Include(p => p.Customer)
                .Include(p => p.Status)
                .Include(p => p.Service)
                .Include(p => p.Staff)
                .ThenInclude(s => s.Role)
                .FirstOrDefaultAsync(p => p.ProjectNumber == projectNumber) ?? new Project();
        }

        public async Task<IEnumerable<Status>> GetProjectStatusesAsync()
        {
            return await _unitOfWork.GetDbSet<Status>()
                .Where(s => s.StatusId.HasValue && !string.IsNullOrEmpty(s.Name))
                .ToListAsync() ?? new List<Status>();
        }



    }
}
