
using Data.Contexts;
using Data.Entities;
using Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories;

public class CustomerRepository(AppDbContext context) : BaseRepository<Customer>(context), ICustomerRepository
{
    private readonly AppDbContext _context = context;

    public async Task<Customer> GetByNameAsync(string name)
    {
        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.Name == name);
        return customer!;
    }

    public async Task<Customer> GetByIdAsync(int customerId)
    {
        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.CustomerId == customerId);
        return customer!;
    }

    public async Task AddAsync(Customer customer)
    {
        await _context.Customers.AddAsync(customer);
        await _context.SaveChangesAsync();
        
    }
}
