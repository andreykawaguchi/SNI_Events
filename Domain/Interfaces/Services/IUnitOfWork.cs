using SNI_Events.Domain.Interfaces.Repositories;

namespace SNI_Events.Domain.Interfaces.Services
{
    /// <summary>
    /// Interface para opera��es transacionais
    /// Implementa Unit of Work pattern para garantir consist�ncia
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Commit de todas as mudan�as pendentes
        /// </summary>
        Task<int> CommitAsync();

        /// <summary>
        /// Rollback de todas as mudan�as pendentes
        /// </summary>
        Task RollbackAsync();
    }
}
