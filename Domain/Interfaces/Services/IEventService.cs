using SNI_Events.Application.Dtos.Event;

namespace SNI_Events.Domain.Interfaces.Services
{
    public interface IEventService
    {
        Task<IEnumerable<EventDto>> GetAllAsync();
        Task<EventDto> GetByIdAsync(long id);
        Task<EventDto> CreateAsync(EventCreateRequestDto dto);
        Task<EventDto> UpdateAsync(long id, EventUpdateRequestDto dto);
        Task DeleteAsync(long id);
    }
}
