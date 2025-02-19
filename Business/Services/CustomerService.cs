using Business.Interfaces;
using Data.Entities;
using Data.Interfaces;
using System.Threading.Tasks;

namespace Business.Services
{
    public class CustomerService(ICustomerRepository customerRepo) : ICustomerService
    {
        private readonly ICustomerRepository _customerRepo = customerRepo;

        public async Task<int> EnsureCustomerAsync(string name, string contactPerson)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Customer name cannot be empty.");

            // 1) Check if a customer with the same name already exists
            var existing = await _customerRepo.GetByNameAsync(name);
            if (existing != null)
            {
                return existing.CustomerId;
            }

            // 2) Otherwise, create a new one
            var newCustomer = new Customer
            {
                Name = name,
                ContactPerson = contactPerson
            };
            await _customerRepo.AddAsync(newCustomer);
            return newCustomer.CustomerId;
        }

        // ✅ New Method: Create Customer
        public async Task CreateCustomerAsync(Customer customer)
        {
            if (customer == null) throw new ArgumentNullException(nameof(customer));

            await _customerRepo.AddAsync(customer);
        }

        // ✅ New Method: Check if Customer Exists
        public async Task<bool> CheckCustomerExistsAsync(int customerId)
        {
            var customer = await _customerRepo.GetByIdAsync(customerId);
            return customer != null;
        }

        // ✅ New Method: Update an existing customer
        public async Task UpdateCustomerAsync(Customer customer)
        {
            if (customer == null) throw new ArgumentNullException(nameof(customer));

            var existingCustomer = await _customerRepo.GetByIdAsync(customer.CustomerId);
            if (existingCustomer == null)
            {
                throw new KeyNotFoundException($"Customer with ID {customer.CustomerId} not found.");
            }

            // Update properties
            existingCustomer.Name = customer.Name;
            existingCustomer.ContactPerson = customer.ContactPerson;

            // Save changes to DB
            await _customerRepo.UpdateAsync(existingCustomer);
        }



    }
}
