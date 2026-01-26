using System.Linq.Expressions;
using Task = System.Threading.Tasks.Task;

namespace July_Team.Repositories
{
    /// <summary>
    /// Generic repository interface that defines common data access operations.
    /// This pattern abstracts the data layer and promotes testability.
    /// </summary>
    /// <typeparam name="T">The entity type this repository manages</typeparam>
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Retrieves all entities of type T from the database.
        /// Uses AsNoTracking for read-only performance optimization.
        /// </summary>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Retrieves a single entity by its primary key.
        /// </summary>
        /// <param name="id">The primary key value</param>
        Task<T?> GetByIdAsync(int id);

        /// <summary>
        /// Finds entities matching a specified predicate.
        /// </summary>
        /// <param name="predicate">Lambda expression for filtering</param>
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Adds a new entity to the database.
        /// </summary>
        /// <param name="entity">The entity to add</param>
        Task AddAsync(T entity);

        /// <summary>
        /// Adds multiple entities to the database in a single operation.
        /// </summary>
        /// <param name="entities">Collection of entities to add</param>
        Task AddRangeAsync(IEnumerable<T> entities);

        /// <summary>
        /// Updates an existing entity in the database.
        /// </summary>
        /// <param name="entity">The entity with updated values</param>
        void Update(T entity);

        /// <summary>
        /// Removes an entity from the database.
        /// </summary>
        /// <param name="entity">The entity to remove</param>
        void Remove(T entity);

        /// <summary>
        /// Saves all pending changes to the database.
        /// </summary>
        Task<int> SaveChangesAsync();
    }
}
