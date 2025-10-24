using SNI_Events.Domain.Interfaces.Services;
using SNI_Events.Infraestructure.Context;

namespace SNI_Events.Infraestructure.Services
{
    /// <summary>
    /// Implementa��o do padr�o Unit of Work
    /// Centraliza as transa��es e garante consist�ncia dos dados
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SNIContext _context;

        public UnitOfWork(SNIContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task RollbackAsync()
        {
            // Discarda todas as mudan�as pendentes
            foreach (var entry in _context.ChangeTracker.Entries())
            {
                entry.State = Microsoft.EntityFrameworkCore.EntityState.Detached;
            }
            await Task.CompletedTask;
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
