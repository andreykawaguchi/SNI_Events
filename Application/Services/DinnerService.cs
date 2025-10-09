using AutoMapper;
using SNI_Events.Application.Dtos.Dinner;
using SNI_Events.Application.Dtos.Event;
using SNI_Events.Domain.Entities;
using SNI_Events.Domain.Interfaces.Repositories;
using SNI_Events.Domain.Interfaces.Services;

public class DinnerService : IDinnerService
{
    private readonly IDinnerRepository _repository;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUser;

    public DinnerService(IDinnerRepository repository, IMapper mapper, ICurrentUserService currentUser)
    {
        _repository = repository;
        _mapper = mapper;
        _currentUser = currentUser;
    }

    public async Task<IEnumerable<DinnerDto>> GetAllAsync()
    {
        var events = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<DinnerDto>>(events);
    }

    public async Task<DinnerDto> GetByIdAsync(long id)
    {
        var ev = await _repository.GetByIdAsync(id);
        return _mapper.Map<DinnerDto>(ev);
    }

    public async Task<DinnerDto> CreateAsync(DinnerCreateRequestDto dto)
    {
        var ev = new Dinner(dto.Name, dto.Price ,_currentUser.UserId);
        await _repository.AddAsync(ev);
        return _mapper.Map<DinnerDto>(ev);
    }

    public async Task<DinnerDto> UpdateAsync(long id, DinnerUpdateRequestDto dto)
    {
        var ev = await _repository.GetByIdAsync(id)
                 ?? throw new Exception("Jantar não encontrado");

        ev.Update(dto.Name, dto.Price, _currentUser.UserId);
        await _repository.UpdateAsync(ev);

        return _mapper.Map<DinnerDto>(ev);
    }

    //public async Task DeleteAsync(long id)
    //{
    //    var ev = await _repository.GetByIdAsync(id)
    //             ?? throw new Exception("Jantar não encontrado");

    //    ev.RemoveUser(_currentUser.UserId); // ou SetDeletionAudit
    //    await _repository.UpdateAsync(ev);
    //}
}
