using System.Linq.Expressions;
using July_Team.Models;
using Microsoft.EntityFrameworkCore;
using Task = System.Threading.Tasks.Task;

namespace July_Team.Repositories
{
    /// <summary>
    /// Generic repository implementation providing common CRUD operations.
    /// Encapsulates Entity Framework Core database access logic.
    /// Benefits:
    /// - Reduces code duplication across controllers
    /// - Makes unit testing easier by allowing mock repositories
    /// - Centralizes data access logic for consistent behavior
    /// </summary>
    /// <typeparam name="T">The entity type this repository manages</typeparam>
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        /// <summary>
        /// Gets all entities. Uses AsNoTracking() for better performance
        /// since we're only reading data, not modifying it.
        /// </summary>
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.AsNoTracking().ToListAsync();
        }

        /// <summary>
        /// Gets a single entity by ID. Returns null if not found.
        /// </summary>
        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        /// <summary>
        /// Finds entities matching the given condition.
        /// Example: FindAsync(p => p.Price > 100)
        /// </summary>
        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        /// <summary>
        /// Adds a new entity to the context (pending save).
        /// </summary>
        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        /// <summary>
        /// Adds multiple entities in a single database round-trip.
        /// More efficient than adding one at a time.
        /// </summary>
        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }

        /// <summary>
        /// Marks an entity as modified for update.
        /// </summary>
        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        /// <summary>
        /// Marks an entity for deletion.
        /// </summary>
        public void Remove(T entity)
        {
            _dbSet.Remove(entity);
        }

        /// <summary>
        /// Persists all pending changes to the database.
        /// Returns the number of affected rows.
        /// </summary>
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
