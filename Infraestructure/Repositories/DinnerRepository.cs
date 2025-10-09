using Microsoft.EntityFrameworkCore;
using SNI_Events.Domain.Entities;
using SNI_Events.Domain.Interfaces.Repositories;
using SNI_Events.Domain.Interfaces.Services;
using SNI_Events.Infraestructure.Context;
using SNI_Events.Infraestructure.Repository.Base;

namespace SNI_Events.Infraestructure.Repository
{
    public class DinnerRepository : BaseRepository<Dinner>, IDinnerRepository
    {
        public DinnerRepository(SNIContext context, ICurrentUserService currentUserService)
     : base(context, currentUserService) { }

        public async Task<IEnumerable<Dinner>> GetAllAsync()
            => await _vSNIContext.Dinners.ToListAsync();

        //public async Task<Event?> GetByIdAsync(long id)
        //    => await _vSNIContext.Events.FindAsync(id);

        //public async Task AddAsync(Event entity)
        //{
        //    await _vSNIContext.Events.AddAsync(entity);
        //    await _vSNIContext.SaveChangesAsync();
        //}

        //public async Task UpdateAsync(Event entity)
        //{
        //    _vSNIContext.Events.Update(entity);
        //    await _vSNIContext.SaveChangesAsync();
        //}
    }
}
