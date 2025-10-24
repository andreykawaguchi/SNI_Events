using SNI_Events.Domain.Entities;
using SNI_Events.Domain.Interfaces.Specifications;

namespace SNI_Events.Domain.Specifications
{
    /// <summary>
    /// Specification para filtrar usuários pelo CPF
    /// </summary>
    public class UserByCpfSpecification : ISpecification<User>
    {
        private readonly string _cpf;

        public UserByCpfSpecification(string cpf)
        {
            _cpf = cpf;
        }

        public IQueryable<User> ToQuery(IQueryable<User> query)
        {
            if (string.IsNullOrWhiteSpace(_cpf))
                return query;

            return query.Where(u => u.Cpf.Number.Contains(_cpf));
        }
    }
}
