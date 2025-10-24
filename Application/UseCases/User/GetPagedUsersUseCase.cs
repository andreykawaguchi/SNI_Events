using SNI_Events.Application.Dtos.Shared;
using SNI_Events.Application.Dtos.User;
using SNI_Events.Domain.Interfaces.Repositories;
using SNI_Events.Domain.Specifications;

namespace SNI_Events.Application.UseCases.User
{
    /// <summary>
    /// Use Case para recuperar usuários com paginação
    /// </summary>
    public class GetPagedUsersUseCase
    {
        private readonly IUserRepository _userRepository;

        public GetPagedUsersUseCase(IUserRepository userRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public async Task<PagedResultDto<Domain.Entities.User>> ExecuteAsync(UserFilterDto filter)
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));

            return await _userRepository.GetPagedAsync(filter);
        }
    }
}
