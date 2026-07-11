using System;
using System.Collections.Generic;

namespace EFA.Domain.Entities
{
    public class SalesReturn
    {
        public int SalesReturnId { get; set; }
        public string ReturnNumber { get; set; }
        public DateTime ReturnDate { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal CreditNoteAmount { get; set; }
        public string Reason { get; set; }
        public string Status { get; set; }

        public int? CustomerId { get; set; }
        public int? SalesInvoiceId { get; set; }
        public int? BranchId { get; set; }
        public int? WarehouseId { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual SalesInvoice SalesInvoice { get; set; }
        public virtual Branch Branch { get; set; }
        public virtual Warehouse Warehouse { get; set; }
        public virtual ICollection<SalesReturnDetail> SalesReturnDetails { get; set; }
    }

    public class SalesReturnDetail
    {
        public int SalesReturnDetailId { get; set; }
        public decimal ReturnedQuantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }

        public int SalesReturnId { get; set; }
        public int? ItemId { get; set; }

        public virtual SalesReturn SalesReturn { get; set; }
        public virtual Item Item { get; set; }
    }
}
