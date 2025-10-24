using SNI_Events.Application.Common.Results;
using SNI_Events.Domain.Interfaces.Repositories;
using SNI_Events.Domain.Interfaces.Services;

namespace SNI_Events.Application.UseCases.User
{
    /// <summary>
    /// Use Case para mudar a senha de um usuário
    /// Utiliza Result Pattern para tratamento de erros
    /// Utiliza Unit of Work para controle transacional
    /// </summary>
    public class ChangePasswordUseCase
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ChangePasswordUseCase(IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<Result> ExecuteAsync(long userId, string newPassword, long? modifiedByUserId)
        {
            if (string.IsNullOrWhiteSpace(newPassword))
                return Result.Failure("Senha não pode estar vazia");

            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                    return Result.Failure($"Usuário com ID {userId} não encontrado");

                user.ChangePassword(newPassword, modifiedByUserId);

                await _userRepository.UpdateAsync(user);
                await _unitOfWork.CommitAsync();

                return Result.Success();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return Result.Failure($"Erro ao mudar senha: {ex.Message}");
            }
        }
    }
}
