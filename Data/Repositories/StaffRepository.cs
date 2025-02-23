using Data.Entities;
using Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public class StaffRepository : BaseRepository<Staff>, IStaffRepository
    {
        private readonly ILogger<StaffRepository> _logger;

        public StaffRepository(IUnitOfWork unitOfWork, ILogger<StaffRepository> logger)
            : base(unitOfWork, logger)
        {
            _logger = logger;
        }

      
        public async Task<Staff?> GetByNameAndRoleIdAsync(string staffName, int roleId)
        {
            if (string.IsNullOrEmpty(staffName) || roleId <= 0)
            {
                _logger.LogWarning("GetByNameAndRoleIdAsync was called with invalid parameters. StaffName: {StaffName}, RoleId: {RoleId}", staffName, roleId);
                return null;
            }

            _logger.LogDebug("Fetching staff with Name: {StaffName} and RoleId: {RoleId}", staffName, roleId);

            return await _unitOfWork.GetDbSet<Staff>()
                .FirstOrDefaultAsync(s => s.Name == staffName && s.RoleId == roleId);
        }

       
        public async Task<IEnumerable<Staff>> GetAllWithRolesAsync()
        {
            _logger.LogDebug("Fetching all staff with roles");

            return await _unitOfWork.GetDbSet<Staff>()
                .Include(s => s.Role)  
                .ToListAsync() ?? new List<Staff>();
        }
    }
}
