using Data.Contexts;
using Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public class UnitOfWork : IUnitOfWork, IAsyncDisposable
    {
        private readonly AppDbContext _context;
        private readonly ILogger<UnitOfWork> _logger;
        private IDbContextTransaction? _transaction;
        private bool _disposed = false;

        public UnitOfWork(AppDbContext context, ILogger<UnitOfWork> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task BeginTransactionAsync()
        {
            if (_transaction != null)
            {
                _logger.LogWarning("Transaction already in progress.");
                throw new InvalidOperationException("Transaction is already in progress.");
            }

            _transaction = await _context.Database.BeginTransactionAsync();
            _logger.LogInformation("Transaction started.");
        }

        public async Task CommitAsync()
        {
            if (_transaction == null)
            {
                _logger.LogWarning("No active transaction to commit.");
                return;
            }

            try
            {
                await _context.SaveChangesAsync();
                await _transaction.CommitAsync();
                _logger.LogInformation("Transaction committed.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Commit failed, rolling back.");
                await RollbackAsync();
                throw;
            }
            finally
            {
                await DisposeTransactionAsync();
            }
        }

        public async Task RollbackAsync()
        {
            if (_transaction == null)
            {
                _logger.LogWarning("No active transaction to roll back.");
                return;
            }

            try
            {
                await _transaction.RollbackAsync();
                _logger.LogWarning("Transaction rolled back.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Rollback failed.");
                throw;
            }
            finally
            {
                await DisposeTransactionAsync();
            }
        }

        private async Task DisposeTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
                _logger.LogInformation("Transaction disposed.");
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _context.Dispose();
                _disposed = true;
                GC.SuppressFinalize(this);
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (!_disposed)
            {
                if (_transaction != null)
                {
                    await DisposeTransactionAsync();
                }

                await _context.DisposeAsync();
                _disposed = true;
                GC.SuppressFinalize(this);
            }
        }
    }
}
