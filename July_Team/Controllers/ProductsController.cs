using July_Team.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using static System.Net.Mime.MediaTypeNames;

//[Authorize]
public class ProductsController : Controller
{
    private readonly AppDbContext _db;

    public ProductsController(AppDbContext db)
    {
        _db = db;
    }


    //[Authorize(Roles = "Admin")] 
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
    [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any, NoStore = false)]
    public async Task<IActionResult> Index()
    {
        // 1. استخدام AsNoTracking() لتسريع الاستعلام لأنه للقراءة فقط
        // 2. جلب البيانات الضرورية فقط (اختياري ولكن يفضل)
        var products = await _db.Products.AsNoTracking().ToListAsync();
        return View(products);
    }

    private async Task<string> SaveImageAsWebP(IFormFile file)
    {
        if (file == null || file.Length == 0) return null;

        var fileName = Guid.NewGuid().ToString() + ".webp";
        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploade", "image");

        if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

        var filePath = Path.Combine(uploadsFolder, fileName);

        // بدلاً من استخدام Image.Load مباشرة، استخدم المسار الكامل لتجنب التضارب
        using (var image = SixLabors.ImageSharp.Image.Load(file.OpenReadStream()))
        {
            if (image.Width > 1200)
            {
                image.Mutate(x => x.Resize(1200, 0));
            }

            await image.SaveAsWebpAsync(filePath);
        }


        return "/Uploade/image/" + fileName;
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