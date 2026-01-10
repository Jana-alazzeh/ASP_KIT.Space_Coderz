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
        public IActionResult SubmitJoinRequest(JoinUsViewModel model)
        {
            if (ModelState.IsValid)
            {
                // 1. الحفظ أولاً
                _context.JoinRequests.Add(model);
                _context.SaveChanges();

                // 2. إعداد رسالة النجاح
                TempData["SuccessMessage"] = "Your application has been submitted successfully! ✅";

                // 3. إعادة التوجيه (هذا يفرغ الفورم تلقائياً)
                return RedirectToAction("Index");
            }

            // إذا فشل التحقق (مثلاً إيميل خطأ)، ارجع للفيو مع إظهار الأخطاء
            return View("Index", model);
        }
    }
}



