using SNI_Events.Domain.Entities;
using SNI_Events.Domain.Interfaces.Specifications;

namespace SNI_Events.Domain.Specifications
{
    /// <summary>
    /// Specification para filtrar usu�rios pelo email
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

            // Use AsEnumerable to bring data to memory for value object comparison
            return query.AsEnumerable().Where(u => u.Email.Address.Contains(_email)).AsQueryable();
        }
    }
}
