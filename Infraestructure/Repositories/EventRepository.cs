using Microsoft.EntityFrameworkCore;
using SNI_Events.Domain.Entities;
using SNI_Events.Domain.Interfaces.Repositories;
using SNI_Events.Domain.Interfaces.Services;
using SNI_Events.Infraestructure.Context;
using SNI_Events.Infraestructure.Repository.Base;

namespace SNI_Events.Infraestructure.Repository
{
    public class EventRepository : BaseRepository<Event>, IEventRepository
    {
        public EventRepository(SNIContext context, ICurrentUserService currentUserService)
     : base(context, currentUserService) { }

        // Use base implementation from BaseRepository<Event>
    }
}
