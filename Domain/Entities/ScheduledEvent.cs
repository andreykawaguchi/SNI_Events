using SNI_Events.Domain.Entities.Base;

namespace SNI_Events.Domain.Entities
{
    public class ScheduledEvent : EntityBase
    {
        public long EventId { get; private set; }
        public Event Event { get; private set; } = null!;
        public string Name { get; private set; }
        public DateOnly Date { get; private set; }
        public DateTime DateTime { get; private set; }
        public TimeOnly EndTime { get; private set; }
        public bool IsAllDay { get; private set; } = false;

        public ICollection<User> Users { get; private set; } = new List<User>();

        // One ScheduledEvent may be related to one Dinner (optional)
        public long? DinnerId { get; private set; }
        public Dinner? Dinner { get; private set; }

        public ScheduledEvent() { } // Required by EF Core

        public ScheduledEvent(
            long eventId, 
            string name,
            DateOnly date,
            DateTime dateTime,
            TimeOnly endTime,
            bool isAllDay,
            long? createdByUserId
        )
        {
            Name = name;
            Date = date;
            DateTime = dateTime;
            EndTime = endTime;
            IsAllDay = isAllDay;
            EventId = eventId;

            SetCreationAudit(createdByUserId);
        }

        public void Update(long eventId, string name, DateOnly date, DateTime dateTime, TimeOnly endTime, bool isAllDay, long? modifiedByUserId)
        {
            EventId = eventId;
            Name = name;
            Date = date;
            DateTime = dateTime;
            EndTime = endTime;
            IsAllDay = isAllDay;

            SetModificationAudit(modifiedByUserId);
        }

        public void AddUser(User user)
        {
            if (!Users.Contains(user))
                Users.Add(user);
        }

        public void RemoveUser(User user)
        {
            if (Users.Contains(user))
                Users.Remove(user);
        }
    }
}
