using SNI_Events.Domain.Entities.Base;
using SNI_Events.Domain.Enum;

namespace SNI_Events.Domain.Entities
{
    public class UserDinner : EntityBase
    {
        public long UserId { get; private set; }
        public User User { get; private set; }

        public long DinnerId { get; private set; }
        public Dinner Dinner { get; private set; }

        public bool IsPresent { get; private set; }
        public EStatusPayment PaymentStatus { get; private set; }

        protected UserDinner() { }

        public UserDinner(User user, Dinner dinner, bool isPresent, EStatusPayment paymentStatus, long? createdByUserId)
        {
            User = user ?? throw new ArgumentNullException(nameof(user));
            Dinner = dinner ?? throw new ArgumentNullException(nameof(dinner));

            UserId = user.Id;
            DinnerId = dinner.Id;
            IsPresent = isPresent;
            PaymentStatus = paymentStatus;

            SetCreationAudit(createdByUserId);
        }

        public void UpdatePresence(bool isPresent, long? modifiedByUserId)
        {
            IsPresent = isPresent;
            SetModificationAudit(modifiedByUserId);
        }

        public void UpdatePaymentStatus(EStatusPayment paymentStatus, long? modifiedByUserId)
        {
            PaymentStatus = paymentStatus;
            SetModificationAudit(modifiedByUserId);
        }
    }
}
