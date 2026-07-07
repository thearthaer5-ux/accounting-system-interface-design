using EFA.Application.DTOs;
using EFA.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EFA.Web.Controllers;

[Authorize]
public class UserManagementController : Controller
{
    private readonly IUserService _userService;
    private readonly ILogger<UserManagementController> _logger;

    public UserManagementController(IUserService userService, ILogger<UserManagementController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10)
    {
        var users = await _userService.GetAllUsersAsync(pageNumber, pageSize);
        return View(users);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateUserDto model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _userService.RegisterAsync(model);

        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        TempData["SuccessMessage"] = "تم إنشاء المستخدم بنجاح";
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Edit(int id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
            return NotFound();

        var updateDto = new UpdateUserDto
        {
            UserId = user.UserId,
            Email = user.Email,
            FullName = user.FullName,
            PhoneNumber = user.PhoneNumber,
            IsActive = user.IsActive,
            GroupId = user.GroupId,
            BranchId = user.BranchId
        };

        return View(updateDto);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(UpdateUserDto model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _userService.UpdateUserAsync(model);

        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        TempData["SuccessMessage"] = "تم تحديث المستخدم بنجاح";
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Details(int id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
            return NotFound();

        return View(user);
    }

    [HttpPost]
    public async Task<IActionResult> Deactivate(int id)
    {
        var result = await _userService.DeactivateUserAsync(id);

        if (result.Success)
            TempData["SuccessMessage"] = "تم تعطيل المستخدم بنجاح";
        else
            TempData["ErrorMessage"] = result.Message;

        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Activate(int id)
    {
        var result = await _userService.ActivateUserAsync(id);

        if (result.Success)
            TempData["SuccessMessage"] = "تم تفعيل المستخدم بنجاح";
        else
            TempData["ErrorMessage"] = result.Message;

        return RedirectToAction("Index");
    }
}
