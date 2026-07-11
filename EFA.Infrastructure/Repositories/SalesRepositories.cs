using EFA.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFA.Infrastructure.Repositories
{
    // Customer Repository
    public interface ICustomerRepository
    {
        Task<List<Customer>> GetAllAsync(int pageNumber = 1, int pageSize = 10, string search = "");
        Task<Customer> GetByIdAsync(int id);
        Task<Customer> GetByCodeAsync(string code);
        Task<int> CreateAsync(Customer entity);
        Task UpdateAsync(Customer entity);
        Task DeleteAsync(int id);
        Task<List<Customer>> GetActiveCustomersAsync();
    }

    public class CustomerRepository : ICustomerRepository
    {
        private readonly EFADbContext _context;

        public CustomerRepository(EFADbContext context)
        {
            _context = context;
        }

        public async Task<List<Customer>> GetAllAsync(int pageNumber = 1, int pageSize = 10, string search = "")
        {
            var query = _context.Customers.AsQueryable();

            if (!string.IsNullOrEmpty(search))
                query = query.Where(c => c.CustomerCode.Contains(search) || c.CustomerNameAr.Contains(search) || c.CustomerNameEn.Contains(search));

            return await query.OrderByDescending(c => c.CreatedDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Customer> GetByIdAsync(int id)
        {
            return await _context.Customers
                .Include(c => c.CustomerType)
                .Include(c => c.Branch)
                .FirstOrDefaultAsync(c => c.CustomerId == id && !c.IsDeleted);
        }

        public async Task<Customer> GetByCodeAsync(string code)
        {
            return await _context.Customers
                .FirstOrDefaultAsync(c => c.CustomerCode == code && !c.IsDeleted);
        }

        public async Task<int> CreateAsync(Customer entity)
        {
            entity.CreatedDate = DateTime.Now;
            _context.Customers.Add(entity);
            await _context.SaveChangesAsync();
            return entity.CustomerId;
        }

        public async Task UpdateAsync(Customer entity)
        {
            entity.ModifiedDate = DateTime.Now;
            _context.Customers.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var customer = await GetByIdAsync(id);
            if (customer != null)
            {
                customer.IsDeleted = true;
                await UpdateAsync(customer);
            }
        }

        public async Task<List<Customer>> GetActiveCustomersAsync()
        {
            return await _context.Customers
                .Where(c => c.IsActive && !c.IsDeleted)
                .OrderBy(c => c.CustomerCode)
                .ToListAsync();
        }
    }

    // Sales Order Repository
    public interface ISalesOrderRepository
    {
        Task<List<SalesOrder>> GetAllAsync(int pageNumber = 1, int pageSize = 10);
        Task<SalesOrder> GetByIdAsync(int id);
        Task<SalesOrder> GetByNumberAsync(string orderNumber);
        Task<int> CreateAsync(SalesOrder entity);
        Task UpdateAsync(SalesOrder entity);
        Task DeleteAsync(int id);
        Task<List<SalesOrder>> GetPendingOrdersAsync(int customerId);
    }

    public class SalesOrderRepository : ISalesOrderRepository
    {
        private readonly EFADbContext _context;

        public SalesOrderRepository(EFADbContext context)
        {
            _context = context;
        }

        public async Task<List<SalesOrder>> GetAllAsync(int pageNumber = 1, int pageSize = 10)
        {
            return await _context.SalesOrders
                .Include(so => so.Customer)
                .Include(so => so.SalesOrderDetails)
                .OrderByDescending(so => so.CreatedDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<SalesOrder> GetByIdAsync(int id)
        {
            return await _context.SalesOrders
                .Include(so => so.Customer)
                .Include(so => so.Salesman)
                .Include(so => so.SalesOrderDetails)
                .FirstOrDefaultAsync(so => so.SalesOrderId == id);
        }

        public async Task<SalesOrder> GetByNumberAsync(string orderNumber)
        {
            return await _context.SalesOrders
                .FirstOrDefaultAsync(so => so.SalesOrderNumber == orderNumber);
        }

        public async Task<int> CreateAsync(SalesOrder entity)
        {
            entity.CreatedDate = DateTime.Now;
            _context.SalesOrders.Add(entity);
            await _context.SaveChangesAsync();
            return entity.SalesOrderId;
        }

        public async Task UpdateAsync(SalesOrder entity)
        {
            entity.ModifiedDate = DateTime.Now;
            _context.SalesOrders.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var order = await GetByIdAsync(id);
            if (order != null)
            {
                _context.SalesOrders.Remove(order);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<SalesOrder>> GetPendingOrdersAsync(int customerId)
        {
            return await _context.SalesOrders
                .Where(so => so.CustomerId == customerId && so.OrderStatus != "Completed")
                .ToListAsync();
        }
    }

    // Sales Invoice Repository
    public interface ISalesInvoiceRepository
    {
        Task<List<SalesInvoice>> GetAllAsync(int pageNumber = 1, int pageSize = 10);
        Task<SalesInvoice> GetByIdAsync(int id);
        Task<SalesInvoice> GetByNumberAsync(string invoiceNumber);
        Task<int> CreateAsync(SalesInvoice entity);
        Task UpdateAsync(SalesInvoice entity);
        Task DeleteAsync(int id);
        Task<List<SalesInvoice>> GetUnpaidInvoicesAsync(int customerId);
    }

    public class SalesInvoiceRepository : ISalesInvoiceRepository
    {
        private readonly EFADbContext _context;

        public SalesInvoiceRepository(EFADbContext context)
        {
            _context = context;
        }

        public async Task<List<SalesInvoice>> GetAllAsync(int pageNumber = 1, int pageSize = 10)
        {
            return await _context.SalesInvoices
                .Include(si => si.Customer)
                .Include(si => si.SalesInvoiceDetails)
                .OrderByDescending(si => si.CreatedDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<SalesInvoice> GetByIdAsync(int id)
        {
            return await _context.SalesInvoices
                .Include(si => si.Customer)
                .Include(si => si.SalesInvoiceDetails)
                .FirstOrDefaultAsync(si => si.SalesInvoiceId == id);
        }

        public async Task<SalesInvoice> GetByNumberAsync(string invoiceNumber)
        {
            return await _context.SalesInvoices
                .FirstOrDefaultAsync(si => si.InvoiceNumber == invoiceNumber);
        }

        public async Task<int> CreateAsync(SalesInvoice entity)
        {
            entity.CreatedDate = DateTime.Now;
            _context.SalesInvoices.Add(entity);
            await _context.SaveChangesAsync();
            return entity.SalesInvoiceId;
        }

        public async Task UpdateAsync(SalesInvoice entity)
        {
            entity.ModifiedDate = DateTime.Now;
            _context.SalesInvoices.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var invoice = await GetByIdAsync(id);
            if (invoice != null)
            {
                _context.SalesInvoices.Remove(invoice);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<SalesInvoice>> GetUnpaidInvoicesAsync(int customerId)
        {
            return await _context.SalesInvoices
                .Where(si => si.CustomerId == customerId && si.PaidAmount < si.TotalAmount)
                .ToListAsync();
        }
    }

    // Sales Return Repository
    public interface ISalesReturnRepository
    {
        Task<List<SalesReturn>> GetAllAsync(int pageNumber = 1, int pageSize = 10);
        Task<SalesReturn> GetByIdAsync(int id);
        Task<int> CreateAsync(SalesReturn entity);
        Task UpdateAsync(SalesReturn entity);
    }

    public class SalesReturnRepository : ISalesReturnRepository
    {
        private readonly EFADbContext _context;

        public SalesReturnRepository(EFADbContext context)
        {
            _context = context;
        }

        public async Task<List<SalesReturn>> GetAllAsync(int pageNumber = 1, int pageSize = 10)
        {
            return await _context.SalesReturns
                .Include(sr => sr.Customer)
                .OrderByDescending(sr => sr.CreatedDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<SalesReturn> GetByIdAsync(int id)
        {
            return await _context.SalesReturns
                .Include(sr => sr.SalesReturnDetails)
                .FirstOrDefaultAsync(sr => sr.SalesReturnId == id);
        }

        public async Task<int> CreateAsync(SalesReturn entity)
        {
            entity.CreatedDate = DateTime.Now;
            _context.SalesReturns.Add(entity);
            await _context.SaveChangesAsync();
            return entity.SalesReturnId;
        }

        public async Task UpdateAsync(SalesReturn entity)
        {
            entity.ModifiedDate = DateTime.Now;
            _context.SalesReturns.Update(entity);
            await _context.SaveChangesAsync();
        }
    }

    // Sales Payment Repository
    public interface ISalesPaymentRepository
    {
        Task<List<SalesPayment>> GetAllAsync(int pageNumber = 1, int pageSize = 10);
        Task<SalesPayment> GetByIdAsync(int id);
        Task<int> CreateAsync(SalesPayment entity);
        Task<List<SalesPayment>> GetByCustomerAsync(int customerId);
    }

    public class SalesPaymentRepository : ISalesPaymentRepository
    {
        private readonly EFADbContext _context;

        public SalesPaymentRepository(EFADbContext context)
        {
            _context = context;
        }

        public async Task<List<SalesPayment>> GetAllAsync(int pageNumber = 1, int pageSize = 10)
        {
            return await _context.SalesPayments
                .Include(sp => sp.Customer)
                .OrderByDescending(sp => sp.CreatedDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<SalesPayment> GetByIdAsync(int id)
        {
            return await _context.SalesPayments
                .FirstOrDefaultAsync(sp => sp.SalesPaymentId == id);
        }

        public async Task<int> CreateAsync(SalesPayment entity)
        {
            entity.CreatedDate = DateTime.Now;
            _context.SalesPayments.Add(entity);
            await _context.SaveChangesAsync();
            return entity.SalesPaymentId;
        }

        public async Task<List<SalesPayment>> GetByCustomerAsync(int customerId)
        {
            return await _context.SalesPayments
                .Where(sp => sp.CustomerId == customerId)
                .OrderByDescending(sp => sp.CreatedDate)
                .ToListAsync();
        }
    }

    // Customer Balance Repository
    public interface ICustomerBalanceRepository
    {
        Task<CustomerBalance> GetBalanceAsync(int customerId, int currencyId);
        Task UpdateBalanceAsync(CustomerBalance balance);
        Task<List<CustomerBalance>> GetOverdueBalancesAsync();
    }

    public class CustomerBalanceRepository : ICustomerBalanceRepository
    {
        private readonly EFADbContext _context;

        public CustomerBalanceRepository(EFADbContext context)
        {
            _context = context;
        }

        public async Task<CustomerBalance> GetBalanceAsync(int customerId, int currencyId)
        {
            return await _context.CustomerBalances
                .FirstOrDefaultAsync(cb => cb.CustomerId == customerId && cb.CurrencyId == currencyId);
        }

        public async Task UpdateBalanceAsync(CustomerBalance balance)
        {
            _context.CustomerBalances.Update(balance);
            await _context.SaveChangesAsync();
        }

        public async Task<List<CustomerBalance>> GetOverdueBalancesAsync()
        {
            return await _context.CustomerBalances
                .Where(cb => cb.BalanceAmount > 0)
                .OrderByDescending(cb => cb.BalanceAmount)
                .ToListAsync();
        }
    }

    // Salesman Repository
    public interface ISalesmanRepository
    {
        Task<List<Salesman>> GetAllAsync();
        Task<Salesman> GetByIdAsync(int id);
        Task<int> CreateAsync(Salesman entity);
        Task UpdateAsync(Salesman entity);
    }

    public class SalesmanRepository : ISalesmanRepository
    {
        private readonly EFADbContext _context;

        public SalesmanRepository(EFADbContext context)
        {
            _context = context;
        }

        public async Task<List<Salesman>> GetAllAsync()
        {
            return await _context.Salesmen
                .Where(s => s.IsActive)
                .OrderBy(s => s.SalesmanCode)
                .ToListAsync();
        }

        public async Task<Salesman> GetByIdAsync(int id)
        {
            return await _context.Salesmen
                .FirstOrDefaultAsync(s => s.SalesmanId == id);
        }

        public async Task<int> CreateAsync(Salesman entity)
        {
            _context.Salesmen.Add(entity);
            await _context.SaveChangesAsync();
            return entity.SalesmanId;
        }

        public async Task UpdateAsync(Salesman entity)
        {
            entity.ModifiedDate = DateTime.Now;
            _context.Salesmen.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
