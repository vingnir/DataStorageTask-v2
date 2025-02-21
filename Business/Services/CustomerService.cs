using Business.Interfaces;
using Data.Entities;
using Data.Interfaces;

namespace Business.Services;

public class CustomerService(ICustomerRepository customerRepo) : ICustomerService
{
    private readonly ICustomerRepository _customerRepo = customerRepo;
    // Created by chatGpt 4o
    // Checks if a customer with the given name exists in the database.
    // If not, creates a new customer with the input written. Like name and contact person.
    // And returns customer ID.
    public async Task<int> EnsureCustomerAsync(string name, string contactPerson)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Customer name cannot be empty.");

        var existing = await _customerRepo.GetByNameAsync(name);
        if (existing != null)
        {
            return existing.CustomerId;
        }
        var newCustomer = new Customer
        {
            Name = name,
            ContactPerson = contactPerson
        };
        await _customerRepo.AddAsync(newCustomer);
        return newCustomer.CustomerId;
    }
    
    public async Task<bool> CheckCustomerExistsAsync(int customerId)
    {
        var customer = await _customerRepo.GetByIdAsync(customerId);
        return customer != null;
    }

    
   

}
