using System.Threading.Tasks;
using Data.Entities;

namespace Business.Interfaces
{
    public interface ICustomerService
    {
        Task<int> EnsureCustomerAsync(string name, string contactPerson);
        Task CreateCustomerAsync(Customer customer); // ✅ Add this method
        Task<bool> CheckCustomerExistsAsync(int customerId); // ✅ Add this method
    }
}
