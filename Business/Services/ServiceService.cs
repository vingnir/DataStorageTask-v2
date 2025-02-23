using Business.Dtos;
using Business.Interfaces;
using Data.Entities;
using Data.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Business.Services;

public class ServiceService : IServiceService
{
    private readonly IServiceRepository _serviceRepo;

    public ServiceService(IServiceRepository serviceRepo)
    {
        _serviceRepo = serviceRepo;
    }

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

    
    public async Task<IEnumerable<ServiceDto>> GetAllServicesAsync()
    {
        var services = await _serviceRepo.GetAllAsync();

        return services.Select(s => new ServiceDto
        {
            Name = s.Name,
            HourlyPrice = s.HourlyPrice
        }).ToList();
    }
}
