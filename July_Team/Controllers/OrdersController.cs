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
using System.Threading.Tasks; // تحديد استخدام Task من System.Threading.Tasks
using Newtonsoft.Json;

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

        // ===============================================
        // صفحة الشيك أوت
        // ===============================================
        // في OrdersController.cs
        // في OrdersController.cs

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

                // 1. نحن نحسب المجموع الفرعي هنا
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

        // ===============================================
        // معالجة طلب الشيك أوت
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async System.Threading.Tasks.Task<IActionResult> ProcessCheckout(CheckoutViewModel model)

        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { success = false, message = "بيانات غير صحيحة" });
                }

                // الحصول على معرف المستخدم الحالي
                string userId = null;
                if (User.Identity.IsAuthenticated)
                {
                    userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                }

                // إنشاء طلبات منفصلة لكل منتج (حسب هيكل Order الحالي)
                var orders = new List<Order>();

                foreach (var item in model.Items)
                {
                    // التحقق من وجود المنتج وتوفر الكمية
                    var product = await _db.Products.FindAsync(item.ProductId);
                    if (product == null)
                    {
                        return Json(new { success = false, message = $"المنتج {item.ProductName} غير موجود" });
                    }

                    if (product.Stock < item.Quantity)
                    {
                        return Json(new { success = false, message = $"الكمية المطلوبة من {product.Name} غير متوفرة" });
                    }

                    // إنشاء طلب جديد
                    var order = new Order
                    {
                        UserId = userId,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        TotalPrice = item.UnitPrice * item.Quantity,
                        Status = "Pending",
                        CreatedAt = DateTime.UtcNow
                    };

                    orders.Add(order);

                    // تقليل المخزون
                    product.Stock -= item.Quantity;
                    _db.Products.Update(product);
                }

                // حفظ الطلبات في قاعدة البيانات
                await _db.Orders.AddRangeAsync(orders);
                await _db.SaveChangesAsync();

                // مسح السلة من الـ Session بعد إتمام الطلب بنجاح
                HttpContext.Session.Remove("Cart");

                // إرسال إشعار أو بريد إلكتروني (اختياري)
                await SendOrderConfirmationEmail(model, orders);

                return RedirectToAction("OrderConfirmation");

            }
            catch (Exception ex)
            {
                // تسجيل الخطأ
                // _logger.LogError(ex, "خطأ في معالجة طلب الشراء");

                return Json(new
                {
                    success = false,
                    message = "حدث خطأ أثناء معالجة طلبك. يرجى المحاولة مرة أخرى."
                });
            }
        }

        // ===============================================
        // صفحة تأكيد الطلب
        // ===============================================
        [HttpGet]
        public IActionResult OrderConfirmation()
        {
            return View();
        }

        // ===============================================
        // لوحة التحكم - عرض جميع طلبات الشراء (للإدارة)
        // ===============================================
        [Authorize(Roles = "Admin")]
        public async System.Threading.Tasks.Task<IActionResult> AdminIndex()
        {
            var orders = await _db.Orders
                .Include(o => o.Product)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return View(orders);
        }

        // ===============================================
        // تفاصيل طلب الشراء
        // ===============================================
        [Authorize(Roles = "Admin")]
        public async System.Threading.Tasks.Task<IActionResult> Details(int id)
        {
            var order = await _db.Orders
                .Include(o => o.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound();
            return View(order);
        }

        // ===============================================
        // تغيير حالة طلب الشراء
        // ===============================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async System.Threading.Tasks.Task<IActionResult> UpdateStatus(int id, string newStatus)
        {
            var order = await _db.Orders.FindAsync(id);
            if (order == null) return NotFound();

            order.Status = newStatus;
            _db.Orders.Update(order);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(AdminIndex));
        }

        // ===============================================
        // طلبات المستخدم الحالي
        // ===============================================
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

        // ===============================================
        // إرسال بريد تأكيد الطلب (دالة مساعدة)
        // ===============================================
        private async System.Threading.Tasks.Task SendOrderConfirmationEmail(CheckoutViewModel model, List<Order> orders)
        {
            // هنا يمكن إضافة منطق إرسال البريد الإلكتروني
            // مثال: استخدام SendGrid أو SMTP

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

        // ===============================================
        // API للحصول على تفاصيل المنتج (للاستخدام في JavaScript)
        // ===============================================
        [HttpGet]
        public async System.Threading.Tasks.Task<IActionResult> GetProductDetails(int productId)
        {
            var product = await _db.Products.FindAsync(productId);
            if (product == null)
            {
                return Json(new { success = false, message = "المنتج غير موجود" });
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
