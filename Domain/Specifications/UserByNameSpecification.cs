using SNI_Events.Domain.Entities;
using SNI_Events.Domain.Interfaces.Specifications;

namespace SNI_Events.Domain.Specifications
{
    /// <summary>
    /// Specification para filtrar usuários pelo nome
    /// </summary>
    public class UserByNameSpecification : ISpecification<User>
    {
        private readonly string _name;

        public UserByNameSpecification(string name)
        {
            _name = name;
        }

        public IQueryable<User> ToQuery(IQueryable<User> query)
        {
            if (string.IsNullOrWhiteSpace(_name))
                return query;

            return query.Where(u => u.Name.Contains(_name));
        }
    }
}
