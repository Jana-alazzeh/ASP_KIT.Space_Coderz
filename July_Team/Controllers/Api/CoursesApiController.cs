using July_Team.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class CoursesApiController : ControllerBase
{
    private readonly AppDbContext _db;

    public CoursesApiController(AppDbContext db)
    {
        _db = db;
    }

    // READ - كل الكورسات
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var courses = await _db.Courses.ToListAsync();
        return Ok(courses);
    }

    // READ - كورس محدد
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var course = await _db.Courses.FindAsync(id);
        if (course == null)
            return NotFound(new { message = "الكورس غير موجود" });
        return Ok(course);
    }

    // CREATE - إضافة كورس جديد
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Course model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _db.Courses.Add(model);
        await _db.SaveChangesAsync();
        return Ok(new { message = "تم إضافة الكورس بنجاح", courseId = model.Id });
    }

    // UPDATE - تعديل كورس
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Course model)
    {
        if (id != model.Id)
            return BadRequest(new { message = "معرف الكورس غير صحيح" });

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var course = await _db.Courses.FindAsync(id);
        if (course == null)
            return NotFound(new { message = "الكورس غير موجود" });

        // تحديث الحقول
        course.Title = model.Title;
        course.Description = model.Description;
        course.TrainerName = model.TrainerName;
        course.StartDate = model.StartDate;
        course.EndDate = model.EndDate;
        course.ImageUrl = model.ImageUrl;
        course.Price = model.Price;
        course.Duration = model.Duration;

        _db.Courses.Update(course);
        await _db.SaveChangesAsync();

        return Ok(new { message = "تم تحديث الكورس بنجاح" });
    }

    // DELETE - حذف كورس
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var course = await _db.Courses.FindAsync(id);
        if (course == null)
            return NotFound(new { message = "الكورس غير موجود" });

        _db.Courses.Remove(course);
        await _db.SaveChangesAsync();

        return Ok(new { message = "تم حذف الكورس بنجاح" });
    }
}
