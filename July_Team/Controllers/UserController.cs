using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using July_Team.Models;
using July_Team.Services;
using July_Team.Helpers;

/// <summary>
/// Controller handling user authentication and profile management.
/// Uses dependency injection to access identity services and custom role service.
/// </summary>
public class UserController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly IUserRoleService _userRoleService;
    private const string SOURCE = "UserController";

    /// <summary>
    /// Constructor with dependency injection.
    /// The IUserRoleService is now injected to provide enhanced role information.
    /// </summary>
    public UserController(
        UserManager<IdentityUser> userManager, 
        SignInManager<IdentityUser> signInManager,
        IUserRoleService userRoleService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
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






    /// <summary>
    /// Displays the user profile with enhanced role information.
    /// Now includes permissions, control levels, and professional role details.
    /// </summary>
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
            roles = profileInfo.RoleDetails,
            controlLevel = profileInfo.HighestControlLevel
        });
    }
}

