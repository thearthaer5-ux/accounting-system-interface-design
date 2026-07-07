using EFA.Application.DTOs;
using EFA.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EFA.Web.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class InventoryApiController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;
        private readonly IItemService _itemService;
        private readonly IWarehouseService _warehouseService;
        private readonly ILogger<InventoryApiController> _logger;

        public InventoryApiController(IInventoryService inventoryService, IItemService itemService, 
            IWarehouseService warehouseService, ILogger<InventoryApiController> logger)
        {
            _inventoryService = inventoryService;
            _itemService = itemService;
            _warehouseService = warehouseService;
            _logger = logger;
        }

        [HttpGet("balance/{itemId}/{warehouseId}")]
        public async Task<ActionResult<ItemBalanceDto>> GetBalance(int itemId, int warehouseId)
        {
            try
            {
                var balance = await _inventoryService.GetBalanceAsync(itemId, warehouseId);
                return Ok(balance);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في جلب الرصيد");
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("warehouse-balances/{warehouseId}")]
        public async Task<ActionResult<List<ItemBalanceDto>>> GetWarehouseBalances(int warehouseId)
        {
            try
            {
                var balances = await _inventoryService.GetWarehouseBalancesAsync(warehouseId);
                return Ok(balances);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في جلب أرصدة المستودع");
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("item-balances/{itemId}")]
        public async Task<ActionResult<List<ItemBalanceDto>>> GetItemBalances(int itemId)
        {
            try
            {
                var balances = await _inventoryService.GetItemBalancesAsync(itemId);
                return Ok(balances);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في جلب أرصدة الصنف");
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("warehouse-value/{warehouseId}")]
        public async Task<ActionResult<decimal>> GetWarehouseValue(int warehouseId)
        {
            try
            {
                var value = await _inventoryService.GetWarehouseValueAsync(warehouseId);
                return Ok(new { value });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في حساب قيمة المستودع");
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("movement")]
        public async Task<ActionResult<ItemBalanceDto>> AddMovement(ItemMovementCreateDto dto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
                var balance = await _inventoryService.AddMovementAsync(dto, userId);
                return Ok(balance);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في إضافة الحركة");
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("post-movement/{movementId}")]
        public async Task<IActionResult> PostMovement(int movementId)
        {
            try
            {
                var result = await _inventoryService.PostMovementAsync(movementId);
                return Ok(new { success = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في ترحيل الحركة");
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("movements")]
        public async Task<ActionResult<List<ItemMovementDto>>> GetMovements([FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
        {
            try
            {
                var movements = await _inventoryService.GetMovementsAsync(fromDate, toDate);
                return Ok(movements);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في جلب الحركات");
                return BadRequest(new { error = ex.Message });
            }
        }
    }

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ItemsApiController : ControllerBase
    {
        private readonly IItemService _itemService;
        private readonly ILogger<ItemsApiController> _logger;

        public ItemsApiController(IItemService itemService, ILogger<ItemsApiController> logger)
        {
            _itemService = itemService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<ItemDto>>> GetAll()
        {
            try
            {
                var items = await _itemService.GetActiveAsync();
                return Ok(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في جلب الأصناف");
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ItemDto>> GetById(int id)
        {
            try
            {
                var item = await _itemService.GetByIdAsync(id);
                return Ok(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في جلب الصنف");
                return NotFound(new { error = ex.Message });
            }
        }

        [HttpGet("code/{code}")]
        public async Task<ActionResult<ItemDto>> GetByCode(string code)
        {
            try
            {
                var item = await _itemService.GetByCodeAsync(code);
                return Ok(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في جلب الصنف");
                return NotFound(new { error = ex.Message });
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<List<ItemDto>>> Search([FromQuery] string searchTerm)
        {
            try
            {
                var items = await _itemService.SearchAsync(searchTerm);
                return Ok(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في البحث");
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<ItemDto>> Create(ItemCreateUpdateDto dto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
                var item = await _itemService.CreateAsync(dto, userId);
                return CreatedAtAction(nameof(GetById), new { id = item.ItemId }, item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في إنشاء صنف جديد");
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ItemDto>> Update(int id, ItemCreateUpdateDto dto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
                var item = await _itemService.UpdateAsync(id, dto, userId);
                return Ok(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في تحديث الصنف");
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _itemService.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في حذف الصنف");
                return BadRequest(new { error = ex.Message });
            }
        }
    }

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class WarehousesApiController : ControllerBase
    {
        private readonly IWarehouseService _warehouseService;
        private readonly ILogger<WarehousesApiController> _logger;

        public WarehousesApiController(IWarehouseService warehouseService, ILogger<WarehousesApiController> logger)
        {
            _warehouseService = warehouseService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<WarehouseDto>>> GetAll()
        {
            try
            {
                var warehouses = await _warehouseService.GetAllAsync();
                return Ok(warehouses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في جلب المستودعات");
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<WarehouseDto>> GetById(int id)
        {
            try
            {
                var warehouse = await _warehouseService.GetByIdAsync(id);
                return Ok(warehouse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في جلب المستودع");
                return NotFound(new { error = ex.Message });
            }
        }

        [HttpGet("branch/{branchId}")]
        public async Task<ActionResult<List<WarehouseDto>>> GetByBranch(int branchId)
        {
            try
            {
                var warehouses = await _warehouseService.GetByBranchAsync(branchId);
                return Ok(warehouses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في جلب مستودعات الفرع");
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("summary/{id}")]
        public async Task<ActionResult<WarehouseInventorySummaryDto>> GetSummary(int id)
        {
            try
            {
                var summary = await _warehouseService.GetInventorySummaryAsync(id);
                return Ok(summary);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في جلب ملخص المخزون");
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
