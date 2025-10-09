using SNI_Events.Domain.Entities;
using SNI_Events.Domain.Interfaces.Repositories.Base;

namespace SNI_Events.Domain.Interfaces.Repositories;
public interface IEventRepository : IRepositoryBase<Event>
{
    Task<IEnumerable<Event>> GetAllAsync();
    //Task<Event?> GetByIdAsync(long id);
    //Task AddAsync(Event entity);
    //Task UpdateAsync(Event entity);
}
