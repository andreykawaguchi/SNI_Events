using AutoMapper;
using SNI_Events.Application.Dtos.Shared;
using SNI_Events.Application.Dtos.User;
using SNI_Events.Domain.Entities;
using SNI_Events.Domain.Interfaces.Repositories;
using SNI_Events.Domain.Interfaces.Services;
using SNI_Events.Domain.Exceptions;
using SNI_Events.Application.UseCases.User;

namespace SNI_Events.Application.Services
{
    /// <summary>
    /// Application Service responsável pela coordenação de Use Cases de usuário
    /// Implementa a interface IUserService e delega para Use Cases
    /// </summary>
    public class UserService : IUserService
    {
        private readonly CreateUserUseCase _createUserUseCase;
        private readonly UpdateUserUseCase _updateUserUseCase;
        private readonly ChangePasswordUseCase _changePasswordUseCase;
        private readonly DeleteUserUseCase _deleteUserUseCase;
        private readonly GetUserByIdUseCase _getUserByIdUseCase;
        private readonly GetPagedUsersUseCase _getPagedUsersUseCase;
        private readonly ICurrentUserService _currentUser;
        private readonly IMapper _mapper;

        public UserService(
            CreateUserUseCase createUserUseCase,
            UpdateUserUseCase updateUserUseCase,
            ChangePasswordUseCase changePasswordUseCase,
            DeleteUserUseCase deleteUserUseCase,
            GetUserByIdUseCase getUserByIdUseCase,
            GetPagedUsersUseCase getPagedUsersUseCase,
            ICurrentUserService currentUser,
            IMapper mapper)
        {
            _createUserUseCase = createUserUseCase ?? throw new ArgumentNullException(nameof(createUserUseCase));
            _updateUserUseCase = updateUserUseCase ?? throw new ArgumentNullException(nameof(updateUserUseCase));
            _changePasswordUseCase = changePasswordUseCase ?? throw new ArgumentNullException(nameof(changePasswordUseCase));
            _deleteUserUseCase = deleteUserUseCase ?? throw new ArgumentNullException(nameof(deleteUserUseCase));
            _getUserByIdUseCase = getUserByIdUseCase ?? throw new ArgumentNullException(nameof(getUserByIdUseCase));
            _getPagedUsersUseCase = getPagedUsersUseCase ?? throw new ArgumentNullException(nameof(getPagedUsersUseCase));
            _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<UserDto> CreateAsync(string name, string email, string password, string phone, string cpf)
        {
            var request = new CreateUserRequest
            {
                Name = name,
                Email = email,
                Password = password,
                PhoneNumber = phone,
                CPF = cpf
            };

            var result = await _createUserUseCase.ExecuteAsync(request, _currentUser.UserId);
            
            if (!result.IsSuccess)
                throw new InvalidOperationException(result.Error);

            return _mapper.Map<UserDto>(result.Value);
        }

        public async Task<UserDto> UpdateAsync(long id, string name, string email, string phone, string cpf)
        {
            var request = new UpdateUserRequest
            {
                Name = name,
                PhoneNumber = phone,
                Role = "User"
            };

            var result = await _updateUserUseCase.ExecuteAsync(id, request, _currentUser.UserId);
            
            if (!result.IsSuccess)
                throw new InvalidOperationException(result.Error);

            return _mapper.Map<UserDto>(result.Value);
        }

        public async Task<UserDto> ChangePasswordAsync(long id, string password)
        {
            var result = await _changePasswordUseCase.ExecuteAsync(id, password, _currentUser.UserId);
            
            if (!result.IsSuccess)
                throw new InvalidOperationException(result.Error);

            var user = await _getUserByIdUseCase.ExecuteAsync(id);
            return _mapper.Map<UserDto>(user);
        }

        public async Task DeleteAsync(long id)
        {
            var result = await _deleteUserUseCase.ExecuteAsync(id, _currentUser.UserId);
            
            if (!result.IsSuccess)
                throw new InvalidOperationException(result.Error);
        }

        public async Task<UserDto?> GetByIdAsync(long id)
        {
            try
            {
                var user = await _getUserByIdUseCase.ExecuteAsync(id);
                return _mapper.Map<UserDto>(user);
            }
            catch (KeyNotFoundException)
            {
                return null;
            }
        }

        public async Task<List<UserDto>> GetAllAsync(UserFilterDto filter)
        {
            var pagedResult = await _getPagedUsersUseCase.ExecuteAsync(filter);
            return _mapper.Map<List<UserDto>>(pagedResult.Items);
        }

        public async Task<PagedResultDto<UserDto>> GetPagedAsync(UserFilterDto filter)
        {
            var pagedUsers = await _getPagedUsersUseCase.ExecuteAsync(filter);
            var mapped = _mapper.Map<IEnumerable<UserDto>>(pagedUsers.Items);

            return new PagedResultDto<UserDto>(mapped, pagedUsers.TotalCount, filter.PageNumber, filter.PageSize);
        }
    }
}
