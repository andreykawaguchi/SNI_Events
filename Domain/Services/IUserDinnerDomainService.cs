using SNI_Events.Domain.Entities;
using SNI_Events.Domain.Enum;

namespace SNI_Events.Domain.Services
{
    /// <summary>
    /// Domain Service respons�vel pela l�gica de neg�cio de UserDinner
    /// Implementa regras de neg�cio relacionadas � participa��o em jantares
    /// </summary>
    public interface IUserDinnerDomainService
    {
        /// <summary>
        /// Cria uma nova associa��o entre usu�rio e jantar
        /// </summary>
        UserDinner CreateUserDinnerAssociation(User user, Dinner dinner, bool isPresent, EStatusPayment paymentStatus, long? createdByUserId);

        /// <summary>
        /// Valida se um usu�rio pode ser adicionado a um jantar
        /// </summary>
        bool CanAddUserToDinner(User user, Dinner dinner);

        /// <summary>
        /// Valida se a mudan�a de status de pagamento � v�lida
        /// </summary>
        bool CanChangePaymentStatus(EStatusPayment currentStatus, EStatusPayment newStatus);
    }
}
