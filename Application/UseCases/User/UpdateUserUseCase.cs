using SNI_Events.Application.Common.Results;
using SNI_Events.Application.UseCases.Base;
using SNI_Events.Domain.Entities;
using SNI_Events.Domain.Interfaces.Repositories;
using SNI_Events.Domain.Interfaces.Services;

namespace SNI_Events.Application.UseCases.User
{
    /// <summary>
    /// Use Case para atualizar um usuário
    /// Utiliza Result Pattern para tratamento de erros
    /// Utiliza Unit of Work para controle transacional
    /// </summary>
    public class UpdateUserUseCase
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateUserUseCase(IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<Result<Domain.Entities.User>> ExecuteAsync(long userId, UpdateUserRequest request, long? modifiedByUserId)
        {
            if (request == null)
                return Result.Failure<Domain.Entities.User>("Requisição inválida");

            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                    return Result.Failure<Domain.Entities.User>($"Usuário com ID {userId} não encontrado");

                user.Update(request.Name, request.PhoneNumber, request.Role ?? "User", modifiedByUserId);

                await _userRepository.UpdateAsync(user);
                await _unitOfWork.CommitAsync();

                return Result.Success(user);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return Result.Failure<Domain.Entities.User>($"Erro ao atualizar usuário: {ex.Message}");
            }
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
