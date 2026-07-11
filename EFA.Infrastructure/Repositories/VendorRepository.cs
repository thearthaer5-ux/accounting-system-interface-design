using EFA.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EFA.Infrastructure.Repositories
{
    public class VendorRepository : GenericRepository<Vendor>, IVendorRepository
    {
        public VendorRepository(EFADbContext context) : base(context) { }

        public async Task<Vendor> GetVendorWithContactsAsync(int vendorId)
        {
            return await _context.Vendors
                .Include(v => v.VendorContacts)
                .Include(v => v.VendorBalances)
                .FirstOrDefaultAsync(v => v.VendorId == vendorId);
        }

        public async Task<List<Vendor>> GetVendorsByTypeAsync(int vendorTypeId)
        {
            return await _context.Vendors
                .Where(v => v.VendorTypeId == vendorTypeId && v.IsActive)
                .ToListAsync();
        }

        public async Task<Vendor> GetVendorByCodeAsync(string vendorCode)
        {
            return await _context.Vendors
                .FirstOrDefaultAsync(v => v.VendorCode == vendorCode);
        }

        public async Task<bool> IsVendorCodeUniqueAsync(string vendorCode, int excludeVendorId = 0)
        {
            if (excludeVendorId == 0)
                return !await _context.Vendors.AnyAsync(v => v.VendorCode == vendorCode);
            
            return !await _context.Vendors.AnyAsync(v => v.VendorCode == vendorCode && v.VendorId != excludeVendorId);
        }

        public async Task<List<Vendor>> SearchVendorsAsync(string searchTerm)
        {
            return await _context.Vendors
                .Where(v => v.VendorCode.Contains(searchTerm) || 
                           v.VendorNameAr.Contains(searchTerm) || 
                           v.VendorNameEn.Contains(searchTerm))
                .OrderBy(v => v.VendorNameAr)
                .ToListAsync();
        }

        public async Task<List<Vendor>> GetVendorsByBranchAsync(int branchId)
        {
            return await _context.Vendors
                .Where(v => v.BranchId == branchId && v.IsActive)
                .ToListAsync();
        }

        public async Task<List<Vendor>> GetActiveVendorsAsync()
        {
            return await _context.Vendors
                .Where(v => v.IsActive)
                .OrderBy(v => v.VendorNameAr)
                .ToListAsync();
        }

        public async Task<decimal> GetVendorTotalBalanceAsync(int vendorId)
        {
            return await _context.VendorBalances
                .Where(vb => vb.VendorId == vendorId)
                .SumAsync(vb => vb.BalanceAmount);
        }
    }

    public class QuotationRepository : GenericRepository<Quotation>, IQuotationRepository
    {
        public QuotationRepository(EFADbContext context) : base(context) { }

        public async Task<Quotation> GetQuotationWithDetailsAsync(int quotationId)
        {
            return await _context.Quotations
                .Include(q => q.QuotationDetails)
                .FirstOrDefaultAsync(q => q.QuotationId == quotationId);
        }

        public async Task<List<Quotation>> GetQuotationsByVendorAsync(int vendorId)
        {
            return await _context.Quotations
                .Where(q => q.VendorId == vendorId)
                .OrderByDescending(q => q.QuotationDate)
                .ToListAsync();
        }

        public async Task<Quotation> GetQuotationByNumberAsync(string quotationNumber)
        {
            return await _context.Quotations
                .FirstOrDefaultAsync(q => q.QuotationNumber == quotationNumber);
        }

        public async Task<List<Quotation>> GetQuotationsByStatusAsync(string status)
        {
            return await _context.Quotations
                .Where(q => q.Status == status)
                .OrderByDescending(q => q.QuotationDate)
                .ToListAsync();
        }

        public async Task<List<Quotation>> GetQuotationsByDateRangeAsync(DateTime fromDate, DateTime toDate)
        {
            return await _context.Quotations
                .Where(q => q.QuotationDate >= fromDate && q.QuotationDate <= toDate)
                .OrderByDescending(q => q.QuotationDate)
                .ToListAsync();
        }
    }

    public class PurchaseOrderRepository : GenericRepository<PurchaseOrder>, IPurchaseOrderRepository
    {
        public PurchaseOrderRepository(EFADbContext context) : base(context) { }

        public async Task<PurchaseOrder> GetPurchaseOrderWithDetailsAsync(int poId)
        {
            return await _context.PurchaseOrders
                .Include(po => po.PurchaseOrderDetails)
                .Include(po => po.Vendor)
                .FirstOrDefaultAsync(po => po.PurchaseOrderId == poId);
        }

        public async Task<List<PurchaseOrder>> GetPurchaseOrdersByVendorAsync(int vendorId)
        {
            return await _context.PurchaseOrders
                .Where(po => po.VendorId == vendorId)
                .OrderByDescending(po => po.OrderDate)
                .ToListAsync();
        }

        public async Task<PurchaseOrder> GetPurchaseOrderByNumberAsync(string poNumber)
        {
            return await _context.PurchaseOrders
                .FirstOrDefaultAsync(po => po.PurchaseOrderNumber == poNumber);
        }

        public async Task<List<PurchaseOrder>> GetPurchaseOrdersByStatusAsync(string status)
        {
            return await _context.PurchaseOrders
                .Where(po => po.Status == status)
                .OrderByDescending(po => po.OrderDate)
                .ToListAsync();
        }

        public async Task<List<PurchaseOrder>> GetPendingPurchaseOrdersAsync()
        {
            return await _context.PurchaseOrders
                .Where(po => po.Status != "Completed" && po.Status != "Cancelled")
                .OrderByDescending(po => po.OrderDate)
                .ToListAsync();
        }

        public async Task<List<PurchaseOrder>> GetPartiallyReceivedPurchaseOrdersAsync()
        {
            return await _context.PurchaseOrders
                .Where(po => po.ReceivedQuantityPercent > 0 && po.ReceivedQuantityPercent < 100)
                .OrderByDescending(po => po.OrderDate)
                .ToListAsync();
        }
    }

    public class PurchaseInvoiceRepository : GenericRepository<PurchaseInvoice>, IPurchaseInvoiceRepository
    {
        public PurchaseInvoiceRepository(EFADbContext context) : base(context) { }

        public async Task<PurchaseInvoice> GetInvoiceWithDetailsAsync(int invoiceId)
        {
            return await _context.PurchaseInvoices
                .Include(pi => pi.PurchaseInvoiceDetails)
                .Include(pi => pi.PurchasePayments)
                .FirstOrDefaultAsync(pi => pi.PurchaseInvoiceId == invoiceId);
        }

        public async Task<List<PurchaseInvoice>> GetInvoicesByVendorAsync(int vendorId)
        {
            return await _context.PurchaseInvoices
                .Where(pi => pi.VendorId == vendorId)
                .OrderByDescending(pi => pi.InvoiceDate)
                .ToListAsync();
        }

        public async Task<PurchaseInvoice> GetInvoiceByNumberAsync(string invoiceNumber)
        {
            return await _context.PurchaseInvoices
                .FirstOrDefaultAsync(pi => pi.InvoiceNumber == invoiceNumber);
        }

        public async Task<List<PurchaseInvoice>> GetInvoicesByStatusAsync(string status)
        {
            return await _context.PurchaseInvoices
                .Where(pi => pi.Status == status)
                .OrderByDescending(pi => pi.InvoiceDate)
                .ToListAsync();
        }

        public async Task<List<PurchaseInvoice>> GetUnpaidInvoicesAsync(int vendorId)
        {
            return await _context.PurchaseInvoices
                .Where(pi => pi.VendorId == vendorId && pi.PaidAmount < pi.TotalAmount)
                .OrderByDescending(pi => pi.InvoiceDate)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalUnpaidAmountAsync(int vendorId)
        {
            return await _context.PurchaseInvoices
                .Where(pi => pi.VendorId == vendorId && pi.Status == "Posted")
                .SumAsync(pi => pi.TotalAmount - pi.PaidAmount);
        }
    }

    public class PurchaseReturnRepository : GenericRepository<PurchaseReturn>, IPurchaseReturnRepository
    {
        public PurchaseReturnRepository(EFADbContext context) : base(context) { }

        public async Task<PurchaseReturn> GetReturnWithDetailsAsync(int returnId)
        {
            return await _context.PurchaseReturns
                .Include(pr => pr.PurchaseReturnDetails)
                .FirstOrDefaultAsync(pr => pr.PurchaseReturnId == returnId);
        }

        public async Task<List<PurchaseReturn>> GetReturnsByVendorAsync(int vendorId)
        {
            return await _context.PurchaseReturns
                .Where(pr => pr.VendorId == vendorId)
                .OrderByDescending(pr => pr.ReturnDate)
                .ToListAsync();
        }

        public async Task<PurchaseReturn> GetReturnByNumberAsync(string returnNumber)
        {
            return await _context.PurchaseReturns
                .FirstOrDefaultAsync(pr => pr.ReturnNumber == returnNumber);
        }

        public async Task<List<PurchaseReturn>> GetReturnsByInvoiceAsync(int invoiceId)
        {
            return await _context.PurchaseReturns
                .Where(pr => pr.PurchaseInvoiceId == invoiceId)
                .OrderByDescending(pr => pr.ReturnDate)
                .ToListAsync();
        }

        public async Task<List<PurchaseReturn>> GetReturnsByDateRangeAsync(DateTime fromDate, DateTime toDate)
        {
            return await _context.PurchaseReturns
                .Where(pr => pr.ReturnDate >= fromDate && pr.ReturnDate <= toDate)
                .OrderByDescending(pr => pr.ReturnDate)
                .ToListAsync();
        }
    }

    public class VendorBalanceRepository : GenericRepository<VendorBalance>, IVendorBalanceRepository
    {
        public VendorBalanceRepository(EFADbContext context) : base(context) { }

        public async Task<VendorBalance> GetBalanceByVendorAndCurrencyAsync(int vendorId, int currencyId)
        {
            return await _context.VendorBalances
                .FirstOrDefaultAsync(vb => vb.VendorId == vendorId && vb.CurrencyId == currencyId);
        }

        public async Task<List<VendorBalance>> GetBalancesByVendorAsync(int vendorId)
        {
            return await _context.VendorBalances
                .Where(vb => vb.VendorId == vendorId)
                .ToListAsync();
        }

        public async Task<List<VendorBalance>> GetBalancesByBranchAsync(int branchId)
        {
            return await _context.VendorBalances
                .Where(vb => vb.Vendor.BranchId == branchId)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalVendorBalanceAsync(int vendorId)
        {
            return await _context.VendorBalances
                .Where(vb => vb.VendorId == vendorId)
                .SumAsync(vb => vb.BalanceAmount);
        }

        public async Task UpdateBalanceAsync(int vendorId, int currencyId, decimal amount)
        {
            var balance = await GetBalanceByVendorAndCurrencyAsync(vendorId, currencyId);
            if (balance != null)
            {
                balance.BalanceAmount += amount;
                _context.VendorBalances.Update(balance);
                await _context.SaveChangesAsync();
            }
        }
    }

    public class PurchasePaymentRepository : GenericRepository<PurchasePayment>, IPurchasePaymentRepository
    {
        public PurchasePaymentRepository(EFADbContext context) : base(context) { }

        public async Task<List<PurchasePayment>> GetPaymentsByVendorAsync(int vendorId)
        {
            return await _context.PurchasePayments
                .Where(pp => pp.VendorId == vendorId)
                .OrderByDescending(pp => pp.PaymentDate)
                .ToListAsync();
        }

        public async Task<List<PurchasePayment>> GetPaymentsByInvoiceAsync(int invoiceId)
        {
            return await _context.PurchasePayments
                .Where(pp => pp.PurchaseInvoiceId == invoiceId)
                .OrderByDescending(pp => pp.PaymentDate)
                .ToListAsync();
        }

        public async Task<List<PurchasePayment>> GetPaymentsByDateRangeAsync(DateTime fromDate, DateTime toDate)
        {
            return await _context.PurchasePayments
                .Where(pp => pp.PaymentDate >= fromDate && pp.PaymentDate <= toDate)
                .OrderByDescending(pp => pp.PaymentDate)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalPaidAmountAsync(int invoiceId)
        {
            return await _context.PurchasePayments
                .Where(pp => pp.PurchaseInvoiceId == invoiceId)
                .SumAsync(pp => pp.PaymentAmount);
        }
    }
}
