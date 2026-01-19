using July_Team.Models;
using Task = System.Threading.Tasks.Task;

namespace July_Team.Services
{
    /// <summary>
    /// Interface defining order-related business operations.
    /// Encapsulates the order processing workflow.
    /// </summary>
    public interface IOrderService
    {
        /// <summary>
        /// Retrieves all orders (for admin view).
        /// </summary>
        Task<IEnumerable<Order>> GetAllOrdersAsync();

        /// <summary>
        /// Gets orders for a specific user.
        /// </summary>
        /// <param name="userId">The user's ID</param>
        Task<IEnumerable<Order>> GetOrdersByUserIdAsync(string userId);

        /// <summary>
        /// Gets a single order by ID with product details.
        /// </summary>
        /// <param name="id">Order ID</param>
        Task<Order?> GetOrderByIdAsync(int id);

        /// <summary>
        /// Processes a checkout and creates orders.
        /// </summary>
        /// <param name="model">Checkout data</param>
        /// <param name="userId">User making the order</param>
        /// <returns>List of created orders</returns>
        Task<List<Order>> ProcessCheckoutAsync(CheckoutViewModel model, string? userId);

        /// <summary>
        /// Updates order status (for admin).
        /// </summary>
        /// <param name="orderId">Order ID</param>
        /// <param name="newStatus">New status value</param>
        Task UpdateOrderStatusAsync(int orderId, string newStatus);
    }
}
