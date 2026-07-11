using System;
using System.Collections.Generic;

namespace EFA.Domain.Entities
{
    public class SalesOrder
    {
        public int SalesOrderId { get; set; }
        public string SalesOrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? RequiredDeliveryDate { get; set; }
        public string OrderStatus { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string Notes { get; set; }

        public int? CustomerId { get; set; }
        public int? SalesmanId { get; set; }
        public int? WarehouseId { get; set; }
        public int? BranchId { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual Salesman Salesman { get; set; }
        public virtual Warehouse Warehouse { get; set; }
        public virtual Branch Branch { get; set; }
        public virtual ICollection<SalesOrderDetail> SalesOrderDetails { get; set; }
        public virtual ICollection<SalesInvoice> SalesInvoices { get; set; }
    }

    public class SalesOrderDetail
    {
        public int SalesOrderDetailId { get; set; }
        public decimal OrderedQuantity { get; set; }
        public decimal DeliveredQuantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }
        public string Notes { get; set; }

        public int SalesOrderId { get; set; }
        public int? ItemId { get; set; }

        public virtual SalesOrder SalesOrder { get; set; }
        public virtual Item Item { get; set; }
    }
}
