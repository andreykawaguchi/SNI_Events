using SNI_Events.Domain.Interfaces.Repositories;

namespace SNI_Events.Application.UseCases.User
{
    /// <summary>
    /// Use Case para mudar a senha de um usu�rio
    /// </summary>
    public class ChangePasswordUseCase
    {
        private readonly IUserRepository _userRepository;

        public ChangePasswordUseCase(IUserRepository userRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public async Task ExecuteAsync(long userId, string newPassword, long? modifiedByUserId)
        {
            if (string.IsNullOrWhiteSpace(newPassword))
                throw new ArgumentException("Senha n�o pode estar vazia.", nameof(newPassword));

            var user = await _userRepository.GetByIdAsync(userId)
                ?? throw new KeyNotFoundException($"Usu�rio com ID {userId} n�o encontrado.");

            user.ChangePassword(newPassword, modifiedByUserId);

            await _userRepository.UpdateAsync(user);
        }
    }
}
