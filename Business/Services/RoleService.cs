using Business.Interfaces;
using Data.Entities;
using Data.Interfaces;

namespace Business.Services;

public class RoleService(IRoleRepository roleRepo) : IRoleService
{
    private readonly IRoleRepository _roleRepo = roleRepo;
    // Based on CustomerService but i have written it myself.
    public async Task<int> EnsureRoleAsync(string roleName)
    {
        if (string.IsNullOrWhiteSpace(roleName))
            throw new ArgumentException("Role name cannot be empty.");

        var existing = await _roleRepo.GetByNameAsync(roleName);
        if (existing != null)
        {
            return existing.Id;
        }
        else
        {
            var newRole = new Role { Name = roleName };
            await _roleRepo.AddAsync(newRole);
            return newRole.Id;
        }
    }
}