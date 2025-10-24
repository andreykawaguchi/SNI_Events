using SNI_Events.Domain.Enum;
using SNI_Events.Domain.Interfaces.Repositories;
using SNI_Events.Domain.Services;

namespace SNI_Events.Application.UseCases.UserDinner
{
    /// <summary>
    /// Use Case para atualizar o status de pagamento de um usuário em um jantar
    /// </summary>
    public class UpdateUserDinnerPaymentStatusUseCase
    {
        private readonly IDinnerRepository _dinnerRepository;
        private readonly IUserDinnerDomainService _userDinnerDomainService;

        public UpdateUserDinnerPaymentStatusUseCase(
            IDinnerRepository dinnerRepository,
            IUserDinnerDomainService userDinnerDomainService)
        {
            _dinnerRepository = dinnerRepository ?? throw new ArgumentNullException(nameof(dinnerRepository));
            _userDinnerDomainService = userDinnerDomainService ?? throw new ArgumentNullException(nameof(userDinnerDomainService));
        }

        public async Task ExecuteAsync(long userDinnerId, EStatusPayment newStatus, long? modifiedByUserId)
        {
            var dinner = await _dinnerRepository.GetByIdAsync(userDinnerId)
                ?? throw new KeyNotFoundException($"Jantar com ID {userDinnerId} não encontrado.");

            var userDinner = dinner.UserDinners.FirstOrDefault(ud => ud.Id == userDinnerId)
                ?? throw new KeyNotFoundException($"Registro de participação não encontrado.");

            if (!_userDinnerDomainService.CanChangePaymentStatus(userDinner.PaymentStatus, newStatus))
                throw new InvalidOperationException("Transição de status de pagamento não é permitida.");

            userDinner.UpdatePaymentStatus(newStatus, modifiedByUserId);

            await _dinnerRepository.UpdateAsync(dinner);
        }
    }
}
