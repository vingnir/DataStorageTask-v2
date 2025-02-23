using Business.Dtos;
using Business.Interfaces;
using Data.Entities;
using Data.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Business.Services;

public class CustomerService(ICustomerRepository customerRepo) : ICustomerService
{
    private readonly ICustomerRepository _customerRepo = customerRepo;

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

    public async Task<IEnumerable<CustomerDto>> GetAllCustomersAsync()
    {
        var customers = await _customerRepo.GetAllAsync();  // ✅ Uses `BaseRepository<T>.GetAllAsync()`

        return customers.Select(c => new CustomerDto
        {
           
            Name = c.Name,
            ContactPerson = c.ContactPerson
        }).ToList();
    }
}
