using SNI_Events.Domain.Entities;
using SNI_Events.Domain.Interfaces.Repositories.Base;

namespace SNI_Events.Domain.Interfaces.Repositories;
public interface IEventRepository : IRepositoryBase<Event>
{
    // Specific repository methods for Event can be added here
}
