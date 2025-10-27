using July_Team.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims; 


public class TasksController : Controller
{
    private readonly AppDbContext _db;
    private readonly UserManager<IdentityUser> _userManager;

    public TasksController(AppDbContext db, UserManager<IdentityUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(currentUserId))
        {
            currentUserId = "user-id-for-testing"; // المستخدم الوهمي
        }

        var userTasks = await _db.Tasks
                                 .Where(t => t.OwnerId == currentUserId)
                                 .ToListAsync();

        // ==================== الكود الجديد لحساب نسبة الإنجاز ====================
        int totalTasks = userTasks.Count;
        int doneTasks = userTasks.Count(t => t.Status == July_Team.Models.TaskStatus.Done);

        // حساب النسبة المئوية (مع تجنب القسمة على صفر)
        int progressPercentage = (totalTasks > 0) ? (int)Math.Round((double)doneTasks / totalTasks * 100) : 0;

        // إرسال النسبة إلى الـ View
        ViewBag.ProgressPercentage = progressPercentage;
        // ==================== نهاية الكود الجديد ====================

        // الآن نعرض صفحة Index.cshtml ونمرر لها قائمة المهام
        return View(userTasks);
    }

    [HttpGet]
    public IActionResult Create()
    {
        // لم نعد بحاجة لـ PopulateUsersDropdown
        return View();
    }

    //[HttpPost]
    //[ValidateAntiForgeryToken]
    //public async Task<IActionResult> Create(July_Team.Models.Task model)
    //{
    //    var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    //    if (string.IsNullOrEmpty(currentUserId))
    //    {
    //        return Unauthorized(); // أو توجيه لصفحة الدخول
    //    }

    //    // تعيين مالك المهمة تلقائياً ليكون المستخدم الحالي
    //    model.OwnerId = currentUserId;

    //    _db.Tasks.Add(model);
    //    await _db.SaveChangesAsync();
    //    return RedirectToAction(nameof(Index));
    //}


    [HttpPost]
    // [ValidateAntiForgeryToken] // اتركيه معطلاً حالياً
    public async Task<IActionResult> Create(July_Team.Models.Task model)
    {
        // ==================== بداية الكود المؤقت (للتطوير) ====================

        // نحاول الحصول على المستخدم الحقيقي
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // إذا لم نجد مستخدماً (لأننا لم نسجل دخولنا)، نستخدم ID افتراضي مؤقت
        if (string.IsNullOrEmpty(currentUserId))
        {
            currentUserId = "user-id-for-testing"; // <-- هذا هو المستخدم الوهمي
        }

        // ==================== نهاية الكود المؤقت ====================


        // الآن، سيتم تعيين المهمة دائماً للمستخدم الوهمي
        model.OwnerId = currentUserId;

        _db.Tasks.Add(model);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
   
    [HttpPost]
    public async Task<IActionResult> QuickCreate(string Title, DateTime DueDate)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(currentUserId))
        {
            currentUserId = "user-id-for-testing";
        }

       
        if (!string.IsNullOrEmpty(Title))
        {
            
            var newTask = new July_Team.Models.Task
            {
                Title = Title,
                DueDate = DueDate,
                OwnerId = currentUserId,
                Status = July_Team.Models.TaskStatus.Pending, 
                Description = ""
            };

            _db.Tasks.Add(newTask);
            await _db.SaveChangesAsync();
        }

        // في كل الحالات، نعود إلى صفحة Index
        return RedirectToAction(nameof(Index));
    }



   
    //[HttpGet]
    //public async Task<IActionResult> Edit(int id)
    //{
    //    var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    //    var task = await _db.Tasks.FindAsync(id);

    //    // التحقق من أن المهمة موجودة وأن المستخدم الحالي هو مالكها
    //    if (task == null || task.OwnerId != currentUserId)
    //    {
    //        return Forbid(); // أو NotFound()
    //    }

    //    return View(task);
    //}
    // 📁 Controllers/TasksController.cs
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(currentUserId))
        {
            currentUserId = "user-id-for-testing"; 
        }
        // ==================== نهاية الجزء المضاف ====================

        var task = await _db.Tasks.FindAsync(id);

        // الآن سيتم التحقق من الملكية بشكل صحيح
        if (task == null || task.OwnerId != currentUserId)
        {
            return Forbid();
        }

        return View(task);
    }

    //[HttpPost]
    //[ValidateAntiForgeryToken]
    //public async Task<IActionResult> Edit(July_Team.Models.Task model)
    //{
    //    var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

    //    // نتأكد أن المستخدم لا يعدل مهمة لا يملكها
    //    if (model.OwnerId != currentUserId)
    //    {
    //        return Forbid();
    //    }

    //    _db.Tasks.Update(model);
    //    await _db.SaveChangesAsync();
    //    return RedirectToAction(nameof(Index));
    //}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(July_Team.Models.Task model)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(currentUserId))
        {
            currentUserId = "user-id-for-testing"; // المستخدم الوهمي
        }

        // نتأكد أن المستخدم لا يعدل مهمة لا يملكها
        if (model.OwnerId != currentUserId)
        {
            return Forbid();
        }

        _db.Tasks.Update(model);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }


    

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        
        if (string.IsNullOrEmpty(currentUserId))
        {
            currentUserId = "user-id-for-testing";
        }
       

        var task = await _db.Tasks.FindAsync(id);

        if (task != null && task.OwnerId == currentUserId)
        {
            _db.Tasks.Remove(task);
            await _db.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    
    [HttpPost]
    public async Task<IActionResult> ToggleStatus(int id)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(currentUserId))
        {
            currentUserId = "user-id-for-testing";
        }

        var task = await _db.Tasks.FirstOrDefaultAsync(t => t.Id == id && t.OwnerId == currentUserId);

        if (task != null)
        {
            task.Status = (task.Status == July_Team.Models.TaskStatus.Done)
                        ? July_Team.Models.TaskStatus.Pending
                        : July_Team.Models.TaskStatus.Done;

            _db.Tasks.Update(task);
            await _db.SaveChangesAsync();

            var userTasks = await _db.Tasks.Where(t => t.OwnerId == currentUserId).ToListAsync();
            int totalTasks = userTasks.Count;
            int doneTasks = userTasks.Count(t => t.Status == July_Team.Models.TaskStatus.Done);
            int newProgress = (totalTasks > 0) ? (int)Math.Round((double)doneTasks / totalTasks * 100) : 0;

            return Ok(new { progress = newProgress });
        }

        return NotFound();
    }



}
