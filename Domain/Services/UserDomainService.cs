using SNI_Events.Domain.Entities;
using SNI_Events.Domain.Interfaces.Repositories;
using SNI_Events.Domain.ValueObjects;

namespace SNI_Events.Domain.Services
{
    /// <summary>
    /// Implementa��o do Domain Service de usu�rio
    /// Cont�m a l�gica de neg�cio relacionada a usu�rios
    /// </summary>
    public class UserDomainService : IUserDomainService
    {
        private readonly IUserRepository _userRepository;

        public UserDomainService(IUserRepository userRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public async Task<bool> CpfAlreadyExistsAsync(string cpf)
        {
            return await _userRepository.ExistsByCpfAsync(cpf);
        }

        public async Task<bool> EmailAlreadyExistsAsync(string email)
        {
            return await _userRepository.GetByEmailAsync(email) != null;
        }

        public bool IsValidCpf(string cpf)
        {
            try
            {
                _ = new Cpf(cpf);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool IsValidEmail(string email)
        {
            try
            {
                _ = new Email(email);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
