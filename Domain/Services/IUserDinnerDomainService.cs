using SNI_Events.Domain.Entities;
using SNI_Events.Domain.Enum;

namespace SNI_Events.Domain.Services
{
    /// <summary>
    /// Domain Service responsável pela lógica de negócio de UserDinner
    /// Implementa regras de negócio relacionadas à participação em jantares
    /// </summary>
    public interface IUserDinnerDomainService
    {
        /// <summary>
        /// Cria uma nova associação entre usuário e jantar
        /// </summary>
        UserDinner CreateUserDinnerAssociation(User user, Dinner dinner, bool isPresent, EStatusPayment paymentStatus, long? createdByUserId);

        /// <summary>
        /// Valida se um usuário pode ser adicionado a um jantar
        /// </summary>
        bool CanAddUserToDinner(User user, Dinner dinner);

        /// <summary>
        /// Valida se a mudança de status de pagamento é válida
        /// </summary>
        bool CanChangePaymentStatus(EStatusPayment currentStatus, EStatusPayment newStatus);
    }
}
