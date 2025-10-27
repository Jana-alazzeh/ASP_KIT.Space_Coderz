using July_Team.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

//[Authorize]
public class ProductsController : Controller
{
    private readonly AppDbContext _db;

    public ProductsController(AppDbContext db)
    {
        _db = db;
    }
   
    
    //[Authorize(Roles = "Admin")] // 👈 فقط Admin يستطيع الدخول
    public async Task<IActionResult> AdminIndex()
    {
        var products = await _db.Products.ToListAsync();
        // يعرض Views/Products/AdminIndex.cshtml
        return View(products);
    }

    
    [HttpGet]
    //[Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        // يعرض Views/Products/Create.cshtml
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    //[Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(Product model)
    {
        if (ModelState.IsValid)
        {
            _db.Products.Add(model);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(AdminIndex)); // العودة إلى لوحة التحكم
        }
        return View(model);
    }

   
    [HttpGet]
    //[Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var product = await _db.Products.FindAsync(id);
        if (product == null) return NotFound();
        return View(product);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    //[Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id, Product model) // أضفنا 'id' هنا
    {
        // 1. تحقق من تطابق الـ id القادم من الرابط مع الـ id الموجود في المودل
        if (id != model.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                var productToUpdate = await _db.Products.FindAsync(id);

                if (productToUpdate == null)
                {
                    return NotFound();
                }

                productToUpdate.Name = model.Name;
                productToUpdate.Description = model.Description;
                productToUpdate.Price = model.Price;
                productToUpdate.Stock = model.Stock;
                productToUpdate.ImageUrl = model.ImageUrl;

                // 4. الآن قم بتحديث الكائن الذي تتبعه قاعدة البيانات
                // _db.Products.Update(productToUpdate); // هذه الخطوة اختيارية إذا كنت تستخدم FindAsync

                // 5. احفظ التغييرات
                await _db.SaveChangesAsync();

                return RedirectToAction(nameof(AdminIndex));
            }
            catch (DbUpdateConcurrencyException)
            {
                ModelState.AddModelError("", "لم يتمكن من حفظ التغييرات. حاول مرة أخرى.");
            }
        }
        return View(model);
    }


    // نستخدم الـ POST مباشرةً للحذف لتبسيط الكود الإداري
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    //[Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var product = await _db.Products.FindAsync(id);
        if (product != null)
        {
            _db.Products.Remove(product);
            await _db.SaveChangesAsync();
        }
        return RedirectToAction(nameof(AdminIndex));
    }

    [HttpGet]
    [AllowAnonymous] // 👈 هذا الأكشن متاح للجميع
    public async Task<IActionResult> Index()
    {
        // عرض المنتجات للبيع
        var products = await _db.Products.ToListAsync();
        return View(products); // يعرض Views/Products/Index.cshtml
    }
    
    public async Task<IActionResult> Details(int id)
    {
        var product = await _db.Products.FindAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        var viewModel = new ProductDetailViewModel
        {
            ProductId = product.Id,
            Name = product.Name,
            Price = product.Price,
            Description = product.Description,
            ImageUrl = product.ImageUrl,
            ImageUrl_Back = product.ImageUrl_Back, // 👈 أضيفي هذا السطر
            AvailableStock = product.Stock
        };

        return View(viewModel);
    }

}