using Microsoft.EntityFrameworkCore;
using Data.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public class BaseRepository<T> : IRepository<T> where T : class
    {
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly DbSet<T> _dbSet;
        private readonly ILogger<BaseRepository<T>> _logger;

        public BaseRepository(IUnitOfWork unitOfWork, ILogger<BaseRepository<T>> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _dbSet = _unitOfWork.GetDbSet<T>(); 
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
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
                await _unitOfWork.CommitAsync(); 
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
                await _unitOfWork.CommitAsync(); 
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
                    await _unitOfWork.CommitAsync(); 
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred while deleting the entity.", ex);
            }
        }

        public async Task ExecuteInTransactionAsync(Func<Task> action)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await action();
                await _unitOfWork.CommitAsync();
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }
    }
}
