using Microsoft.AspNetCore.Mvc;
using July_Team.Models;


namespace July_Team.Controllers
{
    public class ContactUsController : Controller
    {
        private readonly AppDbContext _context;

        public ContactUsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(ContactUsViewModel model)
        {
            if (ModelState.IsValid)
            {
                _context.ContactUs.Add(model);
                _context.SaveChanges();
                ViewBag.Message = "تم إرسال رسالتك بنجاح ✅";
                ModelState.Clear();
                return View();
            }

            return View(model);
        }
    }
}
