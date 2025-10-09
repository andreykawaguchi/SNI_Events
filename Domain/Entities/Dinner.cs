using SNI_Events.Domain.Entities.Base;
using SNI_Events.Domain.Enum;

namespace SNI_Events.Domain.Entities
{
    public class Dinner : EntityBase
    {
        public decimal Price { get; private set; }
        public string Name { get; private set; } = "Jantar";

        public ICollection<ScheduledEvent> ScheduledEvents { get; private set; } = new List<ScheduledEvent>();
        public ICollection<UserDinner> UserDinners { get; private set; } = new List<UserDinner>();

        protected Dinner() { }

        public Dinner(string name, decimal price, long? createdByUserId)
        {
            Name = name;
            Price = price;
            SetCreationAudit(createdByUserId);
        }

        public void Update(string name, decimal price, long? modifiedByUserId)
        {
            Name = name;
            Price = price;
            SetModificationAudit(modifiedByUserId);
        }

        public void AddUser(User user, bool isPresent = false, EStatusPayment status = EStatusPayment.Pending, long? createdByUserId = null)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            if (UserDinners.Any(ud => ud.UserId == user.Id)) return;

            var ud = new UserDinner(user, this, isPresent, status, createdByUserId);
            UserDinners.Add(ud);
            user.UserDinners.Add(ud);
        }

        //public void RemoveUser(User user)
        //{
        //    if (user == null) throw new ArgumentNullException(nameof(user));

        //    var ud = UserDinners.FirstOrDefault(x => x.UserId == user.Id && x.DinnerId == this.Id);
        //    if (ud == null) return;

        //    UserDinners.Remove(ud);
        //    user.UserDinners.Remove(ud);
        //}
    }
}
