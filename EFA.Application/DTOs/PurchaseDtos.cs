using System;
using System.Collections.Generic;

namespace EFA.Application.DTOs
{
    // Vendor DTOs
    public class VendorDto
    {
        public int VendorId { get; set; }
        public string VendorCode { get; set; }
        public string VendorNameAr { get; set; }
        public string VendorNameEn { get; set; }
        public string VendorTypeNameAr { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public decimal CreditLimit { get; set; }
        public string CurrencyCode { get; set; }
        public bool IsActive { get; set; }
        public decimal TotalBalance { get; set; }
        public List<VendorContactDto> Contacts { get; set; } = new();
    }

    public class VendorCreateUpdateDto
    {
        public string VendorCode { get; set; }
        public string VendorNameAr { get; set; }
        public string VendorNameEn { get; set; }
        public int VendorTypeId { get; set; }
        public int BranchId { get; set; }
        public int CurrencyId { get; set; }
        public int? LinkedAccountId { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public decimal CreditLimit { get; set; }
        public bool IsActive { get; set; }
    }

    public class VendorContactDto
    {
        public int VendorContactId { get; set; }
        public string ContactName { get; set; }
        public string Position { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsActive { get; set; }
    }

    // Quotation DTOs
    public class QuotationDto
    {
        public int QuotationId { get; set; }
        public string QuotationNumber { get; set; }
        public string VendorName { get; set; }
        public DateTime QuotationDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string Status { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string Notes { get; set; }
        public List<QuotationDetailDto> Details { get; set; } = new();
    }

    public class QuotationDetailDto
    {
        public int QuotationDetailId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }
        public string UnitName { get; set; }
    }

    // Purchase Order DTOs
    public class PurchaseOrderDto
    {
        public int PurchaseOrderId { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public string VendorName { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string Status { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal ReceivedQuantityPercent { get; set; }
        public string WarehouseName { get; set; }
        public List<PurchaseOrderDetailDto> Details { get; set; } = new();
    }

    public class PurchaseOrderDetailDto
    {
        public int PurchaseOrderDetailId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public decimal OrderedQuantity { get; set; }
        public decimal ReceivedQuantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }
    }

    // Purchase Invoice DTOs
    public class PurchaseInvoiceDto
    {
        public int PurchaseInvoiceId { get; set; }
        public string InvoiceNumber { get; set; }
        public string VendorInvoiceNumber { get; set; }
        public string VendorName { get; set; }
        public DateTime InvoiceDate { get; set; }
        public DateTime DueDate { get; set; }
        public string Status { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal RemainingAmount { get; set; }
        public string Notes { get; set; }
        public List<PurchaseInvoiceDetailDto> Details { get; set; } = new();
        public List<PurchasePaymentDto> Payments { get; set; } = new();
    }

    public class PurchaseInvoiceDetailDto
    {
        public int PurchaseInvoiceDetailId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }
    }

    // Purchase Return DTOs
    public class PurchaseReturnDto
    {
        public int PurchaseReturnId { get; set; }
        public string ReturnNumber { get; set; }
        public string VendorName { get; set; }
        public DateTime ReturnDate { get; set; }
        public string Status { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal CreditNoteAmount { get; set; }
        public string Reason { get; set; }
        public List<PurchaseReturnDetailDto> Details { get; set; } = new();
    }

    public class PurchaseReturnDetailDto
    {
        public int PurchaseReturnDetailId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public decimal ReturnedQuantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }
    }

    // Purchase Payment DTOs
    public class PurchasePaymentDto
    {
        public int PurchasePaymentId { get; set; }
        public string PaymentNumber { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal PaymentAmount { get; set; }
        public string PaymentMethod { get; set; }
        public string ReferenceNumber { get; set; }
        public string Notes { get; set; }
    }

    // Vendor Balance DTOs
    public class VendorBalanceDto
    {
        public int VendorBalanceId { get; set; }
        public string VendorName { get; set; }
        public string CurrencyCode { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal BalanceAmount { get; set; }
        public DateTime LastUpdateDate { get; set; }
    }

    // Purchase Summary DTOs
    public class PurchaseSummaryDto
    {
        public int PeriodMonthYear { get; set; }
        public decimal TotalPurchases { get; set; }
        public decimal TotalPayments { get; set; }
        public decimal OutstandingBalance { get; set; }
        public int VendorCount { get; set; }
        public int InvoiceCount { get; set; }
    }

    public class VendorPerformanceDto
    {
        public int VendorId { get; set; }
        public string VendorName { get; set; }
        public decimal TotalPurchased { get; set; }
        public int OrderCount { get; set; }
        public decimal AverageOrderValue { get; set; }
        public int OnTimeDeliveryPercent { get; set; }
        public decimal QualityRating { get; set; }
    }
}
