using SNI_Events.Domain.Entities;
using SNI_Events.Domain.Interfaces.Repositories.Base;

namespace SNI_Events.Domain.Interfaces.Repositories;
public interface IDinnerRepository : IRepositoryBase<Dinner>
{
    Task<IEnumerable<Dinner>> GetAllAsync();
    //Task<Event?> GetByIdAsync(long id);
    //Task AddAsync(Event entity);
    //Task UpdateAsync(Event entity);
}
