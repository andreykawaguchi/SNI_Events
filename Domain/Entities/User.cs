using SNI_Events.Domain.Entities.Base;
using SNI_Events.Domain.Enum;

namespace SNI_Events.Domain.Entities
{
    public class User : EntityBase
    {
        public string Name { get; private set; }
        public string Email { get; private set; }
        public string Password { get; private set; }
        public string PhoneNumber { get; private set; }
        public string CPF { get; private set; }
        public string Role { get; private set; } = "User"; // Padrão é "User", pode ser alterado para "Admin" ou outros

        public ICollection<ScheduledEvent> ScheduledEvents { get; private set; } = new List<ScheduledEvent>();
        public ICollection<UserDinner> UserDinners { get; private set; } = new List<UserDinner>();

        // Construtor vazio exigido pelo EF Core
        protected User() { }

        public User(
            string name,
            string email,
            string password,
            string phoneNumber,
            string cpf,
            long? createdByUserId,
            string role = "User" // Padrão é "User", pode ser alterado para "Admin" ou outros
        )
        {
            Name = name;
            Email = email;
            Password = password;
            PhoneNumber = phoneNumber;
            CPF = cpf;
            Role = role;

            SetCreationAudit(createdByUserId); // da EntityBase
        }

        public void Update(
            string name,
            string email,
            string phoneNumber,
            string cpf,
            long? modifiedByUserId,
            string role = "User" // Padrão é "User", pode ser alterado para "Admin" ou outros
        )
        {
            Name = name;
            Email = email;
            PhoneNumber = phoneNumber;
            CPF = cpf;
            Role = role;

            SetModificationAudit(modifiedByUserId); // da EntityBase
        }

        public void ChangePassword(string newPassword, long? modifiedByUserId)
        {
            Password = newPassword;
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
