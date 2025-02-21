using Business.Dtos;

namespace Business.Interfaces;

public interface IServiceService
{
    Task<int> EnsureServiceAsync(ServiceDto serviceDto);
}
