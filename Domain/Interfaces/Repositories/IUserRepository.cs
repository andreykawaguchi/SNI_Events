using SNI_Events.Application.Dtos.Shared;
using SNI_Events.Application.Dtos.User;
using SNI_Events.Domain.Entities;
using SNI_Events.Domain.Interfaces.Repositories.Base;

namespace SNI_Events.Domain.Interfaces.Repositories
{
    public interface IUserRepository : IRepositoryBase<User>
    {
        Task<User?> GetByEmailAsync(string email);
        Task<bool> ExistsByCpfAsync(string cpf);
        Task<PagedResultDto<User>> GetPagedAsync(UserFilterDto filter);
        Task<List<User>> GetAllAsync(UserFilterDto filter);
    }
}
