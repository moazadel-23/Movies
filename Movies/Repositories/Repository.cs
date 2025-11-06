using Microsoft.EntityFrameworkCore;
using Movies.Repositories.IRepository;
using System.Linq.Expressions;

namespace Movies.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly DbSet<T> dbSet;

        public Repository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            dbSet = _dbContext.Set<T>();
        }

        public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await dbSet.AddAsync(entity, cancellationToken);
        }

        public void Update(T entity)
        {
            dbSet.Update(entity);
        }

        public void Delete(T entity)
        {
            dbSet.Remove(entity);
        }

        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<T>> GetAsync(
            Expression<Func<T, bool>>? expression = default,
            Expression<Func<T, object>>[]? include = null,
            bool traced = true,
            CancellationToken cancellationToken = default)
        {
            IQueryable<T> query = dbSet.AsQueryable();

            if (expression is not null)
                query = query.Where(expression);

            if (include is not null)
            {
                foreach (var includee in include)
                {
                    query = query.Include(includee);
                }
            }

            if (!traced)
                query = query.AsNoTracking();

            return await query.ToListAsync(cancellationToken);
        }

        public async Task<T?> GetOneAsync(
            Expression<Func<T, bool>> expression,
            Expression<Func<T, object>>[]? include = null,
            bool traced = true,
            CancellationToken cancellationToken = default)
        {
            var result = await GetAsync(expression, include, traced, cancellationToken);
            return result.FirstOrDefault();
        }
    }
}
