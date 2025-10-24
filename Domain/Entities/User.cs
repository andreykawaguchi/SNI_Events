using SNI_Events.Domain.Entities.Base;
using SNI_Events.Domain.Enum;
using SNI_Events.Domain.ValueObjects;

namespace SNI_Events.Domain.Entities
{
    public class User : EntityBase
    {
        public string Name { get; private set; }
        public Email Email { get; private set; }
        public Password Password { get; private set; }
        public PhoneNumber PhoneNumber { get; private set; }
        public Cpf Cpf { get; private set; }
        public string Role { get; private set; } = "User";

        public ICollection<ScheduledEvent> ScheduledEvents { get; private set; } = new List<ScheduledEvent>();
        public ICollection<UserDinner> UserDinners { get; private set; } = new List<UserDinner>();

        protected User() { }

        public User(
            string name,
            string email,
            string password,
            string phoneNumber,
            string cpf,
            long? createdByUserId,
            string role = "User")
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Nome não pode estar vazio.", nameof(name));

            Name = name;
            Email = new Email(email);
            Password = Password.Create(password);
            PhoneNumber = string.IsNullOrWhiteSpace(phoneNumber) ? new PhoneNumber("") : new PhoneNumber(phoneNumber);
            Cpf = new Cpf(cpf);
            Role = role ?? "User";

            SetCreationAudit(createdByUserId);
        }

        public void Update(
            string name,
            string phoneNumber,
            string role = "User",
            long? modifiedByUserId = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Nome não pode estar vazio.", nameof(name));

            Name = name;
            PhoneNumber = string.IsNullOrWhiteSpace(phoneNumber) ? new PhoneNumber("") : new PhoneNumber(phoneNumber);
            Role = role ?? "User";

            SetModificationAudit(modifiedByUserId);
        }

        public void ChangePassword(string newPassword, long? modifiedByUserId)
        {
            if (string.IsNullOrWhiteSpace(newPassword))
                throw new ArgumentException("Senha não pode estar vazia.", nameof(newPassword));

            Password = Password.Create(newPassword);
            SetModificationAudit(modifiedByUserId);
        }

        public void DeleteUser(long? deletedByUserId)
        {
            SetDeletionAudit(deletedByUserId);
        }

        // Domain helpers for dinners
        public void AddDinner(Dinner dinner, bool isPresent = false, EStatusPayment status = EStatusPayment.Pending, long? createdByUserId = null)
        {
            if (dinner == null) throw new ArgumentNullException(nameof(dinner));

            if (UserDinners.Any(ud => ud.DinnerId == dinner.Id)) return;

            var userDinner = new UserDinner(this, dinner, isPresent, status, createdByUserId);
            UserDinners.Add(userDinner);
            dinner.UserDinners.Add(userDinner);
        }

        public void RemoveDinner(Dinner dinner)
        {
            if (dinner == null) throw new ArgumentNullException(nameof(dinner));

            var ud = UserDinners.FirstOrDefault(x => x.DinnerId == dinner.Id && x.UserId == this.Id);
            if (ud == null) return;

            UserDinners.Remove(ud);
            dinner.UserDinners.Remove(ud);
        }
    }
}
