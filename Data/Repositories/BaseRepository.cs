using Microsoft.EntityFrameworkCore;
using Data.Contexts;
using Data.Interfaces;

namespace Data.Repositories;

public class BaseRepository<T> : IRepository<T> where T : class
{
    protected readonly AppDbContext _context;
    private readonly DbSet<T> _dbSet;

    public BaseRepository(AppDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        var entities = await _dbSet.ToListAsync();
        return entities;
    }

    public virtual async Task<T> GetAsync(object id)
    {
        if (id == null || (id is int intId && intId <= 0))
        {
            throw new ArgumentException("Invalid ID value.", nameof(id));
        }

        var entity = await _dbSet.FindAsync(id);
        if (entity == null)
        {
            throw new InvalidOperationException($"Entity with id {id} not found.");
        }
        return entity;
    }

    public virtual async Task AddAsync(T entity)
    {
        try
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("An error occurred while adding the entity.", ex);
        }
    }

    public virtual async Task UpdateAsync(T entity)
    {
        try
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("An error occurred while updating the entity.", ex);
        }
    }

    public virtual async Task DeleteAsync(object id)
    {
        try
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("An error occurred while deleting the entity.", ex);
        }
    }

    public async Task ExecuteInTransactionAsync(Func<Task> action)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            await action();
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

}
