using System;
using System.Collections.Generic;

namespace EFA.Application.DTOs
{
    public class CustomerDto
    {
        public int CustomerId { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerNameAr { get; set; }
        public string CustomerNameEn { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public decimal CreditLimit { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateCustomerDto
    {
        public string CustomerCode { get; set; }
        public string CustomerNameAr { get; set; }
        public string CustomerNameEn { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public decimal CreditLimit { get; set; }
        public int? CustomerTypeId { get; set; }
        public int? BranchId { get; set; }
        public int? CurrencyId { get; set; }
    }

    public class SalesOrderDto
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
        public string CustomerName { get; set; }
        public string SalesmanName { get; set; }
        public List<SalesOrderDetailDto> Details { get; set; }
    }

    public class CreateSalesOrderDto
    {
        public int CustomerId { get; set; }
        public int? SalesmanId { get; set; }
        public int? WarehouseId { get; set; }
        public decimal DiscountAmount { get; set; }
        public string Notes { get; set; }
        public List<SalesOrderDetailDto> Details { get; set; }
    }

    public class SalesOrderDetailDto
    {
        public int ItemId { get; set; }
        public decimal OrderedQuantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
    }

    public class SalesInvoiceDto
    {
        public int SalesInvoiceId { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string InvoiceStatus { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal RemainingAmount { get; set; }
        public string CustomerName { get; set; }
        public List<SalesInvoiceDetailDto> Details { get; set; }
    }

    public class CreateSalesInvoiceDto
    {
        public int CustomerId { get; set; }
        public int? SalesOrderId { get; set; }
        public int? SalesmanId { get; set; }
        public decimal DiscountAmount { get; set; }
        public string Notes { get; set; }
        public List<SalesInvoiceDetailDto> Details { get; set; }
    }

    public class SalesInvoiceDetailDto
    {
        public int ItemId { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
    }

    public class SalesReturnDto
    {
        public int SalesReturnId { get; set; }
        public string ReturnNumber { get; set; }
        public DateTime ReturnDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal CreditNoteAmount { get; set; }
        public string Reason { get; set; }
        public string Status { get; set; }
        public string CustomerName { get; set; }
        public List<SalesReturnDetailDto> Details { get; set; }
    }

    public class CreateSalesReturnDto
    {
        public int CustomerId { get; set; }
        public int? SalesInvoiceId { get; set; }
        public string Reason { get; set; }
        public List<SalesReturnDetailDto> Details { get; set; }
    }

    public class SalesReturnDetailDto
    {
        public int ItemId { get; set; }
        public decimal ReturnedQuantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
    }

    public class SalesPaymentDto
    {
        public int SalesPaymentId { get; set; }
        public string PaymentNumber { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal PaymentAmount { get; set; }
        public string PaymentMethod { get; set; }
        public string ReferenceNumber { get; set; }
        public string CustomerName { get; set; }
    }

    public class CreateSalesPaymentDto
    {
        public int CustomerId { get; set; }
        public int? SalesInvoiceId { get; set; }
        public decimal PaymentAmount { get; set; }
        public string PaymentMethod { get; set; }
        public string ReferenceNumber { get; set; }
    }

    public class CustomerBalanceDto
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal BalanceAmount { get; set; }
        public string CurrencyCode { get; set; }
    }

    public class SalesReportDto
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public decimal TotalSales { get; set; }
        public decimal TotalReturns { get; set; }
        public decimal NetSales { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal TotalDue { get; set; }
        public int TransactionCount { get; set; }
        public List<SalesInvoiceDto> SalesInvoices { get; set; }
    }
}
