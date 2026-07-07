using EFA.Application.DTOs;
using EFA.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EFA.Web.Controllers
{
    [Authorize]
    public class ItemCategoryController : Controller
    {
        private readonly IItemService _itemService;
        private readonly ILogger<ItemCategoryController> _logger;

        public ItemCategoryController(IItemService itemService, ILogger<ItemCategoryController> logger)
        {
            _itemService = itemService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var categories = await _itemService.GetAllAsync();
                return View(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في جلب فئات الأصناف");
                TempData["ErrorMessage"] = "حدث خطأ في جلب البيانات";
                return View(new List<ItemDto>());
            }
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ItemCategoryCreateDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(dto);

                // تنفيذ الإنشاء
                TempData["SuccessMessage"] = "تم إنشاء الفئة بنجاح";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في إنشاء فئة جديدة");
                ModelState.AddModelError("", "فشل إنشاء الفئة");
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
                _logger.LogError(ex, "خطأ في جلب الفئة للتعديل");
                TempData["ErrorMessage"] = "الفئة غير موجودة";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ItemCategoryCreateDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(dto);

                TempData["SuccessMessage"] = "تم تحديث الفئة بنجاح";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في تحديث الفئة");
                ModelState.AddModelError("", "فشل تحديث الفئة");
                return View(dto);
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _itemService.DeleteAsync(id);
                TempData["SuccessMessage"] = "تم حذف الفئة بنجاح";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في حذف الفئة");
                TempData["ErrorMessage"] = "فشل حذف الفئة";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
