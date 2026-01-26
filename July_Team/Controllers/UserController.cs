using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using July_Team.Models;
using July_Team.Services;
using July_Team.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;


public class UserController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IUserRoleService _userRoleService;
    private const string SOURCE = "UserController";

    
    public UserController(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        RoleManager<IdentityRole> roleManager,
        IUserRoleService userRoleService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _userRoleService = userRoleService;
    }


    [HttpGet]
    public IActionResult Register() 
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegesterModel model) 
    {
        if (ModelState.IsValid)
        {
            var user = new IdentityUser { UserName = model.User_Email, Email = model.User_Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
        return View(model);
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginModel model)
    {
        if (ModelState.IsValid)
        {
            var result = await _signInManager.PasswordSignInAsync(model.User_Email, model.Password, model.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            ModelState.AddModelError(string.Empty, "فشل تسجيل الدخول. الرجاء التحقق من البريد الإلكتروني وكلمة المرور.");
        }
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }


    // ===================== Users Management =====================

    [Authorize(Roles = "super admin")]
    [HttpGet]
    public IActionResult Users()
    {
        var users = _userManager.Users.ToList();
        return View(users);
    }


    [HttpGet]
    public async Task<IActionResult> Create_Role()
    {
        return View();
    }

    [Authorize(Roles = "super admin")]
    [HttpGet]
	public async Task<IActionResult> Edit_Role(string userId)
	{
		if (string.IsNullOrEmpty(userId)) return RedirectToAction("Index");

		var user = await _userManager.FindByIdAsync(userId);
		if (user == null) return NotFound();

        var allRoles = await _roleManager.Roles.ToListAsync();
        var userRoles = await _userManager.GetRolesAsync(user);

		var model = new UserRolesViewModel
		{
			UserId = user.Id,
			Email = user.Email,
			Roles = allRoles.Select(r => new RolesViewModel
			{
				RoleName = r.Name,
				IsSelected = userRoles.Contains(r.Name)
			}).ToList()
		};
		return View(model);
	}

    [Authorize(Roles = "super admin")]
    [HttpPost]
    public async Task<IActionResult> Edit_Role(UserRolesViewModel model)
    {
        var user = await _userManager.FindByIdAsync(model.UserId);
        if (user == null) return NotFound();

        // مسح الرتب القديمة
        var currentRoles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, currentRoles);

        // إضافة الرتب الجديدة المختارة
        var selectedRoles = model.Roles.Where(r => r.IsSelected).Select(r => r.RoleName);
        await _userManager.AddToRolesAsync(user, selectedRoles);

        return RedirectToAction("Index"); // العودة لجدول المستخدمين بعد الحفظ
    }
  
    

    [HttpGet]
    public async Task<IActionResult> Profile()
    {
        LoggingHelper.LogInfo(SOURCE, "User accessing profile page");

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            LoggingHelper.LogWarning(SOURCE, "Profile access attempted without authentication");
            return RedirectToAction("Login");
        }

        try
        {
            // Get enhanced profile information with role details
            var profileInfo = await _userRoleService.GetUserProfileInfoAsync(user);
            
            // Pass the enhanced profile info to the view
            ViewBag.ProfileInfo = profileInfo;
            ViewBag.Roles = profileInfo.Roles;
            ViewBag.RoleDetails = profileInfo.RoleDetails;
            ViewBag.ControlLevel = profileInfo.HighestControlLevel;

            LoggingHelper.LogSuccess(SOURCE, $"Profile loaded for user: {user.Email}");
            return View(user);
        }
        catch (Exception ex)
        {
            // Log the error but still show profile with basic info
            LoggingHelper.LogError(SOURCE, "Error loading enhanced profile info", ex);
            
            // Fall back to basic role info from UserManager
            var basicRoles = await _userManager.GetRolesAsync(user);
            ViewBag.Roles = basicRoles;
            ViewBag.RoleDetails = new List<RoleDetailInfo>();
            ViewBag.ControlLevel = 0;
            ViewBag.ErrorMessage = "Could not load complete role details.";
            
            return View(user);
        }
    }

    /// <summary>
    /// Returns detailed role information as JSON for AJAX requests.
    /// Useful for dynamic UI updates without full page reload.
    /// </summary>

    [HttpGet]
    public async Task<IActionResult> GetRoleDetails()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Json(new { success = false, message = "Not authenticated" });
        }

        var profileInfo = await _userRoleService.GetUserProfileInfoAsync(user);

        return Json(new
        {
            success = true,
            userName = user.UserName, // 👈 أضفنا اسم المستخدم
            roles = profileInfo.RoleDetails, // هذه تحتوي على الـ Permissions
            controlLevel = profileInfo.HighestControlLevel
        });
    }


    [AllowAnonymous] 
    public IActionResult AccessDenied()
    {
        return View();
    }
}

