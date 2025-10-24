using SNI_Events.Domain.Interfaces.Repositories;

namespace SNI_Events.Application.UseCases.User
{
    /// <summary>
    /// Use Case para recuperar um usuário por ID
    /// </summary>
    public class GetUserByIdUseCase
    {
        private readonly IUserRepository _userRepository;

        public GetUserByIdUseCase(IUserRepository userRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public async Task<Domain.Entities.User> ExecuteAsync(long userId)
        {
            var user = await _userRepository.GetByIdAsync(userId)
                ?? throw new KeyNotFoundException($"Usuário com ID {userId} não encontrado.");

            return user;
        }
    }
}
