using EFA.Application.DTOs;
using EFA.Domain.Entities;
using EFA.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFA.Application.Services
{
    public interface ICustomerService
    {
        Task<List<CustomerDto>> GetAllCustomersAsync(int pageNumber = 1, int pageSize = 10, string search = "");
        Task<CustomerDto> GetCustomerByIdAsync(int id);
        Task<int> CreateCustomerAsync(CreateCustomerDto dto);
        Task UpdateCustomerAsync(int id, CreateCustomerDto dto);
        Task DeleteCustomerAsync(int id);
        Task<List<CustomerDto>> GetActiveCustomersAsync();
    }

    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _repository;

        public CustomerService(ICustomerRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<CustomerDto>> GetAllCustomersAsync(int pageNumber = 1, int pageSize = 10, string search = "")
        {
            var customers = await _repository.GetAllAsync(pageNumber, pageSize, search);
            return customers.Select(c => MapToDto(c)).ToList();
        }

        public async Task<CustomerDto> GetCustomerByIdAsync(int id)
        {
            var customer = await _repository.GetByIdAsync(id);
            return customer != null ? MapToDto(customer) : null;
        }

        public async Task<int> CreateCustomerAsync(CreateCustomerDto dto)
        {
            var customer = new Customer
            {
                CustomerCode = dto.CustomerCode,
                CustomerNameAr = dto.CustomerNameAr,
                CustomerNameEn = dto.CustomerNameEn,
                PhoneNumber = dto.PhoneNumber,
                Email = dto.Email,
                Address = dto.Address,
                City = dto.City,
                Country = dto.Country,
                CreditLimit = dto.CreditLimit,
                CustomerTypeId = dto.CustomerTypeId,
                BranchId = dto.BranchId,
                CurrencyId = dto.CurrencyId,
                IsActive = true,
                IsDeleted = false
            };

            return await _repository.CreateAsync(customer);
        }

        public async Task UpdateCustomerAsync(int id, CreateCustomerDto dto)
        {
            var customer = await _repository.GetByIdAsync(id);
            if (customer != null)
            {
                customer.CustomerNameAr = dto.CustomerNameAr;
                customer.CustomerNameEn = dto.CustomerNameEn;
                customer.PhoneNumber = dto.PhoneNumber;
                customer.Email = dto.Email;
                customer.Address = dto.Address;
                customer.City = dto.City;
                customer.Country = dto.Country;
                customer.CreditLimit = dto.CreditLimit;

                await _repository.UpdateAsync(customer);
            }
        }

        public async Task DeleteCustomerAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }

        public async Task<List<CustomerDto>> GetActiveCustomersAsync()
        {
            var customers = await _repository.GetActiveCustomersAsync();
            return customers.Select(c => MapToDto(c)).ToList();
        }

        private CustomerDto MapToDto(Customer customer)
        {
            return new CustomerDto
            {
                CustomerId = customer.CustomerId,
                CustomerCode = customer.CustomerCode,
                CustomerNameAr = customer.CustomerNameAr,
                CustomerNameEn = customer.CustomerNameEn,
                PhoneNumber = customer.PhoneNumber,
                Email = customer.Email,
                Address = customer.Address,
                City = customer.City,
                CreditLimit = customer.CreditLimit,
                IsActive = customer.IsActive
            };
        }
    }

    public interface ISalesOrderService
    {
        Task<List<SalesOrderDto>> GetAllOrdersAsync(int pageNumber = 1, int pageSize = 10);
        Task<SalesOrderDto> GetOrderByIdAsync(int id);
        Task<int> CreateSalesOrderAsync(CreateSalesOrderDto dto);
        Task UpdateSalesOrderAsync(int id, CreateSalesOrderDto dto);
    }

    public class SalesOrderService : ISalesOrderService
    {
        private readonly ISalesOrderRepository _repository;

        public SalesOrderService(ISalesOrderRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<SalesOrderDto>> GetAllOrdersAsync(int pageNumber = 1, int pageSize = 10)
        {
            var orders = await _repository.GetAllAsync(pageNumber, pageSize);
            return orders.Select(o => MapToDto(o)).ToList();
        }

        public async Task<SalesOrderDto> GetOrderByIdAsync(int id)
        {
            var order = await _repository.GetByIdAsync(id);
            return order != null ? MapToDto(order) : null;
        }

        public async Task<int> CreateSalesOrderAsync(CreateSalesOrderDto dto)
        {
            var order = new SalesOrder
            {
                SalesOrderNumber = "SO-" + DateTime.Now.Ticks,
                OrderDate = DateTime.Now,
                CustomerId = dto.CustomerId,
                SalesmanId = dto.SalesmanId,
                WarehouseId = dto.WarehouseId,
                DiscountAmount = dto.DiscountAmount,
                Notes = dto.Notes,
                OrderStatus = "Pending"
            };

            if (dto.Details != null && dto.Details.Count > 0)
            {
                order.SalesOrderDetails = dto.Details.Select(d => new SalesOrderDetail
                {
                    ItemId = d.ItemId,
                    OrderedQuantity = d.OrderedQuantity,
                    UnitPrice = d.UnitPrice,
                    LineTotal = d.OrderedQuantity * d.UnitPrice
                }).ToList();

                order.SubTotal = order.SalesOrderDetails.Sum(d => d.LineTotal);
                order.TaxAmount = order.SubTotal * 0.15m;
                order.TotalAmount = order.SubTotal + order.TaxAmount - order.DiscountAmount;
            }

            return await _repository.CreateAsync(order);
        }

        public async Task UpdateSalesOrderAsync(int id, CreateSalesOrderDto dto)
        {
            var order = await _repository.GetByIdAsync(id);
            if (order != null)
            {
                order.DiscountAmount = dto.DiscountAmount;
                order.Notes = dto.Notes;
                await _repository.UpdateAsync(order);
            }
        }

        private SalesOrderDto MapToDto(SalesOrder order)
        {
            return new SalesOrderDto
            {
                SalesOrderId = order.SalesOrderId,
                SalesOrderNumber = order.SalesOrderNumber,
                OrderDate = order.OrderDate,
                OrderStatus = order.OrderStatus,
                SubTotal = order.SubTotal,
                TaxAmount = order.TaxAmount,
                DiscountAmount = order.DiscountAmount,
                TotalAmount = order.TotalAmount,
                CustomerName = order.Customer?.CustomerNameAr,
                SalesmanName = order.Salesman?.SalesmanNameAr,
                Details = order.SalesOrderDetails?.Select(d => new SalesOrderDetailDto
                {
                    ItemId = d.ItemId,
                    OrderedQuantity = d.OrderedQuantity,
                    UnitPrice = d.UnitPrice
                }).ToList()
            };
        }
    }

    public interface ISalesInvoiceService
    {
        Task<List<SalesInvoiceDto>> GetAllInvoicesAsync(int pageNumber = 1, int pageSize = 10);
        Task<SalesInvoiceDto> GetInvoiceByIdAsync(int id);
        Task<int> CreateSalesInvoiceAsync(CreateSalesInvoiceDto dto);
        Task UpdateSalesInvoiceAsync(int id, CreateSalesInvoiceDto dto);
        Task PostInvoiceToAccountingAsync(int invoiceId);
    }

    public class SalesInvoiceService : ISalesInvoiceService
    {
        private readonly ISalesInvoiceRepository _invoiceRepository;
        private readonly IJournalService _journalService;

        public SalesInvoiceService(ISalesInvoiceRepository invoiceRepository, IJournalService journalService)
        {
            _invoiceRepository = invoiceRepository;
            _journalService = journalService;
        }

        public async Task<List<SalesInvoiceDto>> GetAllInvoicesAsync(int pageNumber = 1, int pageSize = 10)
        {
            var invoices = await _invoiceRepository.GetAllAsync(pageNumber, pageSize);
            return invoices.Select(i => MapToDto(i)).ToList();
        }

        public async Task<SalesInvoiceDto> GetInvoiceByIdAsync(int id)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(id);
            return invoice != null ? MapToDto(invoice) : null;
        }

        public async Task<int> CreateSalesInvoiceAsync(CreateSalesInvoiceDto dto)
        {
            var invoice = new SalesInvoice
            {
                InvoiceNumber = "INV-" + DateTime.Now.Ticks,
                InvoiceDate = DateTime.Now,
                CustomerId = dto.CustomerId,
                SalesOrderId = dto.SalesOrderId,
                SalesmanId = dto.SalesmanId,
                DiscountAmount = dto.DiscountAmount,
                Notes = dto.Notes,
                InvoiceStatus = "Pending",
                PaidAmount = 0
            };

            if (dto.Details != null && dto.Details.Count > 0)
            {
                invoice.SalesInvoiceDetails = dto.Details.Select(d => new SalesInvoiceDetail
                {
                    ItemId = d.ItemId,
                    Quantity = d.Quantity,
                    UnitPrice = d.UnitPrice,
                    LineTotal = d.Quantity * d.UnitPrice
                }).ToList();

                invoice.SubTotal = invoice.SalesInvoiceDetails.Sum(d => d.LineTotal);
                invoice.TaxAmount = invoice.SubTotal * 0.15m;
                invoice.TotalAmount = invoice.SubTotal + invoice.TaxAmount - invoice.DiscountAmount;
            }

            var id = await _invoiceRepository.CreateAsync(invoice);

            // Post to Accounting
            await PostInvoiceToAccountingAsync(id);

            return id;
        }

        public async Task UpdateSalesInvoiceAsync(int id, CreateSalesInvoiceDto dto)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(id);
            if (invoice != null)
            {
                invoice.DiscountAmount = dto.DiscountAmount;
                invoice.Notes = dto.Notes;
                await _invoiceRepository.UpdateAsync(invoice);
            }
        }

        public async Task PostInvoiceToAccountingAsync(int invoiceId)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(invoiceId);
            if (invoice != null && invoice.InvoiceStatus != "Posted")
            {
                // Create Journal Entry for Sales
                var entries = new List<JournalEntryDto>
                {
                    new JournalEntryDto
                    {
                        AccountCode = "1000", // Receivables Account
                        Debit = invoice.TotalAmount,
                        Credit = 0,
                        Description = $"Sales Invoice {invoice.InvoiceNumber}"
                    },
                    new JournalEntryDto
                    {
                        AccountCode = "4000", // Sales Account
                        Debit = 0,
                        Credit = invoice.SubTotal,
                        Description = $"Sales Revenue {invoice.InvoiceNumber}"
                    },
                    new JournalEntryDto
                    {
                        AccountCode = "2100", // Tax Payable
                        Debit = 0,
                        Credit = invoice.TaxAmount,
                        Description = $"Sales Tax {invoice.InvoiceNumber}"
                    }
                };

                if (invoice.DiscountAmount > 0)
                {
                    entries.Add(new JournalEntryDto
                    {
                        AccountCode = "4100", // Sales Discount
                        Debit = invoice.DiscountAmount,
                        Credit = 0,
                        Description = $"Sales Discount {invoice.InvoiceNumber}"
                    });
                }

                await _journalService.CreateJournalAsync(new CreateJournalDto
                {
                    JournalTypeId = 2, // Sales Journal
                    JournalDate = invoice.InvoiceDate,
                    Description = $"Sales Invoice {invoice.InvoiceNumber}",
                    ReferenceNumber = invoice.InvoiceNumber,
                    Entries = entries
                });

                invoice.InvoiceStatus = "Posted";
                await _invoiceRepository.UpdateAsync(invoice);
            }
        }

        private SalesInvoiceDto MapToDto(SalesInvoice invoice)
        {
            var dto = new SalesInvoiceDto
            {
                SalesInvoiceId = invoice.SalesInvoiceId,
                InvoiceNumber = invoice.InvoiceNumber,
                InvoiceDate = invoice.InvoiceDate,
                InvoiceStatus = invoice.InvoiceStatus,
                SubTotal = invoice.SubTotal,
                TaxAmount = invoice.TaxAmount,
                TotalAmount = invoice.TotalAmount,
                PaidAmount = invoice.PaidAmount,
                RemainingAmount = invoice.TotalAmount - invoice.PaidAmount,
                CustomerName = invoice.Customer?.CustomerNameAr,
                Details = invoice.SalesInvoiceDetails?.Select(d => new SalesInvoiceDetailDto
                {
                    ItemId = d.ItemId,
                    Quantity = d.Quantity,
                    UnitPrice = d.UnitPrice
                }).ToList()
            };
            return dto;
        }
    }

    public interface ISalesReturnService
    {
        Task<List<SalesReturnDto>> GetAllReturnsAsync(int pageNumber = 1, int pageSize = 10);
        Task<SalesReturnDto> GetReturnByIdAsync(int id);
        Task<int> CreateSalesReturnAsync(CreateSalesReturnDto dto);
    }

    public class SalesReturnService : ISalesReturnService
    {
        private readonly ISalesReturnRepository _repository;

        public SalesReturnService(ISalesReturnRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<SalesReturnDto>> GetAllReturnsAsync(int pageNumber = 1, int pageSize = 10)
        {
            var returns = await _repository.GetAllAsync(pageNumber, pageSize);
            return returns.Select(r => MapToDto(r)).ToList();
        }

        public async Task<SalesReturnDto> GetReturnByIdAsync(int id)
        {
            var salesReturn = await _repository.GetByIdAsync(id);
            return salesReturn != null ? MapToDto(salesReturn) : null;
        }

        public async Task<int> CreateSalesReturnAsync(CreateSalesReturnDto dto)
        {
            var salesReturn = new SalesReturn
            {
                ReturnNumber = "RET-" + DateTime.Now.Ticks,
                ReturnDate = DateTime.Now,
                CustomerId = dto.CustomerId,
                SalesInvoiceId = dto.SalesInvoiceId,
                Reason = dto.Reason,
                Status = "Pending"
            };

            if (dto.Details != null && dto.Details.Count > 0)
            {
                salesReturn.SalesReturnDetails = dto.Details.Select(d => new SalesReturnDetail
                {
                    ItemId = d.ItemId,
                    ReturnedQuantity = d.ReturnedQuantity,
                    UnitPrice = d.UnitPrice,
                    LineTotal = d.ReturnedQuantity * d.UnitPrice
                }).ToList();

                salesReturn.SubTotal = salesReturn.SalesReturnDetails.Sum(d => d.LineTotal);
                salesReturn.TaxAmount = salesReturn.SubTotal * 0.15m;
                salesReturn.TotalAmount = salesReturn.SubTotal + salesReturn.TaxAmount;
                salesReturn.CreditNoteAmount = salesReturn.TotalAmount;
            }

            return await _repository.CreateAsync(salesReturn);
        }

        private SalesReturnDto MapToDto(SalesReturn salesReturn)
        {
            return new SalesReturnDto
            {
                SalesReturnId = salesReturn.SalesReturnId,
                ReturnNumber = salesReturn.ReturnNumber,
                ReturnDate = salesReturn.ReturnDate,
                TotalAmount = salesReturn.TotalAmount,
                CreditNoteAmount = salesReturn.CreditNoteAmount,
                Reason = salesReturn.Reason,
                Status = salesReturn.Status,
                CustomerName = salesReturn.Customer?.CustomerNameAr,
                Details = salesReturn.SalesReturnDetails?.Select(d => new SalesReturnDetailDto
                {
                    ItemId = d.ItemId,
                    ReturnedQuantity = d.ReturnedQuantity,
                    UnitPrice = d.UnitPrice
                }).ToList()
            };
        }
    }

    public interface ICustomerBalanceService
    {
        Task<CustomerBalanceDto> GetCustomerBalanceAsync(int customerId);
        Task UpdateCustomerBalanceAsync(int customerId, decimal amount);
        Task<List<CustomerBalanceDto>> GetOverdueBalancesAsync();
    }

    public class CustomerBalanceService : ICustomerBalanceService
    {
        private readonly ICustomerBalanceRepository _repository;

        public CustomerBalanceService(ICustomerBalanceRepository repository)
        {
            _repository = repository;
        }

        public async Task<CustomerBalanceDto> GetCustomerBalanceAsync(int customerId)
        {
            var balance = await _repository.GetBalanceAsync(customerId, 1);
            return balance != null ? MapToDto(balance) : null;
        }

        public async Task UpdateCustomerBalanceAsync(int customerId, decimal amount)
        {
            var balance = await _repository.GetBalanceAsync(customerId, 1);
            if (balance != null)
            {
                balance.TotalAmount += amount;
                balance.BalanceAmount = balance.TotalAmount - balance.PaidAmount;
                await _repository.UpdateBalanceAsync(balance);
            }
        }

        public async Task<List<CustomerBalanceDto>> GetOverdueBalancesAsync()
        {
            var balances = await _repository.GetOverdueBalancesAsync();
            return balances.Select(b => MapToDto(b)).ToList();
        }

        private CustomerBalanceDto MapToDto(CustomerBalance balance)
        {
            return new CustomerBalanceDto
            {
                CustomerId = balance.CustomerId,
                CustomerName = balance.Customer?.CustomerNameAr,
                TotalAmount = balance.TotalAmount,
                PaidAmount = balance.PaidAmount,
                BalanceAmount = balance.BalanceAmount,
                CurrencyCode = balance.Currency?.CurrencyCode
            };
        }
    }
}
