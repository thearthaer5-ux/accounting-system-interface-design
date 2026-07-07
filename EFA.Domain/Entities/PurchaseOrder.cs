using System;
using System.Collections.Generic;

namespace EFA.Domain.Entities
{
    public class PurchaseOrder
    {
        public int PurchaseOrderId { get; set; }
        public string PurchaseOrderNumber { get; set; } = null!;
        public int VendorId { get; set; }
        
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public DateTime? RequiredDeliveryDate { get; set; }
        
        public decimal SubTotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }
        
        public int? CurrencyId { get; set; }
        public decimal ExchangeRate { get; set; } = 1;
        
        public int? WarehouseId { get; set; }
        public int? BranchId { get; set; }
        
        public string POStatus { get; set; } = "Draft"; // Draft, Confirmed, PartiallyReceived, Received, Cancelled
        public decimal ReceivedQuantityPercent { get; set; } = 0;
        
        public string? DeliveryAddress { get; set; }
        public string? PaymentTerms { get; set; }
        public string? Description { get; set; }
        
        public int? LinkedQuotationId { get; set; }
        public int? RelatedJournalId { get; set; }
        
        public int? CreatedByUserId { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public int? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public virtual Vendor? Vendor { get; set; }
        public virtual Currency? Currency { get; set; }
        public virtual Warehouse? Warehouse { get; set; }
        public virtual Branch? Branch { get; set; }
        public virtual Quotation? LinkedQuotation { get; set; }
        public virtual Journal? RelatedJournal { get; set; }
        public virtual User? CreatedByUser { get; set; }
        public virtual User? ModifiedByUser { get; set; }
        
        public virtual ICollection<PurchaseOrderDetail> PurchaseOrderDetails { get; set; } = new List<PurchaseOrderDetail>();
    }

    public class PurchaseOrderDetail
    {
        public int PurchaseOrderDetailId { get; set; }
        public int PurchaseOrderId { get; set; }
        public int ItemId { get; set; }
        
        public decimal OrderedQuantity { get; set; }
        public decimal ReceivedQuantity { get; set; } = 0;
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }
        
        public int? BatchId { get; set; }
        public string? Remarks { get; set; }

        public virtual PurchaseOrder? PurchaseOrder { get; set; }
        public virtual Item? Item { get; set; }
        public virtual ItemBatch? Batch { get; set; }
    }
}
