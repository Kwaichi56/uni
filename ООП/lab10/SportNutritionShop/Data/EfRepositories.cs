using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace SportNutritionShop.Data;

internal class EfRepository<T>(ShopDbContext context) : IRepository<T> where T : class
{
    protected readonly DbSet<T> Set = context.Set<T>();

    public Task<List<T>> GetAllAsync() => Set.AsNoTracking().ToListAsync();
    public Task<T?> GetByIdAsync(int id) => Set.FindAsync(id).AsTask();
    public Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate) =>
        Set.FirstOrDefaultAsync(predicate);
    public T? FirstOrDefault(Expression<Func<T, bool>> predicate) => Set.FirstOrDefault(predicate);
    public Task<bool> AnyAsync(Expression<Func<T, bool>> predicate) => Set.AnyAsync(predicate);
    public void Add(T entity) => Set.Add(entity);
    public void Remove(T entity) => Set.Remove(entity);
}

internal sealed class EfProductRepository(ShopDbContext context) : EfRepository<ProductEntity>(context), IProductRepository
{
    public Task<List<ProductEntity>> GetCatalogAsync() => Set.AsNoTracking()
        .Include(p => p.Category).OrderBy(p => p.ShortName).ToListAsync();

    public Task<List<ProductEntity>> SearchAsync(string? term, string field, bool descending)
    {
        IQueryable<ProductEntity> query = Set.AsNoTracking().Include(p => p.Category);
        if (!string.IsNullOrWhiteSpace(term))
        {
            var pattern = "%" + term.Trim() + "%";
            query = field switch
            {
                "ShortName" => query.Where(p => EF.Functions.Like(p.ShortName, pattern)),
                "Category" => query.Where(p => EF.Functions.Like(p.Category.Name, pattern)),
                "Brand" => query.Where(p => EF.Functions.Like(p.Brand, pattern)),
                "All" => query.Where(p => EF.Functions.Like(p.ShortName, pattern)
                    || EF.Functions.Like(p.FullName, pattern)
                    || EF.Functions.Like(p.Brand, pattern)
                    || EF.Functions.Like(p.Category.Name, pattern)),
                _ => throw new ArgumentException("Недопустимое поле поиска.", nameof(field))
            };
        }
        else if (field is not ("ShortName" or "Category" or "Brand" or "All"))
            throw new ArgumentException("Недопустимое поле поиска.", nameof(field));
        query = descending ? query.OrderByDescending(p => p.Price) : query.OrderBy(p => p.Price);
        return query.ToListAsync();
    }

    public Task<List<ProductEntity>> GetLowStockAsync(int limit) => Set.AsNoTracking()
        .Where(p => p.Quantity <= limit)
        .OrderBy(p => p.Quantity).ThenBy(p => p.ShortName).ToListAsync();
}

internal sealed class EfCategoryRepository(ShopDbContext context) : EfRepository<CategoryEntity>(context), ICategoryRepository
{
    public Task<CategoryEntity?> GetByNameAsync(string name)
    {
        var local = Set.Local.FirstOrDefault(c => c.Name == name);
        return local is null ? Set.SingleOrDefaultAsync(c => c.Name == name) : Task.FromResult<CategoryEntity?>(local);
    }

    public async Task<List<CategoryStatistics>> GetStatisticsAsync()
    {
        var rows = await Set.AsNoTracking().Select(c => new
        {
            Category = c.Name,
            ProductCount = c.Products.Count,
            AveragePrice = c.Products.Select(p => (double?)p.Price).Average(),
            Stock = c.Products.Sum(p => p.Quantity)
        }).OrderBy(x => x.Category).ToListAsync();
        return rows.Select(x => new CategoryStatistics(x.Category, x.ProductCount, x.AveragePrice, x.Stock)).ToList();
    }
}
