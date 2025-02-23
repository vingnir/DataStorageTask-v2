using Data.Entities;
using Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public class RoleRepository : BaseRepository<Role>, IRoleRepository
    {
        private readonly ILogger<RoleRepository> _logger;

        public RoleRepository(IUnitOfWork unitOfWork, ILogger<RoleRepository> logger)
            : base(unitOfWork, logger) 
        {
            _logger = logger;
        }

        public async Task<Role?> GetByNameAsync(string roleName)
        {
            if (string.IsNullOrEmpty(roleName))
            {
                _logger.LogWarning("GetByNameAsync was called with an empty role name.");
                return null;
            }

            _logger.LogDebug("Fetching role with Name: {RoleName}", roleName);

            return await _unitOfWork.GetDbSet<Role>()
                .FirstOrDefaultAsync(r => r.Name == roleName);
        }
    }
}
