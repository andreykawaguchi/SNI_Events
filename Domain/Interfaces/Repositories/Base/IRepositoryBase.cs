using System.Linq.Expressions;
using SNI_Events.Domain.Interfaces.Specifications;

namespace SNI_Events.Domain.Interfaces.Repositories.Base;

public interface IRepositoryBase<TEntity> where TEntity : class
{
    Task<TEntity> AddAsync(TEntity entity);
    Task<TEntity> UpdateAsync(TEntity entity);
    Task<TEntity> DeleteAsync(TEntity entity);
    Task<TEntity?> GetByIdAsync(long Id);
    Task<IEnumerable<TEntity>> GetAllAsync();
    Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate);
    
    /// <summary>
    /// Busca entidades usando Specification Pattern
    /// </summary>
    Task<TEntity?> FindBySpecificationAsync(ISpecification<TEntity> specification);
    
    /// <summary>
    /// Busca múltiplas entidades usando Specification Pattern
    /// </summary>
    Task<IEnumerable<TEntity>> FindAllBySpecificationAsync(ISpecification<TEntity> specification);
}
