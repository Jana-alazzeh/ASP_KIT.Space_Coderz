using July_Team.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using July_Team.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks; 
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;

namespace July_Team.Controllers
{
    public class OrdersController : Controller
    {
        private readonly AppDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;

        public OrdersController(AppDbContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

       
        [HttpGet]
        public IActionResult Checkout()
        {
            var cart = GetCart();

            if (cart == null || !cart.Items.Any())
            {
                return RedirectToAction("Index", "Cart");
            }

            var checkoutViewModel = new CheckoutViewModel
            {
                Items = cart.Items.Select(item => new OrderItemViewModel
                {


                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    Quantity = item.Quantity,
                    UnitPrice = item.Price,
                    Size = item.Size
                }).ToList(),

               
                SubTotal = cart.Items.Sum(item => item.Price * item.Quantity),

                // 2. رسوم الشحن لها قيمة افتراضية في الـ ViewModel (3.00m)، لذلك لا داعي لتعيينها هنا
                //    إلا إذا أردتِ تغييرها بناءً على شرط معين.
                // ShippingFee = 3.00m, // (هذا السطر ليس ضروريًا)
            };

            // 🛑 لا يوجد سطر لحساب GrandTotal هنا 🛑
            // الـ ViewModel سيقوم بحسابها تلقائيًا عند عرض الصفحة

            return View(checkoutViewModel);
        }
              

    
        private CartViewModel GetCart()
        {
            var cartJson = HttpContext.Session.GetString("Cart");
            if (string.IsNullOrEmpty(cartJson))
            {
                return new CartViewModel();
            }
            return JsonConvert.DeserializeObject<CartViewModel>(cartJson);
        }

        private void SaveCart(CartViewModel cart)
        {
            var cartJson = JsonConvert.SerializeObject(cart);
            HttpContext.Session.SetString("Cart", cartJson);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessCheckout(CheckoutViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View("Checkout", model);
                }

                string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                // ✅ 1. تعريف القائمة هنا (خارج الفور إيتش)
                var ordersList = new List<Order>();

                foreach (var item in model.Items)
                {
                    var product = await _db.Products.FindAsync(item.ProductId);
                    if (product != null && product.Stock >= item.Quantity)
                    {
                        var order = new Order
                        {
                            UserId = userId,
                            ProductId = item.ProductId,
                            Quantity = item.Quantity,
                            TotalPrice = item.UnitPrice * item.Quantity,
                            Status = "Pending",
                            CreatedAt = DateTime.UtcNow
                        };

                        // ✅ 2. إضافة الطلب للقائمة
                        ordersList.Add(order);

                        product.Stock -= item.Quantity;
                        _db.Products.Update(product);
                    }
                }

                // ✅ 3. الآن سطر الحفظ سيعمل لأن ordersList معرفة في هذا النطاق
                await _db.Orders.AddRangeAsync(ordersList);
                await _db.SaveChangesAsync();

                HttpContext.Session.Remove("Cart");

                // تأكدي أن الميثود أدناه تستقبل ordersList وليس orders
                await SendOrderConfirmationEmail(model, ordersList);

                return RedirectToAction("OrderConfirmation");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "حدث خطأ: " + ex.Message);
                return View("Checkout", model);
            }
        }
        [HttpGet]
        public IActionResult OrderConfirmation()
        {
            return View();
        }

        [Authorize(Roles = "super admin, Admin")]
        public async System.Threading.Tasks.Task<IActionResult> AdminIndex()
        {
            var orders = await _db.Orders
                .Include(o => o.Product)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return View(orders);
        }

        [Authorize(Roles = "super admin,Admin")]
        public async System.Threading.Tasks.Task<IActionResult> Details(int id)
        {
            var order = await _db.Orders
                .Include(o => o.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound();
            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "super admin,Admin")]
        public async System.Threading.Tasks.Task<IActionResult> UpdateStatus(int id, string newStatus)
        {
            var order = await _db.Orders.FindAsync(id);
            if (order == null) return NotFound();

            order.Status = newStatus;
            _db.Orders.Update(order);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(AdminIndex));
        }

        
        [HttpGet]
        [Authorize]
        public async System.Threading.Tasks.Task<IActionResult> MyOrders()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var orders = await _db.Orders
                .Include(o => o.Product)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return View(orders);
        }

        private async System.Threading.Tasks.Task SendOrderConfirmationEmail(CheckoutViewModel model, List<Order> orders)
        {

            try
            {
                // كود إرسال البريد الإلكتروني
                // await _emailSender.SendEmailAsync(model.ShippingEmail, "Order Confirmation", "Your order has been placed.");
            }
            catch (Exception ex)
            {
                // تسجيل خطأ إرسال البريد دون توقف العملية
                // _logger.LogWarning(ex, "فشل في إرسال بريد تأكيد الطلب");
            }
            await System.Threading.Tasks.Task.CompletedTask; // ضمان إرجاع Task
        }

       
        [HttpGet]
        public async System.Threading.Tasks.Task<IActionResult> GetProductDetails(int productId)
        {
            var product = await _db.Products.FindAsync(productId);
            if (product == null)
            {
                return Json(new { success = false, message = "Product not found" });
            }

            return Json(new
            {
                success = true,
                product = new
                {
                    id = product.Id,
                    name = product.Name,
                    price = product.Price,
                    stock = product.Stock,
                    imageUrl = product.ImageUrl
                }
            });
        }


       
    }
}
