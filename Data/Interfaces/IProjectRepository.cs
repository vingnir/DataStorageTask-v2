using Data.Entities;

namespace Data.Interfaces;

public interface IProjectRepository : IRepository<Project>
{
    Task ExecuteInTransactionAsync(Func<Task> value);
}
