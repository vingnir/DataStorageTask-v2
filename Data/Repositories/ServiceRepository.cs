using Data.Entities;
using Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public class ServiceRepository : BaseRepository<Service>, IServiceRepository
    {
        private readonly ILogger<ServiceRepository> _logger;

        public ServiceRepository(IUnitOfWork unitOfWork, ILogger<ServiceRepository> logger)
            : base(unitOfWork, logger) 
        {
            _logger = logger;
        }

        public async Task<Service?> GetByNameAsync(string serviceName)
        {
            if (string.IsNullOrEmpty(serviceName))
            {
                _logger.LogWarning("GetByNameAsync was called with an empty service name.");
                return null;
            }

            _logger.LogDebug("Fetching service with Name: {ServiceName}", serviceName);

            return await _unitOfWork.GetDbSet<Service>()
                .FirstOrDefaultAsync(s => s.Name == serviceName);
        }
    }
}
