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
        [ValidateAntiForgeryToken]
        public IActionResult SubmitJoinRequest(ContactUsViewModel model)
        {
            if (ModelState.IsValid)
            {
                // الاسم الصحيح للجدول عندك في الـ AppDbContext هو ContactUs
                _context.ContactUs.Add(model);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Your message has been sent successfully! ✅";

                return RedirectToAction("Index");
            }

            // إذا كان هناك خطأ في البيانات، يرجع لنفس الصفحة مع الموديل الصحيح
            return View("Index", model);
        }
    }
}



