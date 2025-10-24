using SNI_Events.Domain.Entities;
using SNI_Events.Domain.Enum;

namespace SNI_Events.Domain.Services
{
    /// <summary>
    /// Implementação do Domain Service de UserDinner
    /// </summary>
    public class UserDinnerDomainService : IUserDinnerDomainService
    {
        public UserDinner CreateUserDinnerAssociation(User user, Dinner dinner, bool isPresent, EStatusPayment paymentStatus, long? createdByUserId)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (dinner == null)
                throw new ArgumentNullException(nameof(dinner));

            return new UserDinner(user, dinner, isPresent, paymentStatus, createdByUserId);
        }

        public bool CanAddUserToDinner(User user, Dinner dinner)
        {
            if (user == null || dinner == null)
                return false;

            // Lógica de negócio: validar se usuário já está adicionado ao jantar
            return !user.UserDinners.Any(ud => ud.DinnerId == dinner.Id);
        }

        public bool CanChangePaymentStatus(EStatusPayment currentStatus, EStatusPayment newStatus)
        {
            // Regra de negócio: Cancelado não pode voltar para outro estado
            if (currentStatus == EStatusPayment.Canceled)
                return false;

            // Regra de negócio: Não pode mudar para o mesmo status
            if (currentStatus == newStatus)
                return false;

            return true;
        }
    }
}
