using EFCore.BulkExtensions;
using GenericBaseRepository.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GenericBaseRepository;

public class BaseRepository<TEntity, TEntityId> : IBaseRepository<TEntity, TEntityId> where TEntity : EntityBase<TEntityId>
{
    private readonly DbContext _context;
    private readonly DbSet<TEntity> _dbSet;
    private readonly IMemoryCache _cache;
    private readonly ILogger<BaseRepository<TEntity, TEntityId>> _logger;

    public BaseRepository(DbContext context, IMemoryCache cache, ILogger<BaseRepository<TEntity, TEntityId>> logger)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
        _cache = cache;
        _logger = logger;
    }

    // CRUD işlemleri
    public async Task<TEntity> GetByIdAsync(TEntityId id)
    {
        return await _dbSet.FirstOrDefaultAsync(e => e.Id.Equals(id) && e.IsActive);
    }

    public IQueryable<TEntity> GetAll()
    {
        return _dbSet.Where(e => e.IsActive);
    }

    public IQueryable<TEntity> GetAll(
        Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        string? includeProperties = ""
    )
    {
        var query = _dbSet.Where(e => e.IsActive);

        if (filter != null)
        {
            query = query.Where(filter);
        }

        if (!string.IsNullOrEmpty(includeProperties))
        {
            foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }
        }

        if (orderBy != null)
        {
            query = orderBy(query);
        }

        return query;
    }
    public IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
    {
        return _dbSet.Where(predicate).Where(e => e.IsActive);
    }

    public async Task<TEntity> AddAsync(TEntity entity)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            _logger.LogInformation($"Adding new entity {typeof(TEntity).Name}");
            entity.CreatedAt = DateTime.UtcNow;
            _dbSet.Add(entity);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while adding entity");
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<TEntity> UpdateAsync(TEntity entity)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            _logger.LogInformation($"Updating entity {typeof(TEntity).Name} with ID {entity.Id}");
            entity.ModifiedAt = DateTime.UtcNow;
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while updating entity");
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<TEntity> SoftDeleteAsync(TEntity entity, Guid deletedBy)
    {
        entity.DeletedAt = DateTime.UtcNow;
        entity.DeletedBy = deletedBy;
        entity.IsActive = false;
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<TEntity> RestoreAsync(TEntity entity)
    {
        if (entity.DeletedAt == null || entity.IsActive)
            return entity;

        entity.DeletedAt = null;
        entity.DeletedBy = null;
        entity.IsActive = true;
        entity.ModifiedAt = DateTime.UtcNow;

        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> ExistsAsync(TEntityId id)
    {
        return await _dbSet.AnyAsync(e => e.Id.Equals(id) && e.IsActive);
    }

    public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await _dbSet.AnyAsync(predicate);
    }

    public async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await _dbSet.Where(predicate).FirstOrDefaultAsync();
    }

    public async Task<PagedResult<TEntity>> GetPagedAsync(Expression<Func<TEntity, bool>> predicate, int page, int pageSize)
    {
        var totalItems = await _dbSet.CountAsync(predicate);
        var items = await _dbSet
            .Where(predicate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<TEntity>
        {
            TotalCount = totalItems,
            Items = items
        };
    }

    public async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await _dbSet.CountAsync(predicate);
    }

    // Önbellekleme ile GetAll metodunu geliştirme
    public IQueryable<TEntity> GetAllWithCache(
        Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        string? includeProperties = ""
    )
    {
        var cacheKey = $"{typeof(TEntity).Name}_GetAll_WithCache";
        if (!_cache.TryGetValue(cacheKey, out IQueryable<TEntity> entities))
        {
            entities = _dbSet.Where(e => e.IsActive);

            // Eager loading (ilişkili veriler)
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    entities = entities.Include(includeProperty);
                }
            }

            // Filtreleme ve sıralama
            if (filter != null) entities = entities.Where(filter);
            if (orderBy != null) entities = orderBy(entities);

            _cache.Set(cacheKey, entities, TimeSpan.FromMinutes(5)); // 5 dakika boyunca önbellekte tut
        }

        return entities;
    }

    // Toplu işlemler
    public async Task<int> BulkInsertAsync(IEnumerable<TEntity> entities)
    {
        await _context.BulkInsertAsync(entities);
        return entities.Count();
    }

    public async Task<int> BulkUpdateAsync(IEnumerable<TEntity> entities)
    {
        await _context.BulkUpdateAsync(entities);
        return entities.Count();
    }

    // İstatistiksel hesaplamalar
    public async Task<TProperty> MaxAsync<TProperty>(Expression<Func<TEntity, TProperty>> selector)
    {
        return await _dbSet.MaxAsync(selector);
    }

    public async Task<TProperty> MinAsync<TProperty>(Expression<Func<TEntity, TProperty>> selector)
    {
        return await _dbSet.MinAsync(selector);
    }

    public async Task<decimal> SumAsync(Expression<Func<TEntity, decimal>> selector)
    {
        return await _dbSet.SumAsync(selector);
    }

    public async Task<decimal> AverageAsync(Expression<Func<TEntity, decimal>> selector)
    {
        return await _dbSet.AverageAsync(selector);
    }

    // Raw SQL Sorguları
    public async Task<int> ExecuteSqlRawAsync(string sql, params object[] parameters)
    {
        return await _context.Database.ExecuteSqlRawAsync(sql, parameters);
    }
}
