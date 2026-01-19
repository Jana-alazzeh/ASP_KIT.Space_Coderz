using July_Team.Exceptions;
using July_Team.Helpers;
using July_Team.Models;
using July_Team.Repositories;
using Task = System.Threading.Tasks.Task;

namespace July_Team.Services
{
    /// <summary>
    /// Service layer for product business logic.
    /// Separates business rules from controllers, making the code more maintainable and testable.
    /// Controllers become thin - they only handle HTTP concerns, delegating logic to services.
    /// </summary>
    public class ProductService : IProductService
    {
        private readonly IRepository<Product> _productRepository;
        private const string SOURCE = "ProductService";

        public ProductService(IRepository<Product> productRepository)
        {
            _productRepository = productRepository;
        }

        /// <summary>
        /// Gets all products from the database.
        /// Logs the operation for monitoring and debugging.
        /// </summary>
        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            LoggingHelper.LogInfo(SOURCE, "Fetching all products...");
            
            try
            {
                var products = await _productRepository.GetAllAsync();
                LoggingHelper.LogSuccess(SOURCE, $"Retrieved {products.Count()} products");
                return products;
            }
            catch (Exception ex)
            {
                LoggingHelper.LogError(SOURCE, "Failed to retrieve products", ex);
                throw;
            }
        }

        /// <summary>
        /// Gets a single product by ID.
        /// Throws NotFoundException if product doesn't exist.
        /// </summary>
        public async Task<Product?> GetProductByIdAsync(int id)
        {
            LoggingHelper.LogInfo(SOURCE, $"Fetching product with ID: {id}");

            var product = await _productRepository.GetByIdAsync(id);
            
            if (product == null)
            {
                LoggingHelper.LogWarning(SOURCE, $"Product with ID {id} not found");
            }
            else
            {
                LoggingHelper.LogSuccess(SOURCE, $"Found product: {product.Name}");
            }

            return product;
        }

        /// <summary>
        /// Creates a new product with validation.
        /// Validates that required fields are present before saving.
        /// </summary>
        public async Task<Product> CreateProductAsync(Product product)
        {
            LoggingHelper.LogInfo(SOURCE, $"Creating new product: {product.Name}");

            try
            {
                ValidateProduct(product);

                await _productRepository.AddAsync(product);
                await _productRepository.SaveChangesAsync();

                LoggingHelper.LogSuccess(SOURCE, $"Product created with ID: {product.Id}");
                return product;
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                LoggingHelper.LogError(SOURCE, "Failed to create product", ex);
                throw new AppException($"Failed to create product: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates an existing product.
        /// Validates the update and handles concurrency issues.
        /// </summary>
        public async Task<Product> UpdateProductAsync(int id, Product product)
        {
            LoggingHelper.LogInfo(SOURCE, $"Updating product with ID: {id}");

            var existingProduct = await _productRepository.GetByIdAsync(id);
            
            if (existingProduct == null)
            {
                LoggingHelper.LogWarning(SOURCE, $"Product with ID {id} not found for update");
                throw new NotFoundException("Product", id);
            }

            try
            {
                existingProduct.Name = product.Name;
                existingProduct.Description = product.Description;
                existingProduct.Price = product.Price;
                existingProduct.Stock = product.Stock;
                existingProduct.ImageUrl = product.ImageUrl;
                existingProduct.ImageUrl_Back = product.ImageUrl_Back;

                _productRepository.Update(existingProduct);
                await _productRepository.SaveChangesAsync();

                LoggingHelper.LogSuccess(SOURCE, $"Product {id} updated successfully");
                return existingProduct;
            }
            catch (Exception ex)
            {
                LoggingHelper.LogError(SOURCE, $"Failed to update product {id}", ex);
                throw new AppException($"Failed to update product: {ex.Message}");
            }
        }

        /// <summary>
        /// Deletes a product by ID.
        /// Validates the product exists before deletion.
        /// </summary>
        public async Task DeleteProductAsync(int id)
        {
            LoggingHelper.LogInfo(SOURCE, $"Deleting product with ID: {id}");

            var product = await _productRepository.GetByIdAsync(id);
            
            if (product == null)
            {
                LoggingHelper.LogWarning(SOURCE, $"Product with ID {id} not found for deletion");
                throw new NotFoundException("Product", id);
            }

            try
            {
                _productRepository.Remove(product);
                await _productRepository.SaveChangesAsync();
                LoggingHelper.LogSuccess(SOURCE, $"Product {id} deleted successfully");
            }
            catch (Exception ex)
            {
                LoggingHelper.LogError(SOURCE, $"Failed to delete product {id}", ex);
                throw new AppException($"Failed to delete product: {ex.Message}");
            }
        }

        /// <summary>
        /// Checks if a product has enough stock for an order.
        /// Returns true if stock >= quantity, false otherwise.
        /// </summary>
        public async Task<bool> CheckStockAvailabilityAsync(int productId, int quantity)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            
            if (product == null)
            {
                LoggingHelper.LogWarning(SOURCE, $"Stock check failed: Product {productId} not found");
                return false;
            }

            var isAvailable = product.Stock >= quantity;
            LoggingHelper.LogInfo(SOURCE, $"Stock check for product {productId}: {product.Stock} available, {quantity} requested - {(isAvailable ? "OK" : "INSUFFICIENT")}");
            
            return isAvailable;
        }

        /// <summary>
        /// Reduces product stock after a successful order.
        /// Throws BusinessRuleException if insufficient stock.
        /// </summary>
        public async Task ReduceStockAsync(int productId, int quantity)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            
            if (product == null)
            {
                throw new NotFoundException("Product", productId);
            }

            if (product.Stock < quantity)
            {
                throw new BusinessRuleException($"Insufficient stock for product '{product.Name}'. Available: {product.Stock}, Requested: {quantity}");
            }

            product.Stock -= quantity;
            _productRepository.Update(product);
            await _productRepository.SaveChangesAsync();

            LoggingHelper.LogInfo(SOURCE, $"Reduced stock for product {productId} by {quantity}. New stock: {product.Stock}");
        }

        /// <summary>
        /// Validates product data before create/update.
        /// Throws ValidationException if validation fails.
        /// </summary>
        private void ValidateProduct(Product product)
        {
            var errors = new Dictionary<string, string[]>();

            if (string.IsNullOrWhiteSpace(product.Name))
            {
                errors["Name"] = new[] { "Product name is required" };
            }

            if (product.Price < 0)
            {
                errors["Price"] = new[] { "Price cannot be negative" };
            }

            if (product.Stock < 0)
            {
                errors["Stock"] = new[] { "Stock cannot be negative" };
            }

            if (errors.Any())
            {
                LoggingHelper.LogWarning(SOURCE, $"Product validation failed: {string.Join(", ", errors.Keys)}");
                throw new ValidationException(errors);
            }
        }
    }
}
