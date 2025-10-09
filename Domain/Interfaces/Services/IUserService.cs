using SNI_Events.Application.Dtos.Shared;
using SNI_Events.Application.Dtos.User;
using SNI_Events.Domain.Entities;

namespace SNI_Events.Domain.Interfaces.Services
{
    public interface IUserService
    {
        Task<User> CreateAsync(string name, string email, string password, string phone, string cpf);
        Task<User> UpdateAsync(long id, string name, string email, string phone, string cpf);
        Task DeleteAsync(long id);
        Task<User?> GetByIdAsync(long id);
        Task<PagedResultDto<UserDto>> GetPagedAsync(UserFilterDto filter);
        Task<User> ChangePasswordAsync(long id, string password);
        Task<List<UserDto>> GetAllAsync(UserFilterDto filter);
    }
}
