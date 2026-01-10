using Microsoft.AspNetCore.Mvc;
using July_Team.Models;

namespace July_Team.Controllers
{
    public class JoinUsController : Controller
    {
        private readonly AppDbContext _context;

        public JoinUsController(AppDbContext context)
        {
            _context = context;
        }

        
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
                var errors = ModelState.Values.SelectMany(v => v.Errors);
               
                _context.JoinRequests.Add(model);
                _context.SaveChanges();
              

                TempData["SuccessMessage"] = "Your application has been submitted successfully! ✅";
                return RedirectToAction("Index");
                  
            }

            
            return View("Index", model);
        }
    }
}
