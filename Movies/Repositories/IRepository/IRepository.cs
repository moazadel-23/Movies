using System.Linq.Expressions;

namespace Movies.Repositories.IRepository
    {
        public interface IRepository<T> where T : class
        {
            Task AddAsync(T entity, CancellationToken cancellationToken = default);
            void Update(T entity);
            void Delete(T entity);
            Task CommitAsync(CancellationToken cancellationToken = default);

            Task<IEnumerable<T>> GetAsync(
                Expression<Func<T, bool>>? expression = default,
                Expression<Func<T, object>>[]? include = null,
                bool traced = true,
                CancellationToken cancellationToken = default);

            Task<T?> GetOneAsync(
                Expression<Func<T, bool>> expression,
                Expression<Func<T, object>>[]? include = null,
                bool traced = true,
                CancellationToken cancellationToken = default);
        }
    }





