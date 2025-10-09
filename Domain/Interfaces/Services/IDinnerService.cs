using SNI_Events.Application.Dtos.Dinner;
using SNI_Events.Application.Dtos.Event;

namespace SNI_Events.Domain.Interfaces.Services
{
    public interface IDinnerService
    {
        Task<IEnumerable<DinnerDto>> GetAllAsync();
        Task<DinnerDto> GetByIdAsync(long id);
        Task<DinnerDto> CreateAsync(DinnerCreateRequestDto dto);
        Task<DinnerDto> UpdateAsync(long id, DinnerUpdateRequestDto dto);
        //Task DeleteAsync(long id);
    }
}
