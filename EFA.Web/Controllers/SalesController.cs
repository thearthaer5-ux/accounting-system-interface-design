using Microsoft.AspNetCore.Mvc;
using EFA.Application.Services;
using EFA.Application.DTOs;
using System.Threading.Tasks;

namespace EFA.Web.Controllers
{
    [Route("Sales")]
    public class SalesController : Controller
    {
        private readonly ICustomerService _customerService;
        private readonly ISalesOrderService _orderService;
        private readonly ISalesInvoiceService _invoiceService;
        private readonly ISalesReturnService _returnService;
        private readonly ICustomerBalanceService _balanceService;

        public SalesController(
            ICustomerService customerService,
            ISalesOrderService orderService,
            ISalesInvoiceService invoiceService,
            ISalesReturnService returnService,
            ICustomerBalanceService balanceService)
        {
            _customerService = customerService;
            _orderService = orderService;
            _invoiceService = invoiceService;
            _returnService = returnService;
            _balanceService = balanceService;
        }

        // Customers
        [HttpGet("Customers")]
        public async Task<IActionResult> Customers(int page = 1, string search = "")
        {
            var customers = await _customerService.GetAllCustomersAsync(page, 10, search);
            return View(customers);
        }

        [HttpGet("Customer/{id}")]
        public async Task<IActionResult> CustomerDetails(int id)
        {
            var customer = await _customerService.GetCustomerByIdAsync(id);
            if (customer == null)
                return NotFound();
            return View(customer);
        }

        [HttpGet("CreateCustomer")]
        public IActionResult CreateCustomer()
        {
            return View();
        }

        [HttpPost("SaveCustomer")]
        public async Task<IActionResult> SaveCustomer(CreateCustomerDto dto)
        {
            if (!ModelState.IsValid)
                return View("CreateCustomer", dto);

            await _customerService.CreateCustomerAsync(dto);
            return RedirectToAction(nameof(Customers));
        }

        // Sales Orders
        [HttpGet("Orders")]
        public async Task<IActionResult> Orders(int page = 1)
        {
            var orders = await _orderService.GetAllOrdersAsync(page, 10);
            return View(orders);
        }

        [HttpGet("Order/{id}")]
        public async Task<IActionResult> OrderDetails(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
                return NotFound();
            return View(order);
        }

        [HttpGet("CreateOrder")]
        public IActionResult CreateOrder()
        {
            return View();
        }

        [HttpPost("SaveOrder")]
        public async Task<IActionResult> SaveOrder(CreateSalesOrderDto dto)
        {
            if (!ModelState.IsValid)
                return View("CreateOrder", dto);

            await _orderService.CreateSalesOrderAsync(dto);
            return RedirectToAction(nameof(Orders));
        }

        // Sales Invoices
        [HttpGet("Invoices")]
        public async Task<IActionResult> Invoices(int page = 1)
        {
            var invoices = await _invoiceService.GetAllInvoicesAsync(page, 10);
            return View(invoices);
        }

        [HttpGet("Invoice/{id}")]
        public async Task<IActionResult> InvoiceDetails(int id)
        {
            var invoice = await _invoiceService.GetInvoiceByIdAsync(id);
            if (invoice == null)
                return NotFound();
            return View(invoice);
        }

        [HttpGet("CreateInvoice")]
        public IActionResult CreateInvoice()
        {
            return View();
        }

        [HttpPost("SaveInvoice")]
        public async Task<IActionResult> SaveInvoice(CreateSalesInvoiceDto dto)
        {
            if (!ModelState.IsValid)
                return View("CreateInvoice", dto);

            await _invoiceService.CreateSalesInvoiceAsync(dto);
            return RedirectToAction(nameof(Invoices));
        }

        // Sales Returns
        [HttpGet("Returns")]
        public async Task<IActionResult> Returns(int page = 1)
        {
            var returns = await _returnService.GetAllReturnsAsync(page, 10);
            return View(returns);
        }

        [HttpGet("Return/{id}")]
        public async Task<IActionResult> ReturnDetails(int id)
        {
            var salesReturn = await _returnService.GetReturnByIdAsync(id);
            if (salesReturn == null)
                return NotFound();
            return View(salesReturn);
        }

        [HttpGet("CreateReturn")]
        public IActionResult CreateReturn()
        {
            return View();
        }

        [HttpPost("SaveReturn")]
        public async Task<IActionResult> SaveReturn(CreateSalesReturnDto dto)
        {
            if (!ModelState.IsValid)
                return View("CreateReturn", dto);

            await _returnService.CreateSalesReturnAsync(dto);
            return RedirectToAction(nameof(Returns));
        }

        // Customer Balances
        [HttpGet("CustomerBalances")]
        public async Task<IActionResult> CustomerBalances()
        {
            var balances = await _balanceService.GetOverdueBalancesAsync();
            return View(balances);
        }

        [HttpGet("CustomerBalance/{id}")]
        public async Task<IActionResult> CustomerBalance(int id)
        {
            var balance = await _balanceService.GetCustomerBalanceAsync(id);
            if (balance == null)
                return NotFound();
            return View(balance);
        }

        // Dashboard
        [HttpGet("Dashboard")]
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}
