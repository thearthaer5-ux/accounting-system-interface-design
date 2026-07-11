using Microsoft.AspNetCore.Mvc;
using EFA.Application.Services;
using EFA.Application.DTOs;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace EFA.Web.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalesApiController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly ISalesOrderService _orderService;
        private readonly ISalesInvoiceService _invoiceService;
        private readonly ISalesReturnService _returnService;
        private readonly ICustomerBalanceService _balanceService;

        public SalesApiController(
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
        [HttpGet("customers")]
        public async Task<ActionResult<List<CustomerDto>>> GetAllCustomers(int page = 1, int pageSize = 10, string search = "")
        {
            var customers = await _customerService.GetAllCustomersAsync(page, pageSize, search);
            return Ok(customers);
        }

        [HttpGet("customer/{id}")]
        public async Task<ActionResult<CustomerDto>> GetCustomer(int id)
        {
            var customer = await _customerService.GetCustomerByIdAsync(id);
            if (customer == null)
                return NotFound(new { message = "Customer not found" });
            return Ok(customer);
        }

        [HttpGet("customers/active")]
        public async Task<ActionResult<List<CustomerDto>>> GetActiveCustomers()
        {
            var customers = await _customerService.GetActiveCustomersAsync();
            return Ok(customers);
        }

        [HttpPost("customer")]
        public async Task<ActionResult<int>> CreateCustomer(CreateCustomerDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var customerId = await _customerService.CreateCustomerAsync(dto);
            return CreatedAtAction(nameof(GetCustomer), new { id = customerId }, customerId);
        }

        [HttpPut("customer/{id}")]
        public async Task<IActionResult> UpdateCustomer(int id, CreateCustomerDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _customerService.UpdateCustomerAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("customer/{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            await _customerService.DeleteCustomerAsync(id);
            return NoContent();
        }

        // Sales Orders
        [HttpGet("orders")]
        public async Task<ActionResult<List<SalesOrderDto>>> GetAllOrders(int page = 1, int pageSize = 10)
        {
            var orders = await _orderService.GetAllOrdersAsync(page, pageSize);
            return Ok(orders);
        }

        [HttpGet("order/{id}")]
        public async Task<ActionResult<SalesOrderDto>> GetOrder(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
                return NotFound(new { message = "Order not found" });
            return Ok(order);
        }

        [HttpPost("order")]
        public async Task<ActionResult<int>> CreateOrder(CreateSalesOrderDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var orderId = await _orderService.CreateSalesOrderAsync(dto);
            return CreatedAtAction(nameof(GetOrder), new { id = orderId }, orderId);
        }

        [HttpPut("order/{id}")]
        public async Task<IActionResult> UpdateOrder(int id, CreateSalesOrderDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _orderService.UpdateSalesOrderAsync(id, dto);
            return NoContent();
        }

        // Sales Invoices
        [HttpGet("invoices")]
        public async Task<ActionResult<List<SalesInvoiceDto>>> GetAllInvoices(int page = 1, int pageSize = 10)
        {
            var invoices = await _invoiceService.GetAllInvoicesAsync(page, pageSize);
            return Ok(invoices);
        }

        [HttpGet("invoice/{id}")]
        public async Task<ActionResult<SalesInvoiceDto>> GetInvoice(int id)
        {
            var invoice = await _invoiceService.GetInvoiceByIdAsync(id);
            if (invoice == null)
                return NotFound(new { message = "Invoice not found" });
            return Ok(invoice);
        }

        [HttpPost("invoice")]
        public async Task<ActionResult<int>> CreateInvoice(CreateSalesInvoiceDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var invoiceId = await _invoiceService.CreateSalesInvoiceAsync(dto);
            return CreatedAtAction(nameof(GetInvoice), new { id = invoiceId }, invoiceId);
        }

        [HttpPut("invoice/{id}")]
        public async Task<IActionResult> UpdateInvoice(int id, CreateSalesInvoiceDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _invoiceService.UpdateSalesInvoiceAsync(id, dto);
            return NoContent();
        }

        [HttpPost("invoice/{id}/post")]
        public async Task<IActionResult> PostInvoice(int id)
        {
            await _invoiceService.PostInvoiceToAccountingAsync(id);
            return Ok(new { message = "Invoice posted to accounting" });
        }

        // Sales Returns
        [HttpGet("returns")]
        public async Task<ActionResult<List<SalesReturnDto>>> GetAllReturns(int page = 1, int pageSize = 10)
        {
            var returns = await _returnService.GetAllReturnsAsync(page, pageSize);
            return Ok(returns);
        }

        [HttpGet("return/{id}")]
        public async Task<ActionResult<SalesReturnDto>> GetReturn(int id)
        {
            var salesReturn = await _returnService.GetReturnByIdAsync(id);
            if (salesReturn == null)
                return NotFound(new { message = "Return not found" });
            return Ok(salesReturn);
        }

        [HttpPost("return")]
        public async Task<ActionResult<int>> CreateReturn(CreateSalesReturnDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var returnId = await _returnService.CreateSalesReturnAsync(dto);
            return CreatedAtAction(nameof(GetReturn), new { id = returnId }, returnId);
        }

        // Customer Balances
        [HttpGet("customer-balance/{customerId}")]
        public async Task<ActionResult<CustomerBalanceDto>> GetCustomerBalance(int customerId)
        {
            var balance = await _balanceService.GetCustomerBalanceAsync(customerId);
            if (balance == null)
                return NotFound(new { message = "Balance not found" });
            return Ok(balance);
        }

        [HttpGet("overdue-balances")]
        public async Task<ActionResult<List<CustomerBalanceDto>>> GetOverdueBalances()
        {
            var balances = await _balanceService.GetOverdueBalancesAsync();
            return Ok(balances);
        }

        [HttpPut("customer-balance/{customerId}/update")]
        public async Task<IActionResult> UpdateCustomerBalance(int customerId, [FromBody] decimal amount)
        {
            await _balanceService.UpdateCustomerBalanceAsync(customerId, amount);
            return Ok(new { message = "Balance updated" });
        }
    }
}
