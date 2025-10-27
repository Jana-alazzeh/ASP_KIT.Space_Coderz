using July_Team.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json; 

public class CartController : Controller
{
    private readonly AppDbContext _db;

    public CartController(AppDbContext db)
    {
        _db = db;
    }

    public IActionResult Index()
    {
        var cart = GetCart();
        return View(cart);
    }

    [HttpPost]
    public async Task<IActionResult> AddToCart(ProductDetailViewModel model)
    {
        var product = await _db.Products.FindAsync(model.ProductId);
        if (product == null)
        {
            return NotFound();
        }

        var cart = GetCart();
        var cartItem = cart.Items.FirstOrDefault(i => i.ProductId == model.ProductId && i.Size == model.SelectedSize);

        if (cartItem == null)
        {
            
            cart.Items.Add(new CartItem
            {
                ProductId = product.Id,
                ProductName = product.Name,
                Price = product.Price,
                Quantity = model.SelectedQuantity,
                Size = model.SelectedSize,
                ImageUrl = product.ImageUrl
            });
        }
        else
        {
           
            cartItem.Quantity += model.SelectedQuantity;
        }

        SaveCart(cart);
        TempData["SuccessMessage"] = $"'{product.Name}' has been added to the cart!";
        return RedirectToAction("Index"); 
    }

    [HttpPost]
    public IActionResult UpdateQuantity(int productId, string size, int quantity)
    {
        var cart = GetCart();
        var cartItem = cart.Items.FirstOrDefault(i => i.ProductId == productId && i.Size == size);

        if (cartItem != null)
        {
            if (quantity > 0)
            {
                cartItem.Quantity = quantity;
            }
            else
            {
               
                cart.Items.Remove(cartItem);
            }
            SaveCart(cart);
        }
        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult RemoveFromCart(int productId, string size)
    {
        var cart = GetCart();
        var cartItem = cart.Items.FirstOrDefault(i => i.ProductId == productId && i.Size == size);

        if (cartItem != null)
        {
            cart.Items.Remove(cartItem);
            SaveCart(cart);
        }
        return RedirectToAction("Index");
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
}
