using System;
using System.Collections.Generic;

namespace EFA.Domain.Entities
{
    public class Customer
    {
        public int CustomerId { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerNameAr { get; set; }
        public string CustomerNameEn { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public decimal CreditLimit { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        
        public int? CustomerTypeId { get; set; }
        public int? BranchId { get; set; }
        public int? CurrencyId { get; set; }
        public int? LinkedAccountId { get; set; }
        
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }

        // Navigation Properties
        public virtual CustomerType CustomerType { get; set; }
        public virtual Branch Branch { get; set; }
        public virtual Currency Currency { get; set; }
        public virtual ChartOfAccount LinkedAccount { get; set; }
        
        public virtual ICollection<SalesOrder> SalesOrders { get; set; }
        public virtual ICollection<SalesInvoice> SalesInvoices { get; set; }
        public virtual ICollection<SalesReturn> SalesReturns { get; set; }
        public virtual ICollection<SalesPayment> SalesPayments { get; set; }
        public virtual ICollection<CustomerBalance> CustomerBalances { get; set; }
        public virtual ICollection<CustomerContact> CustomerContacts { get; set; }
    }
}
