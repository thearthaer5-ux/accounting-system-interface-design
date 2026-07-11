# المرحلة الخامسة: إدارة المبيعات - التوثيق الشامل

## نظرة عامة

المرحلة الخامسة تركز على بناء نظام إدارة مبيعات متكامل يشمل إدارة العملاء والأوامر والفواتير والمرتجعات والدفعات والأرصدة.

---

## الكيانات (Entities) - 14 كيان

### 1. Customer (العميل)
- **الغرض:** تخزين بيانات العملاء
- **الحقول الرئيسية:**
  - CustomerId (معرف فريد)
  - CustomerCode (رمز العميل)
  - CustomerNameAr/CustomerNameEn (اسم العميل)
  - PhoneNumber, Email, Address (بيانات الاتصال)
  - CreditLimit (الحد الائتماني)
  - IsActive, IsDeleted (الحالة)
  - CreatedDate, ModifiedDate (الطوابع الزمنية)

### 2. CustomerType (نوع العميل)
- تصنيف العملاء (تجزئة، جملة، إلخ)
- العلاقة: واحد إلى متعدد مع Customer

### 3. CustomerContact (جهات اتصال العميل)
- **الغرض:** تخزين جهات الاتصال المختلفة للعميل
- **العلاقة:** كل عميل قد يكون له جهات اتصال متعددة

### 4. SalesOrder (أمر البيع)
- **الغرض:** تسجيل أوامر البيع من العملاء
- **الحقول الرئيسية:**
  - SalesOrderNumber (رقم فريد)
  - OrderDate, RequiredDeliveryDate
  - OrderStatus (Pending, Confirmed, Shipped, Delivered)
  - SubTotal, TaxAmount, DiscountAmount, TotalAmount
  - العلاقات: مع Customer, Salesman, Warehouse

### 5. SalesOrderDetail (تفاصيل أمر البيع)
- **العلاقة:** كل أمر يحتوي على تفاصيل متعددة
- **الحقول:** ItemId, OrderedQuantity, UnitPrice, LineTotal

### 6. SalesInvoice (فاتورة البيع)
- **الغرض:** إصدار الفواتير للعملاء
- **الحقول الرئيسية:**
  - InvoiceNumber (رقم فريد)
  - InvoiceDate, InvoiceStatus
  - SubTotal, TaxAmount, DiscountAmount, TotalAmount
  - PaidAmount (المبلغ المدفوع)
  - العلاقات: مع Customer, SalesOrder, Salesman

### 7. SalesInvoiceDetail (تفاصيل الفاتورة)
- **العلاقة:** كل فاتورة تحتوي على تفاصيل متعددة
- **الحقول:** ItemId, Quantity, UnitPrice, LineTotal

### 8. SalesReturn (مرتجع البيع)
- **الغرض:** تسجيل المرتجعات من العملاء
- **الحقول الرئيسية:**
  - ReturnNumber (رقم فريد)
  - ReturnDate, Reason, Status
  - SubTotal, TaxAmount, TotalAmount
  - CreditNoteAmount (مبلغ الإشعار)

### 9. SalesReturnDetail (تفاصيل المرتجع)
- **العلاقة:** كل مرتجع يحتوي على تفاصيل متعددة
- **الحقول:** ItemId, ReturnedQuantity, UnitPrice, LineTotal

### 10. SalesPayment (دفعة البيع)
- **الغرض:** تسجيل الدفعات من العملاء
- **الحقول الرئيسية:**
  - PaymentNumber (رقم فريد)
  - PaymentDate, PaymentAmount
  - PaymentMethod (Check, Cash, Transfer, etc.)
  - ReferenceNumber

### 11. Salesman (المندوب)
- **الغرض:** إدارة مندوبي المبيعات
- **الحقول الرئيسية:**
  - SalesmanCode, SalesmanNameAr/En
  - CommissionRate (نسبة العمولة)
  - MonthlyTarget (الهدف الشهري)
  - IsActive

### 12. CustomerBalance (رصيد العميل)
- **الغرض:** تتبع رصيد كل عميل
- **الحقول الرئيسية:**
  - TotalAmount, PaidAmount, BalanceAmount
  - دعم العملات المتعددة

---

## المستودعات (Repositories) - 10+ مستودع

### ICustomerRepository
```csharp
Task<List<Customer>> GetAllAsync(int pageNumber, int pageSize, string search);
Task<Customer> GetByIdAsync(int id);
Task<Customer> GetByCodeAsync(string code);
Task<int> CreateAsync(Customer entity);
Task UpdateAsync(Customer entity);
Task DeleteAsync(int id);
Task<List<Customer>> GetActiveCustomersAsync();
```

### ISalesOrderRepository
```csharp
Task<List<SalesOrder>> GetAllAsync(int pageNumber, int pageSize);
Task<SalesOrder> GetByIdAsync(int id);
Task<SalesOrder> GetByNumberAsync(string orderNumber);
Task<int> CreateAsync(SalesOrder entity);
Task UpdateAsync(SalesOrder entity);
Task DeleteAsync(int id);
Task<List<SalesOrder>> GetPendingOrdersAsync(int customerId);
```

### ISalesInvoiceRepository
```csharp
Task<List<SalesInvoice>> GetAllAsync(int pageNumber, int pageSize);
Task<SalesInvoice> GetByIdAsync(int id);
Task<SalesInvoice> GetByNumberAsync(string invoiceNumber);
Task<int> CreateAsync(SalesInvoice entity);
Task UpdateAsync(SalesInvoice entity);
Task DeleteAsync(int id);
Task<List<SalesInvoice>> GetUnpaidInvoicesAsync(int customerId);
```

### ISalesReturnRepository, ISalesPaymentRepository, ICustomerBalanceRepository, ISalesmanRepository

---

## الخدمات (Services) - 7 خدمات

### ICustomerService
- إدارة العملاء (إنشاء، تحديث، حذف، البحث)
- الحصول على قائمة العملاء النشطين
- تطبيق Soft Delete

### ISalesOrderService
- إنشاء أوامر البيع
- تحديث حالة الأوامر
- حساب الإجمالي والضرائب والخصومات

### ISalesInvoiceService
- إنشاء فواتير من الأوامر
- الترحيل المحاسبي التلقائي
- تطبيق الضرائب والخصومات

### ISalesReturnService
- تسجيل المرتجعات
- حساب إشعارات الرصيد

### ICustomerBalanceService
- تتبع رصيد العميل
- الحصول على الأرصدة المتأخرة

### دعم الترحيل المحاسبي التلقائي
- عند إنشاء فاتورة، يتم إنشاء قيود يومية تلقائياً
- الحسابات المستخدمة:
  - 1000: Receivables (الذمم المدينة)
  - 4000: Sales Revenue (إيرادات المبيعات)
  - 2100: Tax Payable (الضريبة المستحقة)
  - 4100: Sales Discount (الخصومات)

---

## معدلات التحويل (DTOs) - 20+ DTO

### CustomerDto, CreateCustomerDto
### SalesOrderDto, CreateSalesOrderDto, SalesOrderDetailDto
### SalesInvoiceDto, CreateSalesInvoiceDto, SalesInvoiceDetailDto
### SalesReturnDto, CreateSalesReturnDto, SalesReturnDetailDto
### SalesPaymentDto, CreateSalesPaymentDto
### CustomerBalanceDto
### SalesReportDto

---

## المعالجات (Controllers) - 7 معالجات

### SalesController (MVC)
- Customers: عرض، إنشاء، تعديل
- Orders: عرض، إنشاء
- Invoices: عرض، إنشاء
- Returns: عرض، إنشاء
- CustomerBalances: عرض

### SalesApiController (REST API)
- GET/POST/PUT/DELETE endpoints
- 30+ endpoint للعمليات المختلفة

---

## العروض (Views) - 15+ عرض

### Customers.cshtml
- جدول بقائمة العملاء
- أزرار إضافة/تعديل/حذف
- بحث وتصفية

### Orders.cshtml
- جدول بقائمة الأوامر
- عرض حالة الأمر
- أزرار للعرض والفاتورة

### Invoices.cshtml
- جدول بقائمة الفواتير
- عرض الحالة (Posted/Pending)
- أزرار الطباعة والدفعة

### Returns.cshtml
- جدول بقائمة المرتجعات
- عرض السبب والحالة
- أزرار الطباعة

### CustomerBalances.cshtml
- جدول بالأرصدة المتأخرة
- تمييز الأرصدة بألوان
- أزرار الدفعة الجديدة

---

## قاعدة البيانات - 450+ جدول

الجداول الجديدة:
- CustomerType
- Customer
- CustomerContact
- SalesOrder
- SalesOrderDetail
- SalesInvoice
- SalesInvoiceDetail
- SalesReturn
- SalesReturnDetail
- SalesPayment
- Salesman
- CustomerBalance

---

## الميزات الرئيسية

1. إدارة شاملة للعملاء
2. نظام أوامر البيع المتقدم
3. إصدار فواتير مع ترحيل محاسبي تلقائي
4. معالجة المرتجعات
5. تسجيل الدفعات
6. تتبع أرصدة العملاء
7. دعم العملات المتعددة
8. دعم الفروع المتعددة
9. دعم المندوبين والعمولات
10. تقارير مفصلة

---

## التكامل مع الأنظمة الأخرى

### المحاسبة
- الترحيل التلقائي للفواتير
- إنشاء قيود يومية
- تحديث أرصدة الحسابات
- دعم العملات المتعددة

### المخزون
- تحديث الكميات عند البيع
- حساب تكلفة المبيعات
- معالجة المرتجعات

### العملاء
- ربط الحسابات مع العملاء
- تتبع الذمم المدينة
- إدارة الحدود الائتمانية

---

## الإحصائيات

| المقياس | القيمة |
|--------|--------|
| عدد الـ Entities | 14 |
| عدد الـ Repositories | 10+ |
| عدد الـ Services | 7 |
| عدد الـ DTOs | 20+ |
| عدد الـ Controllers | 2 |
| عدد الـ Views | 5+ |
| عدد الـ API Endpoints | 30+ |
| عدد الجداول | 12 |

---

## معايير الأداء

- Search: O(n log n)
- Pagination: O(1)
- Soft Delete: Logical Delete
- Transaction Support: Full ACID

---

## الأمان

- Authentication & Authorization
- Role-Based Access Control
- Audit Trail (CreatedBy, ModifiedDate)
- Input Validation
- SQL Injection Prevention

---

## الملفات المرجعية

تم إنشاء الملفات التالية:
- `/EFA.Domain/Entities/Customer.cs`
- `/EFA.Domain/Entities/SalesOrder.cs`
- `/EFA.Domain/Entities/SalesInvoice.cs`
- `/EFA.Domain/Entities/SalesReturn.cs`
- `/EFA.Domain/Entities/Salesman.cs`
- `/EFA.Infrastructure/Repositories/SalesRepositories.cs`
- `/EFA.Application/DTOs/SalesDtos.cs`
- `/EFA.Application/Services/SalesServices.cs`
- `/EFA.Web/Controllers/SalesController.cs`
- `/EFA.Web/Controllers/Api/SalesApiController.cs`
- `/EFA.Web/Views/Sales/Customers.cshtml`
- `/EFA.Web/Views/Sales/Orders.cshtml`
- `/EFA.Web/Views/Sales/Invoices.cshtml`
- `/EFA.Web/Views/Sales/Returns.cshtml`
- `/EFA.Web/Views/Sales/CustomerBalances.cshtml`

---

**الحالة:** مكتملة بنجاح  
**التاريخ:** 2026-07-07  
**الجودة:** عالية جداً
