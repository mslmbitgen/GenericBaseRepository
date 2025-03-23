using GenericBaseRepository.Entities;
using System.Linq.Expressions;
namespace GenericBaseRepository;
public interface IBaseRepository<TEntity, TEntityId> where TEntity : EntityBase<TEntityId>
{
    // CRUD işlemleri
    Task<TEntity> GetByIdAsync(TEntityId id); // ID'ye göre bir kaydı al
    IQueryable<TEntity> GetAll(); // Tüm aktif kayıtları al
    IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> predicate); // Belirli bir koşula göre kayıtları bul
    Task<TEntity> AddAsync(TEntity entity); // Yeni bir kayıt ekle
    Task<TEntity> UpdateAsync(TEntity entity); // Mevcut bir kaydı güncelle
    Task<TEntity> SoftDeleteAsync(TEntity entity, Guid deletedBy); // Soft delete işlemi
    Task<TEntity> RestoreAsync(TEntity entity); // Soft delete işlemi geri al (Restore)
    Task<bool> ExistsAsync(TEntityId id); // Bir kaydın var olup olmadığını kontrol et
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate); // Bir koşul sağlandığında herhangi bir kayıt olup olmadığını kontrol et

    // Dinamik özellikler
    Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate); // İlk kayıt veya null döner
    Task<PagedResult<TEntity>> GetPagedAsync(Expression<Func<TEntity, bool>> predicate, int page, int pageSize); // Sayfalama ile kayıtları getir
    Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate); // Belirli bir koşula göre kayıt sayısını al

    // Sıralama, Eager Loading ve İleri Düzey Sorgulama
    IQueryable<TEntity> GetAll(
        Expression<Func<TEntity, bool>>? filter = null, // Filtreleme
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, // Sıralama
        string? includeProperties = "" // Eager loading
    );

    // İstatistiksel hesaplamalar
    Task<TProperty> MaxAsync<TProperty>(Expression<Func<TEntity, TProperty>> selector); // Maksimum değer
    Task<TProperty> MinAsync<TProperty>(Expression<Func<TEntity, TProperty>> selector); // Minimum değer
    Task<decimal> SumAsync(Expression<Func<TEntity, decimal>> selector); // Toplam
    Task<decimal> AverageAsync(Expression<Func<TEntity, decimal>> selector); // Ortalama

    // Raw SQL Sorguları
    Task<int> ExecuteSqlRawAsync(string sql, params object[] parameters); // Özel SQL sorgusu çalıştır

    // Önbellekleme desteği
    IQueryable<TEntity> GetAllWithCache(
        Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        string? includeProperties = ""
    );

    // Toplu işlemler (bulk operations)
    Task<int> BulkInsertAsync(IEnumerable<TEntity> entities); // Toplu ekleme
    Task<int> BulkUpdateAsync(IEnumerable<TEntity> entities); // Toplu güncelleme
}
