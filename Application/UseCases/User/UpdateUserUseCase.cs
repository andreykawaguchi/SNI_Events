using SNI_Events.Domain.Entities;
using SNI_Events.Domain.Interfaces.Repositories;

namespace SNI_Events.Application.UseCases.User
{
    /// <summary>
    /// Use Case para atualizar um usuário
    /// </summary>
    public class UpdateUserUseCase
    {
        private readonly IUserRepository _userRepository;

        public UpdateUserUseCase(IUserRepository userRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public async Task<Domain.Entities.User> ExecuteAsync(long userId, UpdateUserRequest request, long? modifiedByUserId)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var user = await _userRepository.GetByIdAsync(userId)
                ?? throw new KeyNotFoundException($"Usuário com ID {userId} não encontrado.");

            user.Update(request.Name, request.PhoneNumber, request.Role ?? "User", modifiedByUserId);

            await _userRepository.UpdateAsync(user);

            return user;
        }
    }

    /// <summary>
    /// Request DTO para Use Case de atualização de usuário
    /// </summary>
    public class UpdateUserRequest
    {
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Role { get; set; } = "User";
    }
}
