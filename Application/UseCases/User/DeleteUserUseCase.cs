using SNI_Events.Domain.Interfaces.Repositories;

namespace SNI_Events.Application.UseCases.User
{
    /// <summary>
    /// Use Case para deletar um usu�rio
    /// </summary>
    public class DeleteUserUseCase
    {
        private readonly IUserRepository _userRepository;

        public DeleteUserUseCase(IUserRepository userRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public async Task ExecuteAsync(long userId, long? deletedByUserId)
        {
            var user = await _userRepository.GetByIdAsync(userId)
                ?? throw new KeyNotFoundException($"Usu�rio com ID {userId} n�o encontrado.");

            user.DeleteUser(deletedByUserId);

            await _userRepository.UpdateAsync(user);
        }
    }
}
