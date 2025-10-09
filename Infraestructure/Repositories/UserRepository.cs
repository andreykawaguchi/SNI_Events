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
            return await DbSet.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<bool> ExistsByCpfAsync(string cpf)
        {
            return await DbSet.AnyAsync(u => u.CPF == cpf);
        }

        public async Task<List<User>> GetAllAsync(UserFilterDto filter)
        {
            var query = FilterBy(filter);

            var users = await query
                .OrderBy(u => u.Name)
                .ToListAsync();

            return users;
        }

        public async Task<PagedResultDto<User>> GetPagedAsync(UserFilterDto filter)
        {
            var query = FilterBy(filter);

            var totalCount = await query.CountAsync();

            var users = await query
                .OrderBy(u => u.Name)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new PagedResultDto<User>(users, totalCount, filter.PageNumber, filter.PageSize);
        }

        private IQueryable<User> FilterBy(UserFilterDto filter)
        {
            var query = _vSNIContext.Users.AsQueryable();

            if (!string.IsNullOrEmpty(filter.Name))
                query = query.Where(u => u.Name.Contains(filter.Name));

            if (!string.IsNullOrEmpty(filter.Email))
                query = query.Where(u => u.Email.Contains(filter.Email));

            return query;
        }
    }
}
