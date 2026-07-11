using System;
using System.Collections.Generic;

namespace EFA.Domain.Entities
{
    public class SalesInvoice
    {
        public int SalesInvoiceId { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string InvoiceStatus { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public string Notes { get; set; }

        public int? CustomerId { get; set; }
        public int? SalesOrderId { get; set; }
        public int? SalesmanId { get; set; }
        public int? BranchId { get; set; }
        public int? WarehouseId { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual SalesOrder SalesOrder { get; set; }
        public virtual Salesman Salesman { get; set; }
        public virtual Branch Branch { get; set; }
        public virtual Warehouse Warehouse { get; set; }
        public virtual ICollection<SalesInvoiceDetail> SalesInvoiceDetails { get; set; }
        public virtual ICollection<SalesReturn> SalesReturns { get; set; }
        public virtual ICollection<SalesPayment> SalesPayments { get; set; }
    }

    public class SalesInvoiceDetail
    {
        public int SalesInvoiceDetailId { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }

        public int SalesInvoiceId { get; set; }
        public int? ItemId { get; set; }

        public virtual SalesInvoice SalesInvoice { get; set; }
        public virtual Item Item { get; set; }
    }
}
