using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using July_Team.Models;

[Route("api/[controller]")]
[ApiController]
public class UserApiController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;

    public UserApiController(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    // CREATE - تسجيل مستخدم جديد
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegesterModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = new IdentityUser { UserName = model.User_Email, Email = model.User_Email };
        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
            return Ok(new { message = "تم إنشاء الحساب بنجاح", userId = user.Id });

        var errors = result.Errors.Select(e => e.Description);
        return BadRequest(new { errors });
    }

    // READ - جلب كل المستخدمين
    [HttpGet("all")]
    public IActionResult GetAllUsers()
    {
        var users = _userManager.Users.Select(u => new
        {
            u.Id,
            u.UserName,
            u.Email
        }).ToList();

        return Ok(users);
    }

    // READ - جلب مستخدم محدد
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return NotFound(new { message = "المستخدم غير موجود" });

        return Ok(new { user.Id, user.UserName, user.Email });
    }

    // UPDATE - تعديل اسم المستخدم أو البريد
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(string id, [FromBody] RegesterModel model)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return NotFound(new { message = "المستخدم غير موجود" });

        user.UserName = model.User_Email;
        user.Email = model.User_Email;

        var result = await _userManager.UpdateAsync(user);

        if (result.Succeeded)
            return Ok(new { message = "تم التحديث بنجاح" });

        var errors = result.Errors.Select(e => e.Description);
        return BadRequest(new { errors });
    }

    // DELETE - حذف مستخدم
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return NotFound(new { message = "المستخدم غير موجود" });

        var result = await _userManager.DeleteAsync(user);
        if (result.Succeeded)
            return Ok(new { message = "تم حذف المستخدم" });

        var errors = result.Errors.Select(e => e.Description);
        return BadRequest(new { errors });
    }
}
