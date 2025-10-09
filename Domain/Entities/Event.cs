using SNI_Events.Domain.Entities.Base;
using SNI_Events.Domain.Enum;

namespace SNI_Events.Domain.Entities
{
    public class Event : EntityBase
    {
        public string Name { get; private set; }

        private readonly List<ScheduledEvent> _scheduledEvents = new();
        public IReadOnlyCollection<ScheduledEvent> ScheduledEvents => _scheduledEvents.AsReadOnly();

        protected Event() { }

        public Event(string name, long? createdByUserId)
        {
            SetName(name);
            SetCreationAudit(createdByUserId);
        }

        public void Update(string name, long? modifiedByUserId)
        {
            SetName(name);
            SetModificationAudit(modifiedByUserId);
        }

        public void AddScheduledEvent(ScheduledEvent scheduledEvent)
        {
            if (scheduledEvent == null) throw new ArgumentNullException(nameof(scheduledEvent));
            if (scheduledEvent.EventId != 0 && scheduledEvent.EventId != Id)
                throw new InvalidOperationException("ScheduledEvent pertence a outro evento.");

            _scheduledEvents.Add(scheduledEvent);
        }

        public void RemoveScheduledEvent(long scheduledEventId)
        {
            var item = _scheduledEvents.FirstOrDefault(se => se.Id == scheduledEventId);
            if (item == null) throw new KeyNotFoundException("ScheduledEvent não encontrado no evento.");

            _scheduledEvents.Remove(item);
        }

        public void DeleteUser(long? deletedByUserId)
        {
            SetDeletionAudit(deletedByUserId);
        }

        private void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Nome do evento é obrigatório.", nameof(name));

            if (name.Length > 200)
                throw new ArgumentException("Nome do evento excede o tamanho máximo de 200 caracteres.", nameof(name));

            Name = name.Trim();
        }
    }
}
