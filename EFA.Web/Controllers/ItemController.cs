using EFA.Application.DTOs;
using EFA.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EFA.Web.Controllers
{
    [Authorize]
    public class ItemController : Controller
    {
        private readonly IItemService _itemService;
        private readonly ILogger<ItemController> _logger;

        public ItemController(IItemService itemService, ILogger<ItemController> logger)
        {
            _itemService = itemService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var items = await _itemService.GetActiveAsync();
                return View(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في جلب الأصناف");
                TempData["ErrorMessage"] = "حدث خطأ في جلب البيانات";
                return View(new List<ItemDto>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> Search(string searchTerm)
        {
            try
            {
                var items = await _itemService.SearchAsync(searchTerm);
                return Json(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في البحث");
                return Json(new { error = "فشل البحث" });
            }
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ItemCreateUpdateDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(dto);

                var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
                await _itemService.CreateAsync(dto, userId);

                TempData["SuccessMessage"] = "تم إنشاء الصنف بنجاح";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في إنشاء صنف جديد");
                ModelState.AddModelError("", "فشل إنشاء الصنف");
                return View(dto);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var item = await _itemService.GetByIdAsync(id);
                return View(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في جلب الصنف");
                TempData["ErrorMessage"] = "الصنف غير موجود";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ItemCreateUpdateDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(dto);

                var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
                await _itemService.UpdateAsync(id, dto, userId);

                TempData["SuccessMessage"] = "تم تحديث الصنف بنجاح";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في تحديث الصنف");
                ModelState.AddModelError("", "فشل تحديث الصنف");
                return View(dto);
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _itemService.DeleteAsync(id);
                TempData["SuccessMessage"] = "تم حذف الصنف بنجاح";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في حذف الصنف");
                TempData["ErrorMessage"] = "فشل حذف الصنف";
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> LowStock()
        {
            try
            {
                var items = await _itemService.GetLowStockAsync();
                return View(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في جلب الأصناف منخفضة المخزون");
                return View(new List<ItemDto>());
            }
        }
    }
}
