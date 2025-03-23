# MB.GenericBaseRepository NuGet Package

## Overview

`MB.GenericBaseRepository` is a reusable and customizable **Repository Pattern** implementation for **.NET Core / .NET 6+** applications. It offers a clean and maintainable approach to handle database operations with **Entity Framework Core**. The package includes support for basic CRUD operations, pagination, soft deletes, caching, dynamic queries, and bulk operations.

## Dependency

This library is built for **.NET 9.0** and works with **Entity Framework Core**.

## Install

To integrate `MB.GenericBaseRepository` into your project, install it via the **NuGet package manager**:

```bash

 dotnet add package MB.GenericBaseRepository
 dotnet add package Microsoft.EntityFrameworkCore
 dotnet add package EFCore.BulkExtensions

```

## UnitOfWork Implementation
```CSharp

public class ApplicationDbContext : DbContext, IUnitOfWork
{
    public DbSet<YourEntity> YourEntities { get; set; }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await base.SaveChangesAsync(cancellationToken);
    }
}

```

## Create Repository
```CSharp
public interface IUserRepository : IBaseRepository<User>
{
    // Custom methods for UserRepository (optional)
}

public class UserRepository : BaseRepository<User, ApplicationDbContext>, IUserRepository
{
    public UserRepository(ApplicationDbContext context, IMemoryCache cache, ILogger<UserRepository> logger)
        : base(context, cache, logger)
    {
    }
}
```

## Use in Services
```CSharp

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UserService(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken)
    {
        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _userRepository.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<IList<User>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _userRepository.GetAll().ToListAsync(cancellationToken);
    }
}

```

## Dependency Injection
```CSharp

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUnitOfWork>(srv => srv.GetRequiredService<ApplicationDbContext>());

```

## Methods
This library have two services.
IRepository, IUnitOfWork

```Csharp

    // Basic CRUD Operations:

    Task<TEntity> GetByIdAsync(TEntityId id); // Retrieve a record by its ID.
    IQueryable<TEntity> GetAll(); // Retrieve all active records.
    IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> predicate); // Find records based on a specific condition
    Task<TEntity> AddAsync(TEntity entity); // Add a new record.
    Task<TEntity> UpdateAsync(TEntity entity); // Update an existing record.
    Task<TEntity> SoftDeleteAsync(TEntity entity, Guid deletedBy); // Perform a soft delete operation.
    Task<TEntity> RestoreAsync(TEntity entity); // Restore a soft-deleted record.
    Task<bool> ExistsAsync(TEntityId id); // Check if a record exists by its ID.
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate); // Check if any record exists that satisfies a given condition.

    // Dynamic Features
    Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate); // Return the first record or null based on a condition.
    Task<PagedResult<TEntity>> GetPagedAsync(Expression<Func<TEntity, bool>> predicate, int page, int pageSize); // Retrieve records with pagination support.
    Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate); // Get the number of records that satisfy a given condition.

    // Sorting, Eager Loading, and Advanced Queries
    IQueryable<TEntity> GetAll(
        Expression<Func<TEntity, bool>>? filter = null, // Filter
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, // Sort
        string? includeProperties = "" // Eager loading
    );//Retrieve all records with optional filtering, ordering, and eager loading support.

    // Statistical Calculations
    Task<TProperty> MaxAsync<TProperty>(Expression<Func<TEntity, TProperty>> selector); // Retrieve the maximum value of a given property.
    Task<TProperty> MinAsync<TProperty>(Expression<Func<TEntity, TProperty>> selector); // Retrieve the minimum value of a given property.
    Task<decimal> SumAsync(Expression<Func<TEntity, decimal>> selector); //  Retrieve the sum of a given property.
    Task<decimal> AverageAsync(Expression<Func<TEntity, decimal>> selector); //  Retrieve the average value of a given property.

    // Raw SQL Queries:
    Task<int> ExecuteSqlRawAsync(string sql, params object[] parameters); // Execute a custom raw SQL query.

    // Caching Support
    IQueryable<TEntity> GetAllWithCache(
        Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        string? includeProperties = ""
    );  //Retrieve records with caching support for improved performance.

    // Bulk Operations
    Task<int> BulkInsertAsync(IEnumerable<TEntity> entities); // Insert multiple records at once.
    Task<int> BulkUpdateAsync(IEnumerable<TEntity> entities); // Update multiple records at once.

```
