using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EFA.Application.Services;
using EFA.Application.DTOs;

namespace EFA.Web.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class PurchaseController : Controller
    {
        private readonly IVendorService _vendorService;
        private readonly IPurchaseOrderService _poService;
        private readonly IPurchaseInvoiceService _invoiceService;
        private readonly IPurchaseReturnService _returnService;
        private readonly IVendorBalanceService _balanceService;

        public PurchaseController(
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

        // Vendor Management
        [HttpGet("vendors")]
        public async Task<IActionResult> Vendors()
        {
            try
            {
                var vendors = await _vendorService.GetAllVendorsAsync();
                return View(vendors);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet("vendors/create")]
        public IActionResult CreateVendor()
        {
            return View();
        }

        [HttpPost("vendors/create")]
        public async Task<IActionResult> CreateVendor(VendorCreateUpdateDto dto)
        {
            try
            {
                var userId = User.FindFirst("Id")?.Value;
                if (!int.TryParse(userId, out int uid)) return Unauthorized();

                var vendorId = await _vendorService.CreateVendorAsync(dto, uid);
                TempData["Success"] = $"تم إنشاء الموردين بنجاح";
                return RedirectToAction(nameof(VendorDetails), new { id = vendorId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(dto);
            }
        }

        [HttpGet("vendors/{id}")]
        public async Task<IActionResult> VendorDetails(int id)
        {
            try
            {
                var vendor = await _vendorService.GetVendorByIdAsync(id);
                return View(vendor);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Vendors));
            }
        }

        // Purchase Orders
        [HttpGet("orders")]
        public async Task<IActionResult> PurchaseOrders()
        {
            try
            {
                var orders = await _poService.GetPendingPurchaseOrdersAsync();
                return View(orders);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet("orders/{id}")]
        public async Task<IActionResult> PurchaseOrderDetails(int id)
        {
            try
            {
                var order = await _poService.GetPurchaseOrderAsync(id);
                return View(order);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(PurchaseOrders));
            }
        }

        [HttpPost("orders/{id}/receive")]
        public async Task<IActionResult> ReceivePurchaseOrder(int id, List<int> quantities)
        {
            try
            {
                var userId = User.FindFirst("Id")?.Value;
                if (!int.TryParse(userId, out int uid)) return Unauthorized();

                await _poService.ReceivePurchaseOrderAsync(id, quantities, uid);
                TempData["Success"] = "تم استلام المنتجات بنجاح";
                return RedirectToAction(nameof(PurchaseOrderDetails), new { id });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(PurchaseOrderDetails), new { id });
            }
        }

        // Purchase Invoices
        [HttpGet("invoices")]
        public async Task<IActionResult> PurchaseInvoices()
        {
            try
            {
                var invoices = await _invoiceService.GetUnpaidInvoicesAsync(0);
                return View(invoices);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet("invoices/{id}")]
        public async Task<IActionResult> InvoiceDetails(int id)
        {
            try
            {
                var invoice = await _invoiceService.GetInvoiceAsync(id);
                return View(invoice);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(PurchaseInvoices));
            }
        }

        [HttpPost("invoices/{id}/pay")]
        public async Task<IActionResult> RecordPayment(int id, decimal amount)
        {
            try
            {
                var userId = User.FindFirst("Id")?.Value;
                if (!int.TryParse(userId, out int uid)) return Unauthorized();

                await _invoiceService.RecordPaymentAsync(id, amount, "Transfer", uid);
                TempData["Success"] = $"تم تسجيل دفعة بمبلغ {amount}";
                return RedirectToAction(nameof(InvoiceDetails), new { id });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(InvoiceDetails), new { id });
            }
        }

        // Vendor Balance
        [HttpGet("balances")]
        public async Task<IActionResult> VendorBalances()
        {
            try
            {
                var balances = await _balanceService.GetAllVendorBalancesAsync();
                return View(balances);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        // Purchase Returns
        [HttpGet("returns")]
        public async Task<IActionResult> PurchaseReturns()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchVendors(string term)
        {
            try
            {
                var results = await _vendorService.SearchVendorsAsync(term);
                return Json(results);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }
    }
}
