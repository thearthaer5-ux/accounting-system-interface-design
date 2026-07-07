using EFA.Application.DTOs;
using EFA.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EFA.Web.Controllers;

[Authorize]
public class BranchController : Controller
{
    private readonly IBranchService _branchService;
    private readonly ILogger<BranchController> _logger;

    public BranchController(IBranchService branchService, ILogger<BranchController> logger)
    {
        _branchService = branchService;
        _logger = logger;
    }

    public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10)
    {
        var branches = await _branchService.GetAllBranchesAsync(pageNumber, pageSize);
        return View(branches);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(BranchDto model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _branchService.CreateBranchAsync(model);

        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        TempData["SuccessMessage"] = "تم إنشاء الفرع بنجاح";
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Edit(int id)
    {
        var branch = await _branchService.GetBranchByIdAsync(id);
        if (branch == null)
            return NotFound();

        return View(branch);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(BranchDto model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _branchService.UpdateBranchAsync(model);

        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        TempData["SuccessMessage"] = "تم تحديث الفرع بنجاح";
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Details(int id)
    {
        var branch = await _branchService.GetBranchByIdAsync(id);
        if (branch == null)
            return NotFound();

        return View(branch);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _branchService.DeleteBranchAsync(id);

        if (result.Success)
            TempData["SuccessMessage"] = "تم حذف الفرع بنجاح";
        else
            TempData["ErrorMessage"] = result.Message;

        return RedirectToAction("Index");
    }
}
