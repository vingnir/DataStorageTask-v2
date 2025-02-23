using Data.Entities;

namespace Data.Interfaces;

public interface ICustomerRepository : IRepository<Customer>
{
    Task<Customer?> GetByNameAsync(string customerName);
    Task<Customer?> GetByIdAsync(int customerId); 
}
