using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Data.Interfaces
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
               
        DbSet<T> GetDbSet<T>() where T : class;
    }
}
