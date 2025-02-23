using Business.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Business.Interfaces;

public interface IServiceService
{
    Task<IEnumerable<ServiceDto>> GetAllServicesAsync();  
    Task<int> EnsureServiceAsync(ServiceDto serviceDto);
}
