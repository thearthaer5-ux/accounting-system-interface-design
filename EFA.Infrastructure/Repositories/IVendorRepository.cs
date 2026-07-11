using EFA.Domain.Entities;
using System.Linq.Expressions;

namespace EFA.Infrastructure.Repositories
{
    public interface IVendorRepository : IGenericRepository<Vendor>
    {
        Task<Vendor> GetVendorWithContactsAsync(int vendorId);
        Task<List<Vendor>> GetVendorsByTypeAsync(int vendorTypeId);
        Task<Vendor> GetVendorByCodeAsync(string vendorCode);
        Task<bool> IsVendorCodeUniqueAsync(string vendorCode, int excludeVendorId = 0);
        Task<List<Vendor>> SearchVendorsAsync(string searchTerm);
        Task<List<Vendor>> GetVendorsByBranchAsync(int branchId);
        Task<List<Vendor>> GetActiveVendorsAsync();
        Task<decimal> GetVendorTotalBalanceAsync(int vendorId);
    }

    public interface IQuotationRepository : IGenericRepository<Quotation>
    {
        Task<Quotation> GetQuotationWithDetailsAsync(int quotationId);
        Task<List<Quotation>> GetQuotationsByVendorAsync(int vendorId);
        Task<Quotation> GetQuotationByNumberAsync(string quotationNumber);
        Task<List<Quotation>> GetQuotationsByStatusAsync(string status);
        Task<List<Quotation>> GetQuotationsByDateRangeAsync(DateTime fromDate, DateTime toDate);
    }

    public interface IPurchaseOrderRepository : IGenericRepository<PurchaseOrder>
    {
        Task<PurchaseOrder> GetPurchaseOrderWithDetailsAsync(int poId);
        Task<List<PurchaseOrder>> GetPurchaseOrdersByVendorAsync(int vendorId);
        Task<PurchaseOrder> GetPurchaseOrderByNumberAsync(string poNumber);
        Task<List<PurchaseOrder>> GetPurchaseOrdersByStatusAsync(string status);
        Task<List<PurchaseOrder>> GetPendingPurchaseOrdersAsync();
        Task<List<PurchaseOrder>> GetPartiallyReceivedPurchaseOrdersAsync();
    }

    public interface IPurchaseInvoiceRepository : IGenericRepository<PurchaseInvoice>
    {
        Task<PurchaseInvoice> GetInvoiceWithDetailsAsync(int invoiceId);
        Task<List<PurchaseInvoice>> GetInvoicesByVendorAsync(int vendorId);
        Task<PurchaseInvoice> GetInvoiceByNumberAsync(string invoiceNumber);
        Task<List<PurchaseInvoice>> GetInvoicesByStatusAsync(string status);
        Task<List<PurchaseInvoice>> GetUnpaidInvoicesAsync(int vendorId);
        Task<decimal> GetTotalUnpaidAmountAsync(int vendorId);
    }

    public interface IPurchaseReturnRepository : IGenericRepository<PurchaseReturn>
    {
        Task<PurchaseReturn> GetReturnWithDetailsAsync(int returnId);
        Task<List<PurchaseReturn>> GetReturnsByVendorAsync(int vendorId);
        Task<PurchaseReturn> GetReturnByNumberAsync(string returnNumber);
        Task<List<PurchaseReturn>> GetReturnsByInvoiceAsync(int invoiceId);
        Task<List<PurchaseReturn>> GetReturnsByDateRangeAsync(DateTime fromDate, DateTime toDate);
    }

    public interface IVendorBalanceRepository : IGenericRepository<VendorBalance>
    {
        Task<VendorBalance> GetBalanceByVendorAndCurrencyAsync(int vendorId, int currencyId);
        Task<List<VendorBalance>> GetBalancesByVendorAsync(int vendorId);
        Task<List<VendorBalance>> GetBalancesByBranchAsync(int branchId);
        Task<decimal> GetTotalVendorBalanceAsync(int vendorId);
        Task UpdateBalanceAsync(int vendorId, int currencyId, decimal amount);
    }

    public interface IPurchasePaymentRepository : IGenericRepository<PurchasePayment>
    {
        Task<List<PurchasePayment>> GetPaymentsByVendorAsync(int vendorId);
        Task<List<PurchasePayment>> GetPaymentsByInvoiceAsync(int invoiceId);
        Task<List<PurchasePayment>> GetPaymentsByDateRangeAsync(DateTime fromDate, DateTime toDate);
        Task<decimal> GetTotalPaidAmountAsync(int invoiceId);
    }
}
