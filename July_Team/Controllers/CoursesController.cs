using July_Team.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks; 

//[Authorize]
public class CoursesController : Controller
{
    private readonly AppDbContext _db;

    public CoursesController(AppDbContext db)
    {
        _db = db;
    }


    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        var courses = await _db.Courses.ToListAsync();
        return View(courses);
    }


    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Details(int id)
    {
        var course = await _db.Courses.FindAsync(id);
        if (course == null) return NotFound();
        return View(course);
    }


    [HttpGet]
    //[Authorize(Roles = "Admin, Trainer")]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    //[Authorize(Roles = "Admin, Trainer")]
    public async Task<IActionResult> Create(Course model)
    {
        if (ModelState.IsValid)
        {
            _db.Courses.Add(model);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index)); 
        }
        return View(model);
    }


    [HttpGet]
    //[Authorize(Roles = "Admin, Trainer")]
    public async Task<IActionResult> Edit(int id)
    {
        var course = await _db.Courses.FindAsync(id);
        if (course == null) return NotFound();
        return View(course);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    //[Authorize(Roles = "Admin, Trainer")]
    public async Task<IActionResult> Edit(Course model)
    {
        if (ModelState.IsValid)
        {
            _db.Courses.Update(model);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index)); 
        }
        return View(model);
    }

    [HttpGet]
    //[Authorize(Roles = "Admin, Trainer")]
    public async Task<IActionResult> AdminIndex()
    {
        var courses = await _db.Courses.ToListAsync();
        return View(courses);
    }

   
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    //[Authorize(Roles = "Admin, Trainer")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var course = await _db.Courses.FindAsync(id);
        if (course != null)
        {
            _db.Courses.Remove(course);
            await _db.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}