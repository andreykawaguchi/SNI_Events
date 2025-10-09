using SNI_Events.Domain.Enum;

namespace SNI_Events.Domain.Entities.Base
{
    public abstract class EntityBase
    {
        public long Id { get; protected set; }
        public EStatus Status { get; private set; } = EStatus.Active;

        public DateTime DateCreated { get; private set; }
        public long? UserIdCreated { get; private set; }
        public DateTime? DateModified { get; private set; }
        public long? UserIdModified { get; private set; }
        public DateTime? DateDeleted { get; private set; }
        public long? UserIdDeleted { get; private set; }

        public void SetCreationAudit(long? userId)
        {
            DateCreated = DateTime.UtcNow;
            UserIdCreated = userId;
            Status = EStatus.Active;
        }

        public void SetModificationAudit(long? userId)
        {
            DateModified = DateTime.UtcNow;
            UserIdModified = userId;
        }

        public void SetDeletionAudit(long? userId)
        {
            DateDeleted = DateTime.UtcNow;
            UserIdDeleted = userId;
            Status = EStatus.Inactive;
        }

    }
}
