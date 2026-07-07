using System;
using System.Collections.Generic;

namespace EFA.Domain.Entities
{
    public class Quotation
    {
        public int QuotationId { get; set; }
        public string QuotationNumber { get; set; } = null!;
        public int VendorId { get; set; }
        public DateTime QuotationDate { get; set; } = DateTime.Now;
        public DateTime? ExpiryDate { get; set; }
        
        public decimal SubTotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        
        public int? CurrencyId { get; set; }
        public decimal ExchangeRate { get; set; } = 1;
        
        public string? Description { get; set; }
        public string QuotationStatus { get; set; } = "Draft"; // Draft, Approved, Rejected, Expired
        public int? BranchId { get; set; }
        
        public int? CreatedByUserId { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public int? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public virtual Vendor? Vendor { get; set; }
        public virtual Currency? Currency { get; set; }
        public virtual Branch? Branch { get; set; }
        public virtual User? CreatedByUser { get; set; }
        public virtual User? ModifiedByUser { get; set; }
        
        public virtual ICollection<QuotationDetail> QuotationDetails { get; set; } = new List<QuotationDetail>();
    }

    public class QuotationDetail
    {
        public int QuotationDetailId { get; set; }
        public int QuotationId { get; set; }
        public int ItemId { get; set; }
        
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }
        
        public string? Remarks { get; set; }

        public virtual Quotation? Quotation { get; set; }
        public virtual Item? Item { get; set; }
    }
}
