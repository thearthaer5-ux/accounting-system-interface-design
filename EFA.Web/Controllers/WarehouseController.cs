using EFA.Application.DTOs;
using EFA.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EFA.Web.Controllers
{
    [Authorize]
    public class WarehouseController : Controller
    {
        private readonly IWarehouseService _warehouseService;
        private readonly IBranchService _branchService;
        private readonly ILogger<WarehouseController> _logger;

        public WarehouseController(IWarehouseService warehouseService, IBranchService branchService, ILogger<WarehouseController> logger)
        {
            _warehouseService = warehouseService;
            _branchService = branchService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var warehouses = await _warehouseService.GetAllAsync();
                return View(warehouses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في جلب المستودعات");
                TempData["ErrorMessage"] = "حدث خطأ في جلب البيانات";
                return View(new List<WarehouseDto>());
            }
        }

        public async Task<IActionResult> Create()
        {
            try
            {
                var branches = await _branchService.GetAllAsync();
                ViewData["Branches"] = branches;
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في تحميل الفروع");
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(WarehouseCreateUpdateDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(dto);

                var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
                await _warehouseService.CreateAsync(dto, userId);

                TempData["SuccessMessage"] = "تم إنشاء المستودع بنجاح";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في إنشاء مستودع جديد");
                ModelState.AddModelError("", "فشل إنشاء المستودع");
                return View(dto);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var warehouse = await _warehouseService.GetByIdAsync(id);
                var branches = await _branchService.GetAllAsync();
                ViewData["Branches"] = branches;
                return View(warehouse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في جلب المستودع");
                TempData["ErrorMessage"] = "المستودع غير موجود";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, WarehouseCreateUpdateDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(dto);

                var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
                await _warehouseService.UpdateAsync(id, dto, userId);

                TempData["SuccessMessage"] = "تم تحديث المستودع بنجاح";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في تحديث المستودع");
                ModelState.AddModelError("", "فشل تحديث المستودع");
                return View(dto);
            }
        }

        public async Task<IActionResult> Summary(int id)
        {
            try
            {
                var summary = await _warehouseService.GetInventorySummaryAsync(id);
                return View(summary);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في جلب ملخص المخزون");
                TempData["ErrorMessage"] = "فشل جلب ملخص المخزون";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
