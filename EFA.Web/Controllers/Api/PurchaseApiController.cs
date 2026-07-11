using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EFA.Application.Services;
using EFA.Application.DTOs;

namespace EFA.Web.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PurchaseApiController : ControllerBase
    {
        private readonly IVendorService _vendorService;
        private readonly IPurchaseOrderService _poService;
        private readonly IPurchaseInvoiceService _invoiceService;
        private readonly IPurchaseReturnService _returnService;
        private readonly IVendorBalanceService _balanceService;

        public PurchaseApiController(
            IVendorService vendorService,
            IPurchaseOrderService poService,
            IPurchaseInvoiceService invoiceService,
            IPurchaseReturnService returnService,
            IVendorBalanceService balanceService)
        {
            _vendorService = vendorService;
            _poService = poService;
            _invoiceService = invoiceService;
            _returnService = returnService;
            _balanceService = balanceService;
        }

        // Vendors Endpoints
        [HttpGet("vendors")]
        public async Task<ActionResult<List<VendorDto>>> GetVendors()
        {
            try
            {
                var vendors = await _vendorService.GetAllVendorsAsync();
                return Ok(vendors);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("vendors/{id}")]
        public async Task<ActionResult<VendorDto>> GetVendor(int id)
        {
            try
            {
                var vendor = await _vendorService.GetVendorByIdAsync(id);
                return Ok(vendor);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost("vendors")]
        public async Task<ActionResult<int>> CreateVendor(VendorCreateUpdateDto dto)
        {
            try
            {
                var userId = User.FindFirst("Id")?.Value;
                if (!int.TryParse(userId, out int uid))
                    return Unauthorized();

                var vendorId = await _vendorService.CreateVendorAsync(dto, uid);
                return CreatedAtAction(nameof(GetVendor), new { id = vendorId }, vendorId);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("vendors/{id}")]
        public async Task<IActionResult> UpdateVendor(int id, VendorCreateUpdateDto dto)
        {
            try
            {
                var userId = User.FindFirst("Id")?.Value;
                if (!int.TryParse(userId, out int uid))
                    return Unauthorized();

                var result = await _vendorService.UpdateVendorAsync(id, dto, uid);
                if (!result) return NotFound();
                return Ok(new { message = "تم التحديث بنجاح" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("vendors/{id}/balance")]
        public async Task<ActionResult<decimal>> GetVendorBalance(int id)
        {
            try
            {
                var balance = await _vendorService.GetVendorTotalBalanceAsync(id);
                return Ok(new { vendorId = id, balance });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("vendors/search/{term}")]
        public async Task<ActionResult<List<VendorDto>>> SearchVendors(string term)
        {
            try
            {
                var results = await _vendorService.SearchVendorsAsync(term);
                return Ok(results);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Purchase Orders
        [HttpGet("orders/pending")]
        public async Task<ActionResult<List<PurchaseOrderDto>>> GetPendingOrders()
        {
            try
            {
                var orders = await _poService.GetPendingPurchaseOrdersAsync();
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("orders/{id}")]
        public async Task<ActionResult<PurchaseOrderDto>> GetOrder(int id)
        {
            try
            {
                var order = await _poService.GetPurchaseOrderAsync(id);
                return Ok(order);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost("orders/{id}/receive")]
        public async Task<IActionResult> ReceiveOrder(int id, [FromBody] List<int> quantities)
        {
            try
            {
                var userId = User.FindFirst("Id")?.Value;
                if (!int.TryParse(userId, out int uid))
                    return Unauthorized();

                await _poService.ReceivePurchaseOrderAsync(id, quantities, uid);
                return Ok(new { message = "تم استقبال المنتجات بنجاح" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Purchase Invoices
        [HttpGet("invoices/unpaid/{vendorId}")]
        public async Task<ActionResult<List<PurchaseInvoiceDto>>> GetUnpaidInvoices(int vendorId)
        {
            try
            {
                var invoices = await _invoiceService.GetUnpaidInvoicesAsync(vendorId);
                return Ok(invoices);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("invoices/{id}")]
        public async Task<ActionResult<PurchaseInvoiceDto>> GetInvoice(int id)
        {
            try
            {
                var invoice = await _invoiceService.GetInvoiceAsync(id);
                return Ok(invoice);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost("invoices/{id}/payment")]
        public async Task<IActionResult> RecordPayment(int id, [FromBody] decimal amount)
        {
            try
            {
                var userId = User.FindFirst("Id")?.Value;
                if (!int.TryParse(userId, out int uid))
                    return Unauthorized();

                await _invoiceService.RecordPaymentAsync(id, amount, "API", uid);
                return Ok(new { message = "تم تسجيل الدفعة بنجاح" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Vendor Balances
        [HttpGet("balances")]
        public async Task<ActionResult<List<VendorBalanceDto>>> GetAllBalances()
        {
            try
            {
                var balances = await _balanceService.GetAllVendorBalancesAsync();
                return Ok(balances);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("balances/recalculate")]
        public async Task<IActionResult> RecalculateBalances()
        {
            try
            {
                await _balanceService.RecalculateAllBalancesAsync();
                return Ok(new { message = "تم إعادة حساب جميع الأرصدة بنجاح" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Purchase Returns
        [HttpGet("returns/{vendorId}")]
        public async Task<ActionResult<List<PurchaseReturnDto>>> GetReturnsByVendor(int vendorId)
        {
            try
            {
                var returns = await _returnService.GetReturnsByVendorAsync(vendorId);
                return Ok(returns);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("returns/{id}")]
        public async Task<ActionResult<PurchaseReturnDto>> GetReturn(int id)
        {
            try
            {
                var ret = await _returnService.GetReturnAsync(id);
                return Ok(ret);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost("returns")]
        public async Task<ActionResult<int>> CreateReturn([FromBody] PurchaseReturnDto dto)
        {
            try
            {
                var userId = User.FindFirst("Id")?.Value;
                if (!int.TryParse(userId, out int uid))
                    return Unauthorized();

                var returnId = await _returnService.CreateReturnAsync(dto, uid);
                return CreatedAtAction(nameof(GetReturn), new { id = returnId }, returnId);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
