using Data.Entities;
using Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public class CustomerRepository : BaseRepository<Customer>, ICustomerRepository
    {
        private readonly ILogger<CustomerRepository> _logger;

        public CustomerRepository(IUnitOfWork unitOfWork, ILogger<CustomerRepository> logger)
            : base(unitOfWork, logger) // ✅ Pass UnitOfWork to BaseRepository
        {
            _logger = logger;
        }

        public async Task<Customer?> GetByNameAsync(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                _logger.LogWarning("GetByNameAsync was called with an empty name.");
                return null;
            }

            _logger.LogDebug("Fetching customer with Name: {CustomerName}", name);

            return await _unitOfWork.GetDbSet<Customer>()
                .FirstOrDefaultAsync(c => c.Name == name);
        }

        public async Task<Customer?> GetByIdAsync(int customerId)
        {
            if (customerId <= 0)
            {
                _logger.LogWarning("GetByIdAsync was called with an invalid ID: {CustomerId}", customerId);
                return null;
            }

            _logger.LogDebug("Fetching customer with ID: {CustomerId}", customerId);

            return await _unitOfWork.GetDbSet<Customer>()
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);
        }

        public override async Task AddAsync(Customer customer)
        {
            if (customer == null)
            {
                _logger.LogWarning("Attempted to add a null customer.");
                throw new ArgumentNullException(nameof(customer), "Customer entity cannot be null.");
            }

            try
            {
                _logger.LogDebug("Adding a new customer: {CustomerName}", customer.Name);
                await _unitOfWork.GetDbSet<Customer>().AddAsync(customer);
                await _unitOfWork.CommitAsync(); // ✅ Use UnitOfWork for transaction handling
                _logger.LogInformation("Successfully added customer: {CustomerName}", customer.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding customer: {CustomerName}", customer.Name);
                throw;
            }
        }
    }
}
