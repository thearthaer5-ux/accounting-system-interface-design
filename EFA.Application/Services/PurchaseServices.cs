using EFA.Domain.Entities;
using EFA.Application.DTOs;
using EFA.Infrastructure.Repositories;
using AutoMapper;
using System.Linq;

namespace EFA.Application.Services
{
    public interface IVendorService
    {
        Task<List<VendorDto>> GetAllVendorsAsync();
        Task<VendorDto> GetVendorByIdAsync(int vendorId);
        Task<VendorDto> GetVendorByCodeAsync(string vendorCode);
        Task<int> CreateVendorAsync(VendorCreateUpdateDto dto, int userId);
        Task<bool> UpdateVendorAsync(int vendorId, VendorCreateUpdateDto dto, int userId);
        Task<bool> DeleteVendorAsync(int vendorId, int userId);
        Task<bool> IsVendorCodeUniqueAsync(string vendorCode, int excludeVendorId = 0);
        Task<decimal> GetVendorTotalBalanceAsync(int vendorId);
        Task<List<VendorDto>> SearchVendorsAsync(string searchTerm);
    }

    public interface IPurchaseOrderService
    {
        Task<PurchaseOrderDto> GetPurchaseOrderAsync(int poId);
        Task<List<PurchaseOrderDto>> GetPendingPurchaseOrdersAsync();
        Task<int> CreatePurchaseOrderAsync(PurchaseOrderDto dto, int userId);
        Task<bool> ReceivePurchaseOrderAsync(int poId, List<int> quantities, int userId);
        Task<bool> CompletePurchaseOrderAsync(int poId, int userId);
        Task<bool> CancelPurchaseOrderAsync(int poId, int userId);
        Task UpdateReceivedQuantityPercentAsync(int poId);
    }

    public interface IPurchaseInvoiceService
    {
        Task<PurchaseInvoiceDto> GetInvoiceAsync(int invoiceId);
        Task<List<PurchaseInvoiceDto>> GetUnpaidInvoicesAsync(int vendorId);
        Task<int> CreateInvoiceAsync(PurchaseInvoiceDto dto, int userId);
        Task<bool> PostInvoiceAsync(int invoiceId, int userId);
        Task<bool> RecordPaymentAsync(int invoiceId, decimal amount, string method, int userId);
        Task<bool> CancelInvoiceAsync(int invoiceId, int userId);
        Task UpdateInvoiceStatusAsync(int invoiceId);
    }

    public interface IPurchaseReturnService
    {
        Task<PurchaseReturnDto> GetReturnAsync(int returnId);
        Task<List<PurchaseReturnDto>> GetReturnsByVendorAsync(int vendorId);
        Task<int> CreateReturnAsync(PurchaseReturnDto dto, int userId);
        Task<bool> PostReturnAsync(int returnId, int userId);
        Task<bool> ApplyCreditNoteAsync(int returnId, int userId);
    }

    public interface IVendorBalanceService
    {
        Task<VendorBalanceDto> GetBalanceAsync(int vendorId, int currencyId);
        Task<List<VendorBalanceDto>> GetAllVendorBalancesAsync();
        Task UpdateBalanceAsync(int vendorId, int currencyId, decimal amount);
        Task RecalculateAllBalancesAsync();
        Task<decimal> GetTotalOutstandingAsync();
    }

    // Implementation
    public class VendorService : IVendorService
    {
        private readonly IVendorRepository _repository;
        private readonly IMapper _mapper;

        public VendorService(IVendorRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<VendorDto>> GetAllVendorsAsync()
        {
            var vendors = await _repository.GetActiveVendorsAsync();
            return _mapper.Map<List<VendorDto>>(vendors);
        }

        public async Task<VendorDto> GetVendorByIdAsync(int vendorId)
        {
            var vendor = await _repository.GetVendorWithContactsAsync(vendorId);
            if (vendor == null) throw new Exception($"الموردين مع المعرّف {vendorId} غير موجود");
            
            var dto = _mapper.Map<VendorDto>(vendor);
            dto.TotalBalance = await _repository.GetVendorTotalBalanceAsync(vendorId);
            return dto;
        }

        public async Task<VendorDto> GetVendorByCodeAsync(string vendorCode)
        {
            var vendor = await _repository.GetVendorByCodeAsync(vendorCode);
            if (vendor == null) throw new Exception($"الموردين بالكود {vendorCode} غير موجود");
            return _mapper.Map<VendorDto>(vendor);
        }

        public async Task<int> CreateVendorAsync(VendorCreateUpdateDto dto, int userId)
        {
            if (!await IsVendorCodeUniqueAsync(dto.VendorCode))
                throw new Exception("كود الموردين موجود بالفعل");

            var vendor = _mapper.Map<Vendor>(dto);
            vendor.CreatedBy = userId;
            vendor.CreatedDate = DateTime.UtcNow;
            vendor.IsActive = true;

            var result = await _repository.AddAsync(vendor);
            await _repository.SaveAsync();
            return result.VendorId;
        }

        public async Task<bool> UpdateVendorAsync(int vendorId, VendorCreateUpdateDto dto, int userId)
        {
            var vendor = await _repository.GetByIdAsync(vendorId);
            if (vendor == null) return false;

            if (vendor.VendorCode != dto.VendorCode && !await IsVendorCodeUniqueAsync(dto.VendorCode, vendorId))
                throw new Exception("كود الموردين موجود بالفعل");

            _mapper.Map(dto, vendor);
            vendor.ModifiedBy = userId;
            vendor.ModifiedDate = DateTime.UtcNow;

            await _repository.UpdateAsync(vendor);
            await _repository.SaveAsync();
            return true;
        }

        public async Task<bool> DeleteVendorAsync(int vendorId, int userId)
        {
            var vendor = await _repository.GetByIdAsync(vendorId);
            if (vendor == null) return false;

            vendor.IsActive = false;
            vendor.ModifiedBy = userId;
            vendor.ModifiedDate = DateTime.UtcNow;

            await _repository.UpdateAsync(vendor);
            await _repository.SaveAsync();
            return true;
        }

        public async Task<bool> IsVendorCodeUniqueAsync(string vendorCode, int excludeVendorId = 0)
        {
            return await _repository.IsVendorCodeUniqueAsync(vendorCode, excludeVendorId);
        }

        public async Task<decimal> GetVendorTotalBalanceAsync(int vendorId)
        {
            return await _repository.GetVendorTotalBalanceAsync(vendorId);
        }

        public async Task<List<VendorDto>> SearchVendorsAsync(string searchTerm)
        {
            var vendors = await _repository.SearchVendorsAsync(searchTerm);
            return _mapper.Map<List<VendorDto>>(vendors);
        }
    }

    public class PurchaseOrderService : IPurchaseOrderService
    {
        private readonly IPurchaseOrderRepository _repository;
        private readonly IPurchaseInvoiceRepository _invoiceRepository;
        private readonly IMapper _mapper;

        public PurchaseOrderService(IPurchaseOrderRepository repository, IPurchaseInvoiceRepository invoiceRepository, IMapper mapper)
        {
            _repository = repository;
            _invoiceRepository = invoiceRepository;
            _mapper = mapper;
        }

        public async Task<PurchaseOrderDto> GetPurchaseOrderAsync(int poId)
        {
            var po = await _repository.GetPurchaseOrderWithDetailsAsync(poId);
            if (po == null) throw new Exception("أمر الشراء غير موجود");
            return _mapper.Map<PurchaseOrderDto>(po);
        }

        public async Task<List<PurchaseOrderDto>> GetPendingPurchaseOrdersAsync()
        {
            var orders = await _repository.GetPendingPurchaseOrdersAsync();
            return _mapper.Map<List<PurchaseOrderDto>>(orders);
        }

        public async Task<int> CreatePurchaseOrderAsync(PurchaseOrderDto dto, int userId)
        {
            var po = _mapper.Map<PurchaseOrder>(dto);
            po.Status = "Draft";
            po.CreatedBy = userId;
            po.CreatedDate = DateTime.UtcNow;

            var result = await _repository.AddAsync(po);
            await _repository.SaveAsync();
            return result.PurchaseOrderId;
        }

        public async Task<bool> ReceivePurchaseOrderAsync(int poId, List<int> quantities, int userId)
        {
            var po = await _repository.GetPurchaseOrderWithDetailsAsync(poId);
            if (po == null) return false;

            int totalQuantity = 0;
            for (int i = 0; i < po.PurchaseOrderDetails.Count; i++)
            {
                po.PurchaseOrderDetails[i].ReceivedQuantity = quantities[i];
                totalQuantity += quantities[i];
            }

            po.Status = "Received";
            po.ModifiedBy = userId;
            po.ModifiedDate = DateTime.UtcNow;

            await _repository.UpdateAsync(po);
            await _repository.SaveAsync();
            await UpdateReceivedQuantityPercentAsync(poId);
            return true;
        }

        public async Task<bool> CompletePurchaseOrderAsync(int poId, int userId)
        {
            var po = await _repository.GetByIdAsync(poId);
            if (po == null) return false;

            po.Status = "Completed";
            po.ModifiedBy = userId;
            po.ModifiedDate = DateTime.UtcNow;

            await _repository.UpdateAsync(po);
            await _repository.SaveAsync();
            return true;
        }

        public async Task<bool> CancelPurchaseOrderAsync(int poId, int userId)
        {
            var po = await _repository.GetByIdAsync(poId);
            if (po == null) return false;

            po.Status = "Cancelled";
            po.ModifiedBy = userId;
            po.ModifiedDate = DateTime.UtcNow;

            await _repository.UpdateAsync(po);
            await _repository.SaveAsync();
            return true;
        }

        public async Task UpdateReceivedQuantityPercentAsync(int poId)
        {
            var po = await _repository.GetPurchaseOrderWithDetailsAsync(poId);
            if (po?.PurchaseOrderDetails.Count > 0)
            {
                decimal orderedTotal = po.PurchaseOrderDetails.Sum(d => d.OrderedQuantity);
                decimal receivedTotal = po.PurchaseOrderDetails.Sum(d => d.ReceivedQuantity);
                
                if (orderedTotal > 0)
                {
                    po.ReceivedQuantityPercent = (receivedTotal / orderedTotal) * 100;
                    await _repository.UpdateAsync(po);
                    await _repository.SaveAsync();
                }
            }
        }
    }

    public class PurchaseInvoiceService : IPurchaseInvoiceService
    {
        private readonly IPurchaseInvoiceRepository _repository;
        private readonly IPurchasePaymentRepository _paymentRepository;
        private readonly IMapper _mapper;

        public PurchaseInvoiceService(IPurchaseInvoiceRepository repository, IPurchasePaymentRepository paymentRepository, IMapper mapper)
        {
            _repository = repository;
            _paymentRepository = paymentRepository;
            _mapper = mapper;
        }

        public async Task<PurchaseInvoiceDto> GetInvoiceAsync(int invoiceId)
        {
            var invoice = await _repository.GetInvoiceWithDetailsAsync(invoiceId);
            if (invoice == null) throw new Exception("الفاتورة غير موجودة");
            return _mapper.Map<PurchaseInvoiceDto>(invoice);
        }

        public async Task<List<PurchaseInvoiceDto>> GetUnpaidInvoicesAsync(int vendorId)
        {
            var invoices = await _repository.GetUnpaidInvoicesAsync(vendorId);
            return _mapper.Map<List<PurchaseInvoiceDto>>(invoices);
        }

        public async Task<int> CreateInvoiceAsync(PurchaseInvoiceDto dto, int userId)
        {
            var invoice = _mapper.Map<PurchaseInvoice>(dto);
            invoice.Status = "Draft";
            invoice.CreatedBy = userId;
            invoice.CreatedDate = DateTime.UtcNow;

            var result = await _repository.AddAsync(invoice);
            await _repository.SaveAsync();
            return result.PurchaseInvoiceId;
        }

        public async Task<bool> PostInvoiceAsync(int invoiceId, int userId)
        {
            var invoice = await _repository.GetByIdAsync(invoiceId);
            if (invoice == null) return false;

            invoice.Status = "Posted";
            invoice.ModifiedBy = userId;
            invoice.ModifiedDate = DateTime.UtcNow;

            await _repository.UpdateAsync(invoice);
            await _repository.SaveAsync();
            return true;
        }

        public async Task<bool> RecordPaymentAsync(int invoiceId, decimal amount, string method, int userId)
        {
            var invoice = await _repository.GetByIdAsync(invoiceId);
            if (invoice == null) return false;

            invoice.PaidAmount += amount;
            invoice.ModifiedBy = userId;
            invoice.ModifiedDate = DateTime.UtcNow;

            await _repository.UpdateAsync(invoice);
            await _repository.SaveAsync();
            return true;
        }

        public async Task<bool> CancelInvoiceAsync(int invoiceId, int userId)
        {
            var invoice = await _repository.GetByIdAsync(invoiceId);
            if (invoice == null) return false;

            invoice.Status = "Cancelled";
            invoice.ModifiedBy = userId;
            invoice.ModifiedDate = DateTime.UtcNow;

            await _repository.UpdateAsync(invoice);
            await _repository.SaveAsync();
            return true;
        }

        public async Task UpdateInvoiceStatusAsync(int invoiceId)
        {
            var invoice = await _repository.GetByIdAsync(invoiceId);
            if (invoice == null) return;

            if (invoice.PaidAmount >= invoice.TotalAmount)
                invoice.Status = "Paid";
            else if (invoice.PaidAmount > 0)
                invoice.Status = "PartiallyPaid";

            await _repository.UpdateAsync(invoice);
            await _repository.SaveAsync();
        }
    }

    public class PurchaseReturnService : IPurchaseReturnService
    {
        private readonly IPurchaseReturnRepository _repository;
        private readonly IMapper _mapper;

        public PurchaseReturnService(IPurchaseReturnRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PurchaseReturnDto> GetReturnAsync(int returnId)
        {
            var ret = await _repository.GetReturnWithDetailsAsync(returnId);
            if (ret == null) throw new Exception("المرتجع غير موجود");
            return _mapper.Map<PurchaseReturnDto>(ret);
        }

        public async Task<List<PurchaseReturnDto>> GetReturnsByVendorAsync(int vendorId)
        {
            var returns = await _repository.GetReturnsByVendorAsync(vendorId);
            return _mapper.Map<List<PurchaseReturnDto>>(returns);
        }

        public async Task<int> CreateReturnAsync(PurchaseReturnDto dto, int userId)
        {
            var ret = _mapper.Map<PurchaseReturn>(dto);
            ret.Status = "Draft";
            ret.CreatedBy = userId;
            ret.CreatedDate = DateTime.UtcNow;

            var result = await _repository.AddAsync(ret);
            await _repository.SaveAsync();
            return result.PurchaseReturnId;
        }

        public async Task<bool> PostReturnAsync(int returnId, int userId)
        {
            var ret = await _repository.GetByIdAsync(returnId);
            if (ret == null) return false;

            ret.Status = "Posted";
            ret.ModifiedBy = userId;
            ret.ModifiedDate = DateTime.UtcNow;

            await _repository.UpdateAsync(ret);
            await _repository.SaveAsync();
            return true;
        }

        public async Task<bool> ApplyCreditNoteAsync(int returnId, int userId)
        {
            var ret = await _repository.GetByIdAsync(returnId);
            if (ret == null) return false;

            ret.Status = "CreditNoteApplied";
            ret.ModifiedBy = userId;
            ret.ModifiedDate = DateTime.UtcNow;

            await _repository.UpdateAsync(ret);
            await _repository.SaveAsync();
            return true;
        }
    }

    public class VendorBalanceService : IVendorBalanceService
    {
        private readonly IVendorBalanceRepository _repository;
        private readonly IMapper _mapper;

        public VendorBalanceService(IVendorBalanceRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<VendorBalanceDto> GetBalanceAsync(int vendorId, int currencyId)
        {
            var balance = await _repository.GetBalanceByVendorAndCurrencyAsync(vendorId, currencyId);
            if (balance == null) throw new Exception("الرصيد غير موجود");
            return _mapper.Map<VendorBalanceDto>(balance);
        }

        public async Task<List<VendorBalanceDto>> GetAllVendorBalancesAsync()
        {
            var balances = await _repository.GetAsync();
            return _mapper.Map<List<VendorBalanceDto>>(balances);
        }

        public async Task UpdateBalanceAsync(int vendorId, int currencyId, decimal amount)
        {
            await _repository.UpdateBalanceAsync(vendorId, currencyId, amount);
        }

        public async Task RecalculateAllBalancesAsync()
        {
            var balances = await _repository.GetAsync();
            foreach (var balance in balances)
            {
                balance.BalanceAmount = balance.TotalAmount - balance.PaidAmount;
            }
            await _repository.SaveAsync();
        }

        public async Task<decimal> GetTotalOutstandingAsync()
        {
            var balances = await _repository.GetAsync();
            return balances.Sum(b => b.BalanceAmount);
        }
    }
}
