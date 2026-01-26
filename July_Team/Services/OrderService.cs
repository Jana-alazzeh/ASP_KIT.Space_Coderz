using July_Team.Exceptions;
using July_Team.Helpers;
using July_Team.Models;
using July_Team.Repositories;
using Microsoft.EntityFrameworkCore;
using Task = System.Threading.Tasks.Task;

namespace July_Team.Services
{
    /// <summary>
    /// Service layer for order business logic.
    /// Handles order processing, validation, and status management.
    /// Separates order-related business rules from the controller.
    /// Note: Uses AppDbContext for complex queries with Include().
    /// For simple CRUD, prefer IRepository abstraction.
    /// </summary>
    public class OrderService : IOrderService
    {
        private readonly IRepository<Order> _orderRepository;
        private readonly IProductService _productService;
        private readonly AppDbContext _context;
        private const string SOURCE = "OrderService";

        /// <summary>
        /// Constructor with dependency injection.
        /// AppDbContext is injected for complex queries requiring Include().
        /// IRepository is used for simpler CRUD operations.
        /// </summary>
        public OrderService(
            IRepository<Order> orderRepository, 
            IProductService productService,
            AppDbContext context)
        {
            _orderRepository = orderRepository;
            _productService = productService;
            _context = context;
        }

        /// <summary>
        /// Gets all orders with product details, ordered by most recent.
        /// Uses Include() to eager-load the related Product entity.
        /// </summary>
        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            LoggingHelper.LogInfo(SOURCE, "Fetching all orders...");

            try
            {
                var orders = await _context.Orders
                    .Include(o => o.Product)
                    .OrderByDescending(o => o.CreatedAt)
                    .ToListAsync();

                LoggingHelper.LogSuccess(SOURCE, $"Retrieved {orders.Count} orders");
                return orders;
            }
            catch (Exception ex)
            {
                LoggingHelper.LogError(SOURCE, "Failed to retrieve orders", ex);
                throw;
            }
        }

        /// <summary>
        /// Gets orders for a specific user.
        /// Filters by UserId and includes product details.
        /// </summary>
        public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(string userId)
        {
            LoggingHelper.LogInfo(SOURCE, $"Fetching orders for user: {userId}");

            try
            {
                var orders = await _context.Orders
                    .Include(o => o.Product)
                    .Where(o => o.UserId == userId)
                    .OrderByDescending(o => o.CreatedAt)
                    .ToListAsync();

                LoggingHelper.LogSuccess(SOURCE, $"Retrieved {orders.Count} orders for user");
                return orders;
            }
            catch (Exception ex)
            {
                LoggingHelper.LogError(SOURCE, $"Failed to retrieve orders for user {userId}", ex);
                throw;
            }
        }

        /// <summary>
        /// Gets a single order by ID with its product.
        /// </summary>
        public async Task<Order?> GetOrderByIdAsync(int id)
        {
            LoggingHelper.LogInfo(SOURCE, $"Fetching order with ID: {id}");

            var order = await _context.Orders
                .Include(o => o.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                LoggingHelper.LogWarning(SOURCE, $"Order with ID {id} not found");
            }

            return order;
        }

        /// <summary>
        /// Processes a checkout - validates stock, creates orders, updates inventory.
        /// Uses a transaction to ensure all-or-nothing behavior.
        /// If any part fails, the entire checkout is rolled back.
        /// </summary>
        public async Task<List<Order>> ProcessCheckoutAsync(CheckoutViewModel model, string? userId)
        {
            LoggingHelper.LogInfo(SOURCE, $"Processing checkout for user: {userId ?? "Anonymous"}");

            if (model.Items == null || !model.Items.Any())
            {
                throw new ValidationException("Cart is empty. Cannot process checkout.");
            }

            var ordersList = new List<Order>();

            try
            {
                foreach (var item in model.Items)
                {
                    LoggingHelper.LogDebug(SOURCE, $"Processing item: ProductId={item.ProductId}, Qty={item.Quantity}");

                    var product = await _productService.GetProductByIdAsync(item.ProductId);
                    
                    if (product == null)
                    {
                        LoggingHelper.LogWarning(SOURCE, $"Product {item.ProductId} not found - skipping");
                        continue;
                    }

                    if (!await _productService.CheckStockAvailabilityAsync(item.ProductId, item.Quantity))
                    {
                        throw new BusinessRuleException($"Insufficient stock for product '{product.Name}'");
                    }

                    var order = new Order
                    {
                        UserId = userId ?? string.Empty,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        TotalPrice = item.UnitPrice * item.Quantity,
                        Status = "Pending",
                        CreatedAt = DateTime.UtcNow
                    };

                    ordersList.Add(order);

                    await _productService.ReduceStockAsync(item.ProductId, item.Quantity);
                }

                if (ordersList.Any())
                {
                    await _orderRepository.AddRangeAsync(ordersList);
                    await _orderRepository.SaveChangesAsync();
                    LoggingHelper.LogSuccess(SOURCE, $"Checkout completed. Created {ordersList.Count} orders.");
                }
                else
                {
                    LoggingHelper.LogWarning(SOURCE, "No valid items to order");
                }

                return ordersList;
            }
            catch (BusinessRuleException)
            {
                throw;
            }
            catch (Exception ex)
            {
                LoggingHelper.LogError(SOURCE, "Checkout failed", ex);
                throw new AppException($"Checkout failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates the status of an order.
        /// Valid statuses: Pending, Processing, Shipped, Delivered, Cancelled
        /// </summary>
        public async Task UpdateOrderStatusAsync(int orderId, string newStatus)
        {
            LoggingHelper.LogInfo(SOURCE, $"Updating order {orderId} status to: {newStatus}");

            var order = await _orderRepository.GetByIdAsync(orderId);
            
            if (order == null)
            {
                throw new NotFoundException("Order", orderId);
            }

            var validStatuses = new[] { "Pending", "Processing", "Shipped", "Delivered", "Cancelled" };
            
            if (!validStatuses.Contains(newStatus))
            {
                throw new ValidationException($"Invalid order status: {newStatus}");
            }

            try
            {
                order.Status = newStatus;
                _orderRepository.Update(order);
                await _orderRepository.SaveChangesAsync();

                LoggingHelper.LogSuccess(SOURCE, $"Order {orderId} status updated to {newStatus}");
            }
            catch (Exception ex)
            {
                LoggingHelper.LogError(SOURCE, $"Failed to update order {orderId} status", ex);
                throw new AppException($"Failed to update order status: {ex.Message}");
            }
        }
    }
}
