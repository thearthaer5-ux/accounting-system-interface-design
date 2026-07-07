using EFA.Application.DTOs;
using EFA.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EFA.Web.Controllers;

[Authorize]
public class GroupController : Controller
{
    private readonly IGroupService _groupService;
    private readonly IPrivilegeRepository _privilegeRepository;
    private readonly ILogger<GroupController> _logger;

    public GroupController(
        IGroupService groupService,
        IPrivilegeRepository privilegeRepository,
        ILogger<GroupController> logger)
    {
        _groupService = groupService;
        _privilegeRepository = privilegeRepository;
        _logger = logger;
    }

    public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10)
    {
        var groups = await _groupService.GetAllGroupsAsync(pageNumber, pageSize);
        return View(groups);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateGroupDto model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _groupService.CreateGroupAsync(model);

        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        TempData["SuccessMessage"] = "تم إنشاء المجموعة بنجاح";
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Edit(int id)
    {
        var group = await _groupService.GetGroupByIdAsync(id);
        if (group == null)
            return NotFound();

        var updateDto = new UpdateGroupDto
        {
            GroupId = group.GroupId,
            GroupCode = group.GroupCode,
            GroupName = group.GroupName,
            Description = group.Description,
            IsActive = group.IsActive
        };

        return View(updateDto);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(UpdateGroupDto model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _groupService.UpdateGroupAsync(model);

        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        TempData["SuccessMessage"] = "تم تحديث المجموعة بنجاح";
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Details(int id)
    {
        var group = await _groupService.GetGroupDetailsAsync(id);
        if (group == null)
            return NotFound();

        return View(group);
    }

    public async Task<IActionResult> AssignPrivileges(int id)
    {
        var group = await _groupService.GetGroupByIdAsync(id);
        if (group == null)
            return NotFound();

        ViewBag.GroupId = id;
        ViewBag.GroupName = group.GroupName;
        ViewBag.Privileges = await _privilegeRepository.GetAllAsync();

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> AssignPrivileges(int groupId, List<int> privilegeIds)
    {
        var result = await _groupService.AssignPrivilegesToGroupAsync(groupId, privilegeIds ?? new List<int>());

        if (!result.Success)
        {
            TempData["ErrorMessage"] = result.Message;
            return RedirectToAction("AssignPrivileges", new { id = groupId });
        }

        TempData["SuccessMessage"] = "تم تعيين الصلاحيات بنجاح";
        return RedirectToAction("Details", new { id = groupId });
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _groupService.DeleteGroupAsync(id);

        if (result.Success)
            TempData["SuccessMessage"] = "تم حذف المجموعة بنجاح";
        else
            TempData["ErrorMessage"] = result.Message;

        return RedirectToAction("Index");
    }
}
