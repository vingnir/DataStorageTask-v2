using Data.Contexts;
using Data.Entities;
using Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories
{
    public class ProjectRepository(AppDbContext context) : BaseRepository<Project>(context), IProjectRepository
    {

        public override async Task<IEnumerable<Project>> GetAllAsync()
        {
            return await _context.Projects
                .Include(p => p.Customer)
                .Include(p => p.Status)
                .Include(p => p.Service)
                .Include(p => p.Staff)
                .ThenInclude(s => s.Role)
                .ToListAsync() ?? Enumerable.Empty<Project>();
        }

        public override async Task<Project> GetAsync(object id)
        {
            // If your ProjectNumber is a string PK, cast `id` to string
            var projectNumber = (string)id;

            return await _context.Projects
                .Include(p => p.Customer)
                .Include(p => p.Status)
                .Include(p => p.Service)
                .Include(p => p.Staff)
                .ThenInclude(s => s.Role)
                .FirstOrDefaultAsync(p => p.ProjectNumber == projectNumber) ?? new Project();
        }

    }
}
