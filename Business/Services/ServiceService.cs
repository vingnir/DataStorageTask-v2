using Business.Dtos;
using Business.Interfaces;
using Data.Entities;
using Data.Interfaces;

namespace Business.Services;

public class ServiceService : IServiceService
{
    private readonly IServiceRepository _serviceRepo;

    public ServiceService(IServiceRepository serviceRepo)
    {
        _serviceRepo = serviceRepo;
    }
    // Based on CustomerService but i have written it myself.
    public async Task<int> EnsureServiceAsync(ServiceDto serviceDto)
    {
        if (serviceDto == null)
            throw new ArgumentException("Service details cannot be null.");

        if (string.IsNullOrWhiteSpace(serviceDto.Name))
            throw new ArgumentException("Service name is required.");

        var existingService = await _serviceRepo.GetByNameAsync(serviceDto.Name);
        if (existingService != null)
        {
            existingService.HourlyPrice = serviceDto.HourlyPrice;
            await _serviceRepo.UpdateAsync(existingService);
            return existingService.ServiceId;
        }

        var newService = new Service
        {
            Name = serviceDto.Name,
            HourlyPrice = serviceDto.HourlyPrice
        };
        await _serviceRepo.AddAsync(newService);
        return newService.ServiceId;
    }




}
