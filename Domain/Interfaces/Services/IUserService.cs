using SNI_Events.Application.Dtos.Shared;
using SNI_Events.Application.Dtos.User;

namespace SNI_Events.Domain.Interfaces.Services
{
    public interface IUserService
    {
        Task<UserDto> CreateAsync(string name, string email, string password, string phone, string cpf);
        Task<UserDto> UpdateAsync(long id, string name, string email, string phone, string cpf);
        Task DeleteAsync(long id);
        Task<UserDto?> GetByIdAsync(long id);
        Task<PagedResultDto<UserDto>> GetPagedAsync(UserFilterDto filter);
        Task<UserDto> ChangePasswordAsync(long id, string password);
        Task<List<UserDto>> GetAllAsync(UserFilterDto filter);
    }
}
