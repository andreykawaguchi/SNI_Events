using Microsoft.EntityFrameworkCore;
using SNI_Events.Application.Dtos.Shared;
using SNI_Events.Application.Dtos.User;
using SNI_Events.Domain.Entities;
using SNI_Events.Domain.Interfaces.Repositories;
using SNI_Events.Domain.Interfaces.Services;
using SNI_Events.Infraestructure.Context;
using SNI_Events.Infraestructure.Repository.Base;

namespace SNI_Events.Infraestructure.Repository
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(SNIContext context, ICurrentUserService currentUserService)
        : base(context, currentUserService) { }

        public async Task<User?> GetByEmailAsync(string email)
        {
            var users = await DbSet.AsNoTracking().ToListAsync();
            return users.FirstOrDefault(u => u.Email.Address == email);
        }

        public async Task<bool> ExistsByCpfAsync(string cpf)
        {
            var users = await DbSet.AsNoTracking().ToListAsync();
            return users.Any(u => u.Cpf.Number == cpf);
        }

        public async Task<List<User>> GetAllAsync(UserFilterDto filter)
        {
            var query = _vSNIContext.Users.AsNoTracking();

            if (!string.IsNullOrEmpty(filter.Name))
                query = query.Where(u => u.Name.Contains(filter.Name));

            // For Email filtering, we need to bring to memory first
            var users = await query.ToListAsync();

            if (!string.IsNullOrEmpty(filter.Email))
                users = users.Where(u => u.Email.Address.Contains(filter.Email)).ToList();

            return users.OrderBy(u => u.Name).ToList();
        }

        public async Task<PagedResultDto<User>> GetPagedAsync(UserFilterDto filter)
        {
            var query = _vSNIContext.Users.AsNoTracking();

            if (!string.IsNullOrEmpty(filter.Name))
                query = query.Where(u => u.Name.Contains(filter.Name));

            // Load all matching users first
            var allUsers = await query.ToListAsync();

            // Apply email filter in-memory
            if (!string.IsNullOrEmpty(filter.Email))
                allUsers = allUsers.Where(u => u.Email.Address.Contains(filter.Email)).ToList();

            var totalCount = allUsers.Count;

            // Then apply pagination
            var users = allUsers
                        .OrderBy(u => u.Name)
                        .Skip((filter.PageNumber - 1) * filter.PageSize)
                        .Take(filter.PageSize)
                        .ToList();

            return new PagedResultDto<User>(users, totalCount, filter.PageNumber, filter.PageSize);
        }
    }
}
