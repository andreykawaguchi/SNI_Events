using SNI_Events.Domain.Entities;
using SNI_Events.Domain.Interfaces.Specifications;

namespace SNI_Events.Domain.Specifications
{
    /// <summary>
    /// Specification para filtrar usuários pelo email
    /// </summary>
    public class UserByEmailSpecification : ISpecification<User>
    {
        private readonly string _email;

        public UserByEmailSpecification(string email)
        {
            _email = email;
        }

        public IQueryable<User> ToQuery(IQueryable<User> query)
        {
            if (string.IsNullOrWhiteSpace(_email))
                return query;

            return query.Where(u => u.Email.Address.Contains(_email));
        }
    }
}
