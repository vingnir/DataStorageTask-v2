using Business.Dtos;

namespace Business.Interfaces;

public interface IStaffService
{
    Task<int> EnsureStaffAsync(StaffDto staffDto);
    Task<bool> CheckStaffExistsAsync(int staffId);
}
