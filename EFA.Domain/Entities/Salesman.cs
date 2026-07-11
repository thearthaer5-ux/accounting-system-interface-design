using System;
using System.Collections.Generic;

namespace EFA.Domain.Entities
{
    public class Salesman
    {
        public int SalesmanId { get; set; }
        public string SalesmanCode { get; set; }
        public string SalesmanNameAr { get; set; }
        public string SalesmanNameEn { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public decimal CommissionRate { get; set; }
        public decimal MonthlyTarget { get; set; }
        public bool IsActive { get; set; }

        public int? BranchId { get; set; }
        public int? UserId { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public virtual Branch Branch { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<SalesOrder> SalesOrders { get; set; }
        public virtual ICollection<SalesInvoice> SalesInvoices { get; set; }
    }

    public class SalesPayment
    {
        public int SalesPaymentId { get; set; }
        public string PaymentNumber { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal PaymentAmount { get; set; }
        public string PaymentMethod { get; set; }
        public string ReferenceNumber { get; set; }

        public int? CustomerId { get; set; }
        public int? SalesInvoiceId { get; set; }
        public int? BranchId { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? CreatedBy { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual SalesInvoice SalesInvoice { get; set; }
        public virtual Branch Branch { get; set; }
    }

    public class CustomerBalance
    {
        public int CustomerBalanceId { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal BalanceAmount { get; set; }

        public int? CustomerId { get; set; }
        public int? CurrencyId { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual Currency Currency { get; set; }
    }

    public class CustomerContact
    {
        public int CustomerContactId { get; set; }
        public string ContactName { get; set; }
        public string Position { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        public int? CustomerId { get; set; }

        public virtual Customer Customer { get; set; }
    }
}
