using Business.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Business.Interfaces;

public interface ICustomerService
{
    Task<int> EnsureCustomerAsync(string name, string contactPerson);
    Task<bool> CheckCustomerExistsAsync(int customerId);

    Task<IEnumerable<CustomerDto>> GetAllCustomersAsync();  
}
