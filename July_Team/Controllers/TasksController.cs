using July_Team.Models;
using July_Team.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System; // تمت إضافة هذا السطر لضمان عمل DateTime

public class TasksController : Controller
{
    private readonly AppDbContext _db;
    private readonly UserManager<IdentityUser> _userManager;

    public TasksController(AppDbContext db, UserManager<IdentityUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    // دالة مركزية للحصول على هوية المستخدم
    private string GetCurrentUserId()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return "user-id-for-testing";
        }
        return userId;
    }

    // دالة مركزية لحساب الإحصائيات
    private async Task<TaskDashboardViewModel> GetDashboardStatsAsync()
    {
        var userId = GetCurrentUserId();
        var userTasks = await _db.Tasks
                                 .Where(t => t.OwnerId == userId)
                                 .ToListAsync();

        var totalTasks = userTasks.Count;
        // ===== التعديل الأول هنا =====
        var doneTasks = userTasks.Count(t => t.Status == July_Team.Models.TaskStatus.Done);
        var progress = (totalTasks > 0) ? (int)Math.Round((double)doneTasks * 100 / totalTasks) : 0;

        return new TaskDashboardViewModel
        {
            TotalTasks = totalTasks,
            PendingTasks = totalTasks - doneTasks,
            ProgressPercentage = progress
        };
    }

    // Action الرئيسي للعرض
    public async Task<IActionResult> Index()
    {
        var viewModel = await GetDashboardStatsAsync();
        return View(viewModel);
    }

    // Action لجلب المهام بشكل ديناميكي (AJAX)
    [HttpGet]
    public async Task<IActionResult> GetTasksPartial(string filter = "daily")
    {
        var userId = GetCurrentUserId();
        var query = _db.Tasks.Where(t => t.OwnerId == userId);

        var today = DateTime.Today;
        switch (filter.ToLower())
        {
            case "weekly":
                var startOfWeek = today.AddDays(-(int)today.DayOfWeek);
                var endOfWeek = startOfWeek.AddDays(6);
                query = query.Where(t => t.DueDate.Date >= startOfWeek && t.DueDate.Date <= endOfWeek);
                break;
            case "all":
                break;
            case "daily":
            default:
                query = query.Where(t => t.DueDate.Date == today);
                break;
        }

        var tasks = await query.OrderBy(t => t.DueDate).ToListAsync();
        return PartialView("_TaskList", tasks);
    }

    // Action لإنشاء مهمة سريعة (AJAX)
    [HttpPost]
    public async Task<IActionResult> QuickCreate([FromForm] string Title, [FromForm] DateTime DueDate)
    {
        if (string.IsNullOrWhiteSpace(Title))
        {
            return Json(new { success = false, message = "Task title is required." });
        }

        // تم استخدام النموذج Task من Models وليس من System.Threading.Tasks
        var newTask = new July_Team.Models.Task
        {
            Title = Title,
            DueDate = DueDate,
            OwnerId = GetCurrentUserId(),
            // ===== التعديل الثاني هنا =====
            Status = July_Team.Models.TaskStatus.Pending,
            Description = ""
        };

        _db.Tasks.Add(newTask);
        await _db.SaveChangesAsync();

        var newStats = await GetDashboardStatsAsync();
        return Json(new { success = true, newStats });
    }

    // Action لتغيير حالة المهمة (AJAX)
    [HttpPost]
    public async Task<IActionResult> ToggleStatus(int id)
    {
        var userId = GetCurrentUserId();
        var task = await _db.Tasks.FirstOrDefaultAsync(t => t.Id == id && t.OwnerId == userId);

        if (task == null)
        {
            return NotFound(new { success = false, message = "Task not found." });
        }

        // ===== التعديل الثالث هنا =====
        task.Status = (task.Status == July_Team.Models.TaskStatus.Done)
                    ? July_Team.Models.TaskStatus.Pending
                    : July_Team.Models.TaskStatus.Done;

        _db.Tasks.Update(task);
        await _db.SaveChangesAsync();

        var newStats = await GetDashboardStatsAsync();
        return Json(new { success = true, newStats });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var userId = GetCurrentUserId();
        var task = await _db.Tasks.FindAsync(id);

        if (task == null)
        {
            // إذا لم يتم العثور على المهمة أصلاً
            return NotFound("Task not found.");
        }

        if (task.OwnerId != userId)
        {
            // إذا كانت المهمة موجودة ولكنها لا تخص المستخدم الحالي
            return Forbid("You do not have permission to edit this task.");
        }

        return View(task);
    }

    // POST: /Tasks/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,DueDate,Status,OwnerId,CreatedAt")] July_Team.Models.Task task)
    {
        if (id != task.Id)
        {
            return BadRequest("Task ID mismatch.");
        }

        // التحقق من الملكية مرة أخرى كإجراء أمني إضافي
        var userId = GetCurrentUserId();
        if (task.OwnerId != userId)
        {
            return Forbid("You cannot change the owner of this task.");
        }

        if (ModelState.IsValid)
        {
            try
            {
                // Entity Framework ذكي كفاية ليعرف أن هذا الكائن موجود ويجب تحديثه
                _db.Update(task);
                await _db.SaveChangesAsync();

                // بعد الحفظ الناجح، أعد التوجيه إلى لوحة التحكم
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                // في حال حدوث خطأ غير متوقع أثناء الحفظ، اعرضه
                ModelState.AddModelError("", "Unable to save changes. " + ex.Message);
            }
        }

        // إذا وصلنا إلى هنا، فهذا يعني أن `ModelState` غير صالح
        // أعد عرض الصفحة مع نفس البيانات التي أدخلها المستخدم لعرض رسائل الخطأ
        return View(task);
    }
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        // 1. استخدام _db بدلاً من _context ليتوافق مع الـ Constructor الخاص بك
        var userId = GetCurrentUserId();
        var task = await _db.Tasks.FirstOrDefaultAsync(t => t.Id == id && t.OwnerId == userId);

        if (task == null)
        {
            return Json(new { success = false, message = "المهمة غير موجودة أو لا تملك صلاحية حذفها" });
        }

        try
        {
            // 2. الحذف من قاعدة البيانات باستخدام _db
            _db.Tasks.Remove(task);
            await _db.SaveChangesAsync();

            // 3. تحديث الإحصائيات للمستخدم الحالي فقط لضمان دقة الأرقام في الواجهة
            var userTasks = await _db.Tasks.Where(t => t.OwnerId == userId).ToListAsync();

            var totalTasks = userTasks.Count;
            var completedTasks = userTasks.Count(t => t.Status == July_Team.Models.TaskStatus.Done);
            var pendingTasks = totalTasks - completedTasks;

            double progressPercentage = totalTasks > 0
                ? Math.Round((double)completedTasks / totalTasks * 100)
                : 0;

            return Json(new
            {
                success = true,
                newStats = new
                {
                    totalTasks = totalTasks,
                    pendingTasks = pendingTasks,
                    progressPercentage = progressPercentage
                }
            });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = "حدث خطأ أثناء الحذف: " + ex.Message });
        }
    }
}
