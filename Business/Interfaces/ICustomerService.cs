


namespace Business.Interfaces;

public interface ICustomerService
{
    Task<int> EnsureCustomerAsync(string name, string contactPerson);
    Task<bool> CheckCustomerExistsAsync(int customerId); 
}
