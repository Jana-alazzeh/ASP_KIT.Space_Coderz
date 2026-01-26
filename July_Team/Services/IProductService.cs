using July_Team.Models;
using Task = System.Threading.Tasks.Task;

namespace July_Team.Services
{
    /// <summary>
    /// Interface defining product-related business operations.
    /// Controllers depend on this interface, not the concrete implementation,
    /// following the Dependency Inversion Principle (DIP).
    /// </summary>
    public interface IProductService
    {
        /// <summary>
        /// Retrieves all products for display.
        /// </summary>
        Task<IEnumerable<Product>> GetAllProductsAsync();

        /// <summary>
        /// Gets a single product by its ID.
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <returns>Product or null if not found</returns>
        Task<Product?> GetProductByIdAsync(int id);

        /// <summary>
        /// Creates a new product with validation.
        /// </summary>
        /// <param name="product">Product to create</param>
        Task<Product> CreateProductAsync(Product product);

        /// <summary>
        /// Updates an existing product.
        /// </summary>
        /// <param name="id">Product ID to update</param>
        /// <param name="product">Updated product data</param>
        Task<Product> UpdateProductAsync(int id, Product product);

        /// <summary>
        /// Deletes a product by ID.
        /// </summary>
        /// <param name="id">Product ID to delete</param>
        Task DeleteProductAsync(int id);

        /// <summary>
        /// Checks if sufficient stock is available for a quantity.
        /// </summary>
        /// <param name="productId">Product ID</param>
        /// <param name="quantity">Requested quantity</param>
        Task<bool> CheckStockAvailabilityAsync(int productId, int quantity);

        /// <summary>
        /// Reduces product stock after a successful order.
        /// </summary>
        /// <param name="productId">Product ID</param>
        /// <param name="quantity">Quantity to reduce</param>
        Task ReduceStockAsync(int productId, int quantity);
    }
}
