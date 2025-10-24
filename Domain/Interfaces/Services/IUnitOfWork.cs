using SNI_Events.Domain.Interfaces.Repositories;

namespace SNI_Events.Domain.Interfaces.Services
{
    /// <summary>
    /// Interface para operações transacionais
    /// Implementa Unit of Work pattern para garantir consistência
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Commit de todas as mudanças pendentes
        /// </summary>
        Task<int> CommitAsync();

        /// <summary>
        /// Rollback de todas as mudanças pendentes
        /// </summary>
        Task RollbackAsync();
    }
}
