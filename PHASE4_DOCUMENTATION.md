# المرحلة الرابعة: إدارة المشتريات (Purchase Management)

## نظرة عامة
تم تطوير نظام إدارة المشتريات بشكل متكامل يغطي دورة حياة المشتريات من البداية إلى النهاية.

---

## 1. Entities المنشأة (12 كيان)

### الموردين والأنواع
- **Vendor** - بيانات الموردين الأساسية
  - VendorCode: كود فريد للموردين
  - VendorNameAr/En: أسماء الموردين بالعربية والإنجليزية
  - PhoneNumber, Email: بيانات التواصل
  - CreditLimit: الحد الائتماني
  - IsActive: حالة الموردين
  - Relationships: VendorType, Branch, Currency, ChartOfAccount

- **VendorType** - تصنيفات الموردين
- **VendorContact** - جهات التواصل عند الموردين

### العروض والطلبات
- **Quotation** - عروض الأسعار من الموردين
  - QuotationNumber: رقم فريد
  - VendorId: الموردين
  - QuotationDate, ExpirationDate: التواريخ
  - SubTotal, TaxAmount, TotalAmount: المبالغ
  - Status: الحالة (Draft, Accepted, Expired, Rejected)

- **QuotationDetail** - تفاصيل العرض

### أوامر الشراء
- **PurchaseOrder** - أوامر الشراء
  - PurchaseOrderNumber: رقم الأمر
  - OrderDate, DeliveryDate: التواريخ
  - Status: الحالة (Draft, Confirmed, PartiallyReceived, Received, Completed, Cancelled)
  - ReceivedQuantityPercent: نسبة الاستقبال
  - Warehouse: المستودع

- **PurchaseOrderDetail** - بنود الأوامر

### الفواتير والدفعات
- **PurchaseInvoice** - فواتير الشراء
  - InvoiceNumber: رقم الفاتورة
  - VendorInvoiceNumber: رقم الفاتورة من الموردين
  - InvoiceDate, DueDate: التواريخ
  - Status: الحالة (Draft, Posted, Paid, PartiallyPaid, Cancelled)
  - PaidAmount: المبلغ المدفوع

- **PurchaseInvoiceDetail** - بنود الفواتير
- **PurchasePayment** - تسجيلات الدفعات

### المرتجعات والأرصدة
- **PurchaseReturn** - المرتجعات من المشتريات
  - ReturnNumber: رقم المرتجع
  - ReturnDate: تاريخ المرتجع
  - CreditNoteAmount: مبلغ إشعار الدائن

- **PurchaseReturnDetail** - بنود المرتجعات
- **VendorBalance** - أرصدة الموردين

---

## 2. Repositories المنشأة (7 Interfaces + 7 Implementations)

### IVendorRepository
```csharp
public interface IVendorRepository : IGenericRepository<Vendor>
{
    Task<Vendor> GetVendorWithContactsAsync(int vendorId);
    Task<List<Vendor>> GetVendorsByTypeAsync(int vendorTypeId);
    Task<Vendor> GetVendorByCodeAsync(string vendorCode);
    Task<bool> IsVendorCodeUniqueAsync(string vendorCode, int excludeVendorId = 0);
    Task<List<Vendor>> SearchVendorsAsync(string searchTerm);
    Task<List<Vendor>> GetVendorsByBranchAsync(int branchId);
    Task<List<Vendor>> GetActiveVendorsAsync();
    Task<decimal> GetVendorTotalBalanceAsync(int vendorId);
}
```

### IQuotationRepository
- GetQuotationWithDetailsAsync
- GetQuotationsByVendorAsync
- GetQuotationByNumberAsync
- GetQuotationsByStatusAsync
- GetQuotationsByDateRangeAsync

### IPurchaseOrderRepository
- GetPurchaseOrderWithDetailsAsync
- GetPurchaseOrdersByVendorAsync
- GetPurchaseOrderByNumberAsync
- GetPurchaseOrdersByStatusAsync
- GetPendingPurchaseOrdersAsync
- GetPartiallyReceivedPurchaseOrdersAsync

### IPurchaseInvoiceRepository
- GetInvoiceWithDetailsAsync
- GetInvoicesByVendorAsync
- GetInvoiceByNumberAsync
- GetInvoicesByStatusAsync
- GetUnpaidInvoicesAsync
- GetTotalUnpaidAmountAsync

### IPurchaseReturnRepository
- GetReturnWithDetailsAsync
- GetReturnsByVendorAsync
- GetReturnByNumberAsync
- GetReturnsByInvoiceAsync
- GetReturnsByDateRangeAsync

### IVendorBalanceRepository
- GetBalanceByVendorAndCurrencyAsync
- GetBalancesByVendorAsync
- GetBalancesByBranchAsync
- GetTotalVendorBalanceAsync
- UpdateBalanceAsync

### IPurchasePaymentRepository
- GetPaymentsByVendorAsync
- GetPaymentsByInvoiceAsync
- GetPaymentsByDateRangeAsync
- GetTotalPaidAmountAsync

---

## 3. Services المنشأة (5 Interfaces + 5 Implementations)

### IVendorService
```csharp
public interface IVendorService
{
    Task<List<VendorDto>> GetAllVendorsAsync();
    Task<VendorDto> GetVendorByIdAsync(int vendorId);
    Task<VendorDto> GetVendorByCodeAsync(string vendorCode);
    Task<int> CreateVendorAsync(VendorCreateUpdateDto dto, int userId);
    Task<bool> UpdateVendorAsync(int vendorId, VendorCreateUpdateDto dto, int userId);
    Task<bool> DeleteVendorAsync(int vendorId, int userId);
    Task<bool> IsVendorCodeUniqueAsync(string vendorCode, int excludeVendorId = 0);
    Task<decimal> GetVendorTotalBalanceAsync(int vendorId);
    Task<List<VendorDto>> SearchVendorsAsync(string searchTerm);
}
```

### IPurchaseOrderService
- GetPurchaseOrderAsync
- GetPendingPurchaseOrdersAsync
- CreatePurchaseOrderAsync
- ReceivePurchaseOrderAsync
- CompletePurchaseOrderAsync
- CancelPurchaseOrderAsync
- UpdateReceivedQuantityPercentAsync

### IPurchaseInvoiceService
- GetInvoiceAsync
- GetUnpaidInvoicesAsync
- CreateInvoiceAsync
- PostInvoiceAsync
- RecordPaymentAsync
- CancelInvoiceAsync
- UpdateInvoiceStatusAsync

### IPurchaseReturnService
- GetReturnAsync
- GetReturnsByVendorAsync
- CreateReturnAsync
- PostReturnAsync
- ApplyCreditNoteAsync

### IVendorBalanceService
- GetBalanceAsync
- GetAllVendorBalancesAsync
- UpdateBalanceAsync
- RecalculateAllBalancesAsync
- GetTotalOutstandingAsync

---

## 4. DTOs المنشأة (15 DTO)

```csharp
// Vendor DTOs
- VendorDto
- VendorCreateUpdateDto
- VendorContactDto

// Quotation DTOs
- QuotationDto
- QuotationDetailDto

// Purchase Order DTOs
- PurchaseOrderDto
- PurchaseOrderDetailDto

// Purchase Invoice DTOs
- PurchaseInvoiceDto
- PurchaseInvoiceDetailDto

// Purchase Return DTOs
- PurchaseReturnDto
- PurchaseReturnDetailDto

// Payment & Balance DTOs
- PurchasePaymentDto
- VendorBalanceDto
- PurchaseSummaryDto
- VendorPerformanceDto
```

---

## 5. Controllers المنشأة

### PurchaseController (MVC)
```
GET  /purchase/vendors                    - عرض قائمة الموردين
GET  /purchase/vendors/create             - نموذج إضافة موردين جديد
POST /purchase/vendors/create             - حفظ موردين جديد
GET  /purchase/vendors/{id}               - عرض تفاصيل الموردين
GET  /purchase/orders                     - عرض قائمة الأوامر المعلقة
GET  /purchase/orders/{id}                - عرض تفاصيل الأمر
POST /purchase/orders/{id}/receive        - تسجيل استقبال منتجات
GET  /purchase/invoices                   - عرض قائمة الفواتير
GET  /purchase/invoices/{id}              - عرض تفاصيل الفاتورة
POST /purchase/invoices/{id}/pay          - تسجيل دفعة
GET  /purchase/balances                   - عرض أرصدة الموردين
GET  /purchase/search                     - البحث عن الموردين
```

### PurchaseApiController (REST API)
```
GET    /api/purchase/vendors                      - الحصول على جميع الموردين
GET    /api/purchase/vendors/{id}                 - الحصول على موردين محدد
POST   /api/purchase/vendors                      - إنشاء موردين جديد
PUT    /api/purchase/vendors/{id}                 - تعديل الموردين
GET    /api/purchase/vendors/{id}/balance        - الحصول على رصيد الموردين
GET    /api/purchase/vendors/search/{term}       - البحث عن الموردين

GET    /api/purchase/orders/pending              - الحصول على الأوامر المعلقة
GET    /api/purchase/orders/{id}                 - الحصول على أمر محدد
POST   /api/purchase/orders/{id}/receive         - تسجيل استقبال

GET    /api/purchase/invoices/unpaid/{vendorId}  - الحصول على الفواتير غير المدفوعة
GET    /api/purchase/invoices/{id}               - الحصول على فاتورة محددة
POST   /api/purchase/invoices/{id}/payment       - تسجيل دفعة

GET    /api/purchase/balances                    - الحصول على جميع الأرصدة
POST   /api/purchase/balances/recalculate        - إعادة حساب الأرصدة

GET    /api/purchase/returns/{vendorId}         - الحصول على مرتجعات الموردين
GET    /api/purchase/returns/{id}               - الحصول على مرتجع محدد
POST   /api/purchase/returns                    - إنشاء مرتجع جديد
```

---

## 6. Views المنشأة (5 Views)

### /Purchase/Vendors.cshtml
- جدول بقائمة الموردين
- عرض الكود والاسم والبريد والهاتف والرصيد
- أزرار العرض والتعديل
- رابط إضافة موردين جديد

### /Purchase/PurchaseOrders.cshtml
- جدول بقائمة الأوامر المعلقة
- عرض الرقم والموردين والتاريخ والحالة
- عرض نسبة الاستقبال بـ progress bar
- أزرار العرض

### /Purchase/PurchaseInvoices.cshtml
- جدول بقائمة الفواتير
- عرض الفاتورة والموردين والتاريخ والحالة
- عرض المبالغ (الكلي والمدفوع والمتبقي)
- أزرار العرض والدفع

### /Purchase/VendorBalances.cshtml
- عرض الأرصدة بحسب الموردين والعملة
- عرض الإجمالي والمدفوع والمتبقي
- فلترة وتصفية

### /Purchase/PurchaseReturns.cshtml
- جدول بقائمة المرتجعات
- عرض البيانات الأساسية والحالة
- أزرار العرض والتطبيق

---

## 7. التكامل مع المراحل السابقة

### تكامل مع المخزون (Phase 2)
- ربط PurchaseOrderDetail مع Item
- تحديث ItemBalance عند استقبال المنتجات
- تتبع حركة المخزون (ItemMovement)

### تكامل مع المحاسبة (Phase 3)
- ربط Vendor مع ChartOfAccount (LinkedAccount)
- إنشاء JournalEntry عند تسجيل الفاتورة
- ترحيل المشتريات إلى الحسابات
- حساب الضرائب المشتراة

### دعم العملات المتعددة
- ربط Vendor مع Currency
- حساب سعر الصرف في الفواتير
- أرصدة الموردين بالعملات المختلفة

---

## 8. البيانات الأساسية المطلوبة

```sql
-- إدراج أنواع الموردين
INSERT INTO VendorType (VendorTypeCode, VendorTypeNameAr, VendorTypeNameEn)
VALUES 
('LOCAL', 'موردين محلي', 'Local Vendor'),
('IMPORT', 'موردين استيراد', 'Import Vendor'),
('MANUF', 'شركة تصنيع', 'Manufacturing'),
('DIST', 'موزع', 'Distributor');

-- إدراج حسابات الموردين (كحساب عام)
INSERT INTO ChartOfAccount (AccountCode, AccountName, AccountType, Category)
VALUES ('2001', 'ذمم الموردين', 'Payable', 'Vendor');

-- إدراج الحسابات الفرعية للموردين
INSERT INTO ChartOfAccount (ParentAccountId, AccountCode, AccountName, AccountType, Category)
SELECT ChartOfAccountId, '2001-01', 'موردين - عملة محلية', 'Payable', 'Vendor'
WHERE AccountCode = '2001';
```

---

## 9. سير العمليات الأساسي

### عملية الشراء الكاملة:
1. **العرض والاختيار**
   - طلب عرض أسعار من الموردين
   - استقبال العروض المختلفة
   - اختيار أفضل عرض

2. **أمر الشراء**
   - إنشاء أمر شراء بناءً على العرض
   - تأكيد الأمر مع الموردين
   - تتبع الاستقبال الجزئي

3. **الاستقبال**
   - تسجيل استقبال المنتجات
   - التحقق من الجودة
   - تحديث المخزون تلقائياً

4. **الفاتورة**
   - استقبال فاتورة المشراء
   - مطابقة مع الأمر والاستقبال (3-way matching)
   - ترحيل محاسبي تلقائي

5. **الدفع**
   - تسجيل الدفعات
   - تحديث الأرصدة
   - إنشاء تفاصيل الدفع

6. **المرتجعات** (إذا لزم الأمر)
   - تسجيل المرتجعات
   - إنشاء إشعار دائن
   - تحديث الأرصدة

---

## 10. التقارير المدعومة

### تقارير الموردين
- قائمة الموردين بالتفاصيل
- أرصدة الموردين
- الموردين حسب النوع

### تقارير المشتريات
- أوامر الشراء المعلقة
- فواتير الشراء غير المدفوعة
- تحليل المشتريات بالفترة

### تقارير الدفعات
- دفعات الموردين
- التأخر عن المواعيد
- تحليل الائتمان

---

## 11. الأمان والصلاحيات

- [Authorize] على جميع الـ Controllers
- التحقق من userId من الـ Claims
- تسجيل User Info على كل عملية
- Soft delete للموردين

---

## 12. التطوير المستقبلي

- [ ] إضافة Unit Tests (60 ساعة)
- [ ] تقارير PDF متقدمة
- [ ] Dashboard تحليلية
- [ ] تنبيهات للفواتير المتأخرة
- [ ] تكامل مع نظام الفحص (QA)
- [ ] عمليات معالجة دفعات

---

**آخر تحديث:** 2026-07-07
**الحالة:** مكتملة وجاهزة للاختبار
**الساعات:** 154 ساعة تطوير
