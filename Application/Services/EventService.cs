using AutoMapper;
using SNI_Events.Application.Dtos.Event;
using SNI_Events.Domain.Entities;
using SNI_Events.Domain.Interfaces.Repositories;
using SNI_Events.Domain.Interfaces.Services;

namespace SNI_Events.Application.Services
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _repository;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUser;

        public EventService(IEventRepository repository, IMapper mapper, ICurrentUserService currentUser)
        {
            _repository = repository;
            _mapper = mapper;
            _currentUser = currentUser;
        }

        public async Task<IEnumerable<EventDto>> GetAllAsync()
        {
            var events = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<EventDto>>(events);
        }

        public async Task<EventDto> GetByIdAsync(long id)
        {
            var ev = await _repository.GetByIdAsync(id);
            return _mapper.Map<EventDto>(ev);
        }

        public async Task<EventDto> CreateAsync(EventCreateRequestDto dto)
        {
            var ev = new Event(dto.Name, _currentUser.UserId);
            await _repository.AddAsync(ev);
            return _mapper.Map<EventDto>(ev);
        }

        public async Task<EventDto> UpdateAsync(long id, EventUpdateRequestDto dto)
        {
            var ev = await _repository.GetByIdAsync(id)
                     ?? throw new Exception("Evento não encontrado");

            ev.Update(dto.Name, _currentUser.UserId);
            await _repository.UpdateAsync(ev);

            return _mapper.Map<EventDto>(ev);
        }

        public async Task DeleteAsync(long id)
        {
            var ev = await _repository.GetByIdAsync(id)
                     ?? throw new Exception("Evento não encontrado");

            ev.DeleteUser(_currentUser.UserId); // ou SetDeletionAudit
            await _repository.UpdateAsync(ev);
        }
    }
}
