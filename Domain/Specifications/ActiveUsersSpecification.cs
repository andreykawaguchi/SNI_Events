using SNI_Events.Domain.Entities;
using SNI_Events.Domain.Interfaces.Specifications;

namespace SNI_Events.Domain.Specifications
{
    /// <summary>
    /// Specification para filtrar usuários ativos
    /// </summary>
    public class ActiveUsersSpecification : ISpecification<User>
    {
        public IQueryable<User> ToQuery(IQueryable<User> query)
        {
            return query.Where(u => u.Status == Domain.Enum.EStatus.Active);
        }
    }
}
