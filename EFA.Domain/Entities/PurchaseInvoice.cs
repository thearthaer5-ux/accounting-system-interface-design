using System;
using System.Collections.Generic;

namespace EFA.Domain.Entities
{
    public class PurchaseInvoice
    {
        public int PurchaseInvoiceId { get; set; }
        public string InvoiceNumber { get; set; } = null!;
        public string? VendorInvoiceNumber { get; set; }
        
        public int VendorId { get; set; }
        public int? PurchaseOrderId { get; set; }
        
        public DateTime InvoiceDate { get; set; } = DateTime.Now;
        public DateTime? DueDate { get; set; }
        
        public decimal SubTotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; } = 0;
        
        public int? CurrencyId { get; set; }
        public decimal ExchangeRate { get; set; } = 1;
        
        public int? WarehouseId { get; set; }
        public int? BranchId { get; set; }
        
        public string InvoiceStatus { get; set; } = "Draft"; // Draft, Received, PartiallyPaid, Paid, Cancelled
        public bool IsPosted { get; set; } = false;
        
        public int? RelatedJournalId { get; set; }
        public DateTime? PostedDate { get; set; }
        
        public string? Description { get; set; }
        
        public int? CreatedByUserId { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public int? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public virtual Vendor? Vendor { get; set; }
        public virtual PurchaseOrder? PurchaseOrder { get; set; }
        public virtual Currency? Currency { get; set; }
        public virtual Warehouse? Warehouse { get; set; }
        public virtual Branch? Branch { get; set; }
        public virtual Journal? RelatedJournal { get; set; }
        public virtual User? CreatedByUser { get; set; }
        public virtual User? ModifiedByUser { get; set; }
        
        public virtual ICollection<PurchaseInvoiceDetail> PurchaseInvoiceDetails { get; set; } = new List<PurchaseInvoiceDetail>();
        public virtual ICollection<PurchasePayment> PurchasePayments { get; set; } = new List<PurchasePayment>();
    }

    public class PurchaseInvoiceDetail
    {
        public int PurchaseInvoiceDetailId { get; set; }
        public int PurchaseInvoiceId { get; set; }
        public int ItemId { get; set; }
        
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }
        
        public int? BatchId { get; set; }
        public string? Remarks { get; set; }

        public virtual PurchaseInvoice? PurchaseInvoice { get; set; }
        public virtual Item? Item { get; set; }
        public virtual ItemBatch? Batch { get; set; }
    }
}
