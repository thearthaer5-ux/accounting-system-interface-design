using EFA.Application.DTOs;
using EFA.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EFA.Web.Controllers
{
    [Authorize]
    public class InventoryController : Controller
    {
        private readonly IInventoryService _inventoryService;
        private readonly IWarehouseService _warehouseService;
        private readonly IItemService _itemService;
        private readonly ILogger<InventoryController> _logger;

        public InventoryController(IInventoryService inventoryService, IWarehouseService warehouseService, 
            IItemService itemService, ILogger<InventoryController> logger)
        {
            _inventoryService = inventoryService;
            _warehouseService = warehouseService;
            _itemService = itemService;
            _logger = logger;
        }

        public async Task<IActionResult> Balances(int warehouseId)
        {
            try
            {
                var balances = await _inventoryService.GetWarehouseBalancesAsync(warehouseId);
                ViewData["WarehouseId"] = warehouseId;
                return View(balances);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في جلب الأرصدة");
                TempData["ErrorMessage"] = "فشل جلب الأرصدة";
                return RedirectToAction("Index", "Warehouse");
            }
        }

        public async Task<IActionResult> ItemBalances(int itemId)
        {
            try
            {
                var balances = await _inventoryService.GetItemBalancesAsync(itemId);
                ViewData["ItemId"] = itemId;
                return View(balances);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في جلب أرصدة الصنف");
                return View(new List<ItemBalanceDto>());
            }
        }

        public async Task<IActionResult> Movements(DateTime? fromDate, DateTime? toDate)
        {
            try
            {
                var from = fromDate ?? DateTime.Now.AddMonths(-1);
                var to = toDate ?? DateTime.Now;

                var movements = await _inventoryService.GetMovementsAsync(from, to);
                ViewData["FromDate"] = from;
                ViewData["ToDate"] = to;

                return View(movements);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في جلب الحركات");
                return View(new List<ItemMovementDto>());
            }
        }

        public async Task<IActionResult> AddMovement()
        {
            try
            {
                var warehouses = await _warehouseService.GetAllAsync();
                var items = await _itemService.GetActiveAsync();
                ViewData["Warehouses"] = warehouses;
                ViewData["Items"] = items;
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في تحميل البيانات");
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddMovement(ItemMovementCreateDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(dto);

                var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
                await _inventoryService.AddMovementAsync(dto, userId);

                TempData["SuccessMessage"] = "تم تسجيل الحركة بنجاح";
                return RedirectToAction(nameof(Movements));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في إضافة الحركة");
                ModelState.AddModelError("", "فشل إضافة الحركة");
                return View(dto);
            }
        }

        [HttpPost]
        public async Task<IActionResult> PostMovement(int movementId)
        {
            try
            {
                await _inventoryService.PostMovementAsync(movementId);
                return Json(new { success = true, message = "تم ترحيل الحركة بنجاح" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في ترحيل الحركة");
                return Json(new { success = false, message = "فشل ترحيل الحركة" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetItemBalances(int itemId)
        {
            try
            {
                var balances = await _inventoryService.GetItemBalancesAsync(itemId);
                return Json(balances);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في جلب البيانات");
                return Json(new { error = "فشل جلب البيانات" });
            }
        }
    }
}
