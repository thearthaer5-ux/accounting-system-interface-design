using System;
using System.Collections.Generic;

namespace EFA.Domain.Entities
{
    /// <summary>
    /// جدول الموردين - Vendors
    /// يحتوي على معلومات الموردين والشركات الموردة للمنشأة
    /// </summary>
    public class Vendor
    {
        public int VendorId { get; set; }
        
        /// <summary>كود الموردين الفريد</summary>
        public string VendorCode { get; set; } = null!;
        
        /// <summary>اسم المورد بالعربية</summary>
        public string VendorNameAr { get; set; } = null!;
        
        /// <summary>اسم المورد بالإنجليزية</summary>
        public string? VendorNameEn { get; set; }
        
        /// <summary>نوع المورد</summary>
        public int? VendorTypeId { get; set; }
        
        /// <summary>رقم الاتصال الرئيسي</summary>
        public string? PhoneNumber { get; set; }
        
        /// <summary>رقم الفاكس</summary>
        public string? FaxNumber { get; set; }
        
        /// <summary>البريد الإلكتروني</summary>
        public string? Email { get; set; }
        
        /// <summary>العنوان</summary>
        public string? Address { get; set; }
        
        /// <summary>الدولة</summary>
        public string? Country { get; set; }
        
        /// <summary>المدينة</summary>
        public string? City { get; set; }
        
        /// <summary>الرمز البريدي</summary>
        public string? PostalCode { get; set; }
        
        /// <summary>الاسم الشخصي للمورد</summary>
        public string? ContactPersonName { get; set; }
        
        /// <summary>شروط الدفع (أيام)</summary>
        public int PaymentTermsDays { get; set; } = 30;
        
        /// <summary>حد الائتمان الأقصى</summary>
        public decimal CreditLimit { get; set; } = 0;
        
        /// <summary>العملة الافتراضية</summary>
        public int? CurrencyId { get; set; }
        
        /// <summary>الفرع</summary>
        public int? BranchId { get; set; }
        
        /// <summary>الحساب المحاسبي المرتبط</summary>
        public int? LinkedAccountId { get; set; }
        
        /// <summary>حالة المورد (نشط/معطل)</summary>
        public bool IsActive { get; set; } = true;
        
        /// <summary>ملاحظات</summary>
        public string? Remarks { get; set; }
        
        /// <summary>معرف المستخدم الذي قام بالإنشاء</summary>
        public int? CreatedByUserId { get; set; }
        
        /// <summary>تاريخ الإنشاء</summary>
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        
        /// <summary>معرف المستخدم الذي قام بآخر تعديل</summary>
        public int? ModifiedByUserId { get; set; }
        
        /// <summary>تاريخ آخر تعديل</summary>
        public DateTime? ModifiedDate { get; set; }

        // Navigation properties
        public virtual VendorType? VendorType { get; set; }
        public virtual Branch? Branch { get; set; }
        public virtual Currency? Currency { get; set; }
        public virtual ChartOfAccount? LinkedAccount { get; set; }
        public virtual User? CreatedByUser { get; set; }
        public virtual User? ModifiedByUser { get; set; }
        
        public virtual ICollection<VendorContact> VendorContacts { get; set; } = new List<VendorContact>();
        public virtual ICollection<Quotation> Quotations { get; set; } = new List<Quotation>();
        public virtual ICollection<PurchaseOrder> PurchaseOrders { get; set; } = new List<PurchaseOrder>();
        public virtual ICollection<PurchaseInvoice> PurchaseInvoices { get; set; } = new List<PurchaseInvoice>();
        public virtual ICollection<PurchaseReturn> PurchaseReturns { get; set; } = new List<PurchaseReturn>();
        public virtual ICollection<VendorBalance> VendorBalances { get; set; } = new List<VendorBalance>();
    }
}
