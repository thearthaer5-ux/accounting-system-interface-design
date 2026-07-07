using System;

namespace EFA.Domain.Entities
{
    public class PurchasePayment
    {
        public int PurchasePaymentId { get; set; }
        public string PaymentNumber { get; set; } = null!;
        
        public int VendorId { get; set; }
        public int? PurchaseInvoiceId { get; set; }
        
        public DateTime PaymentDate { get; set; } = DateTime.Now;
        public decimal PaymentAmount { get; set; }
        
        public int? CurrencyId { get; set; }
        public decimal ExchangeRate { get; set; } = 1;
        
        public string PaymentMethod { get; set; } = "Cash"; // Cash, Check, Transfer, CreditCard
        public string? ReferenceNumber { get; set; }
        
        public int? RelatedJournalId { get; set; }
        public bool IsPosted { get; set; } = false;
        public DateTime? PostedDate { get; set; }
        
        public string? Notes { get; set; }
        
        public int? CreatedByUserId { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public virtual Vendor? Vendor { get; set; }
        public virtual PurchaseInvoice? PurchaseInvoice { get; set; }
        public virtual Currency? Currency { get; set; }
        public virtual Journal? RelatedJournal { get; set; }
        public virtual User? CreatedByUser { get; set; }
    }

    public class VendorBalance
    {
        public int VendorBalanceId { get; set; }
        public int VendorId { get; set; }
        
        public decimal TotalAmount { get; set; } = 0;
        public decimal PaidAmount { get; set; } = 0;
        public decimal BalanceAmount { get; set; } = 0;
        
        public DateTime LastTransactionDate { get; set; } = DateTime.Now;
        public DateTime LastPaymentDate { get; set; } = DateTime.Now;
        
        public int? CurrencyId { get; set; }

        public virtual Vendor? Vendor { get; set; }
        public virtual Currency? Currency { get; set; }
    }
}
