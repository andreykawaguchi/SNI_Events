using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SNI_Events.Domain.Entities.Base;
using SNI_Events.Domain.Enum;
using SNI_Events.Domain.Interfaces.Repositories.Base;
using SNI_Events.Domain.Interfaces.Services;
using SNI_Events.Infraestructure.Context;
using System.Linq.Expressions;

namespace SNI_Events.Infraestructure.Repository.Base;

public abstract class BaseRepository<TEntity> : IRepositoryBase<TEntity> where TEntity : EntityBase
{
    protected readonly SNIContext _vSNIContext;
    protected readonly DbSet<TEntity> DbSet;
    private readonly ICurrentUserService _currentUserService;

    public BaseRepository(SNIContext pSNIContext, ICurrentUserService currentUserService)
    {
        _vSNIContext = pSNIContext;
        DbSet = pSNIContext.Set<TEntity>();
        _currentUserService = currentUserService;
    }

    public virtual async Task<TEntity> AddAsync(TEntity entity)
    {
        _ = entity ?? throw new ArgumentNullException(nameof(entity));

        await DbSet.AddAsync(entity);
        await _vSNIContext.SaveChangesAsync();
        return entity;
    }

    public async Task<TEntity> DeleteAsync(TEntity entity)
    {
        _ = entity ?? throw new ArgumentNullException(nameof(entity));

        if (entity is EntityBase)
        {
            entity.SetDeletionAudit(_currentUserService.UserId);
            _vSNIContext.Entry(entity).State = EntityState.Modified;
        }
        else
        {
            throw new InvalidOperationException("A entidade não herda de EntityBase e não pode ser auditada.");
        }

        await _vSNIContext.SaveChangesAsync();
        return entity;
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await DbSet.AsNoTracking().Where(predicate).ToListAsync();
    }

    public async Task<TEntity?> GetByIdAsync(long Id)
    {
        var result = await _vSNIContext.Set<TEntity>().FindAsync(Id);
        return result;
    }

    public async Task<TEntity> UpdateAsync(TEntity entity)
    {
        try
        {
            _ = entity ?? throw new ArgumentNullException(nameof(entity));

            if (entity is not EntityBase entityBase)
            {
                throw new InvalidOperationException("A entidade não herda de EntityBase.");
            }

            DetachLocalIfTracked(entityBase.Id);

            _vSNIContext.Entry(entity).State = EntityState.Modified;
            await _vSNIContext.SaveChangesAsync();

            return entity;
        }
        catch
        {
            throw; // preserva stack trace original
        }
    }

    private void DetachLocalIfTracked(long id)
    {
        var local = _vSNIContext.Set<TEntity>()
                                .Local
                                .FirstOrDefault(entry =>
                                    entry is EntityBase eb && eb.Id == id);

        if (local != null)
        {
            _vSNIContext.Entry(local).State = EntityState.Detached;
        }
    }
}
