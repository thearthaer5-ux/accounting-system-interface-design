using System;
using System.Collections.Generic;

namespace EFA.Domain.Entities
{
    public class PurchaseReturn
    {
        public int PurchaseReturnId { get; set; }
        public string ReturnNumber { get; set; } = null!;
        
        public int VendorId { get; set; }
        public int? PurchaseInvoiceId { get; set; }
        
        public DateTime ReturnDate { get; set; } = DateTime.Now;
        
        public decimal SubTotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal CreditNoteAmount { get; set; } = 0;
        
        public int? CurrencyId { get; set; }
        public decimal ExchangeRate { get; set; } = 1;
        
        public int? WarehouseId { get; set; }
        public int? BranchId { get; set; }
        
        public string ReturnStatus { get; set; } = "Draft"; // Draft, Approved, PartiallyReceived, Received, Credited
        public bool IsPosted { get; set; } = false;
        
        public int? RelatedJournalId { get; set; }
        public DateTime? PostedDate { get; set; }
        
        public string? Reason { get; set; }
        public string? Description { get; set; }
        
        public int? CreatedByUserId { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public int? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public virtual Vendor? Vendor { get; set; }
        public virtual PurchaseInvoice? PurchaseInvoice { get; set; }
        public virtual Currency? Currency { get; set; }
        public virtual Warehouse? Warehouse { get; set; }
        public virtual Branch? Branch { get; set; }
        public virtual Journal? RelatedJournal { get; set; }
        public virtual User? CreatedByUser { get; set; }
        public virtual User? ModifiedByUser { get; set; }
        
        public virtual ICollection<PurchaseReturnDetail> PurchaseReturnDetails { get; set; } = new List<PurchaseReturnDetail>();
    }

    public class PurchaseReturnDetail
    {
        public int PurchaseReturnDetailId { get; set; }
        public int PurchaseReturnId { get; set; }
        public int ItemId { get; set; }
        
        public decimal ReturnedQuantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }
        
        public int? BatchId { get; set; }
        public string? Reason { get; set; }
        public string? Remarks { get; set; }

        public virtual PurchaseReturn? PurchaseReturn { get; set; }
        public virtual Item? Item { get; set; }
        public virtual ItemBatch? Batch { get; set; }
    }
}
