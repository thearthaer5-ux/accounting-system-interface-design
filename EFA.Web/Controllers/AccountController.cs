using EFA.Application.DTOs;
using EFA.Application.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EFA.Web.Controllers;

/// <summary>
/// متحكم حسابات المستخدمين
/// يدير تسجيل الدخول والخروج والتسجيل
/// </summary>
public class AccountController : Controller
{
    private readonly IUserService _userService;
    private readonly ILogger<AccountController> _logger;

    public AccountController(IUserService userService, ILogger<AccountController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Login()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Home");

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginDto model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _userService.LoginAsync(model);

        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        var user = result.Data;
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user!.UserId.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim("FullName", user.FullName ?? "")
        };

        var claimsIdentity = new ClaimsIdentity(claims, "DefaultScheme");
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = model.RememberMe,
            ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
        };

        await HttpContext.SignInAsync(
            "DefaultScheme",
            new ClaimsPrincipal(claimsIdentity),
            authProperties);

        _logger.LogInformation($"المستخدم {user.Username} قام بتسجيل الدخول");

        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(CreateUserDto model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _userService.RegisterAsync(model);

        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        TempData["SuccessMessage"] = "تم إنشاء الحساب بنجاح. يرجى تسجيل الدخول";
        return RedirectToAction("Login");
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync("DefaultScheme");
        _logger.LogInformation($"المستخدم {User.Identity?.Name} قام بتسجيل الخروج");
        return RedirectToAction("Login");
    }

    [HttpGet]
    public async Task<IActionResult> Profile()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
            return RedirectToAction("Login");

        var user = await _userService.GetUserByIdAsync(userId);
        if (user == null)
            return RedirectToAction("Login");

        return View(user);
    }

    [HttpGet]
    public IActionResult ChangePassword()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ChangePassword(ChangePasswordDto model)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
            return RedirectToAction("Login");

        model.UserId = userId;

        if (!ModelState.IsValid)
            return View(model);

        var result = await _userService.ChangePasswordAsync(model);

        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View();
        }

        TempData["SuccessMessage"] = "تم تغيير كلمة المرور بنجاح";
        return RedirectToAction("Profile");
    }

    public IActionResult AccessDenied()
    {
        return View();
    }
}
