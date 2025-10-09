using AutoMapper;
using SNI_Events.Application.Dtos.Shared;
using SNI_Events.Application.Dtos.User;
using SNI_Events.Domain.Entities;
using SNI_Events.Domain.Interfaces.Repositories;
using SNI_Events.Domain.Interfaces.Services;

namespace SNI_Events.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ICurrentUserService _currentUser;
        private readonly IMapper _mapper;
        public UserService(IUserRepository userRepository, ICurrentUserService currentUser, IMapper mapper)
        {
            _userRepository = userRepository;
            _currentUser = currentUser;
            _mapper = mapper;
        }

        public async Task<User> CreateAsync(string name, string email, string password, string phone, string cpf)
        {
            if (await _userRepository.ExistsByCpfAsync(cpf))
                throw new InvalidOperationException("CPF já cadastrado.");

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
            var user = new User(name, email, passwordHash, phone, cpf, _currentUser.UserId);

            var emailExists = await _userRepository.GetByEmailAsync(email);
            if (emailExists != null)
                throw new InvalidOperationException("E-mail já cadastrado.");

            return await _userRepository.AddAsync(user);
        }

        public async Task<User> UpdateAsync(long id, string name, string email, string phone, string cpf)
        {
            var user = await _userRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException("Usuário não encontrado.");

            if (await _userRepository.ExistsByCpfAsync(cpf) && user.CPF != cpf)
                throw new InvalidOperationException("CPF já cadastrado.");

            if (await _userRepository.GetByEmailAsync(email) != null && user.Email != email)
                throw new InvalidOperationException("E-mail já cadastrado.");

            if (user.Email != email)
                throw new InvalidOperationException("E-mail não pode ser alterado.");

            //var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

            user.Update(name, email, phone, cpf, _currentUser.UserId);
            return await _userRepository.UpdateAsync(user);
        }

        public async Task<User> ChangePasswordAsync(long id, string password)
        {
            var user = await _userRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException("Usuário não encontrado.");

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

            user.ChangePassword(passwordHash, _currentUser.UserId);
            return await _userRepository.UpdateAsync(user);
        }

        public async Task DeleteAsync(long id)
        {
            var user = await _userRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException("Usuário não encontrado.");
            user.DeleteUser(_currentUser.UserId);
            await _userRepository.DeleteAsync(user);
        }

        public async Task<User?> GetByIdAsync(long id)
        {
            return await _userRepository.GetByIdAsync(id);
        }

        public async Task<List<UserDto>> GetAllAsync(UserFilterDto filter)
        {
            var list = await _userRepository.GetAllAsync(filter);
            var mapped = _mapper.Map<List<UserDto>>(list);

            return mapped;
        }

        public async Task<PagedResultDto<UserDto>> GetPagedAsync(UserFilterDto filter)
        {
            var pagedUsers = await _userRepository.GetPagedAsync(filter);
            var mapped = _mapper.Map<IEnumerable<UserDto>>(pagedUsers.Items);

            return new PagedResultDto<UserDto>(mapped, pagedUsers.TotalCount, filter.PageNumber, filter.PageSize);
        }
    }
}
