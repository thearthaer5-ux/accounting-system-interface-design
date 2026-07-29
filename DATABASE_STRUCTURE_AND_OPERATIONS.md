# قاعدة البيانات - الهيكل والعمليات
## نظام المحاسبة المتكامل (EFA)

---

## 📊 نظرة عامة على قاعدة البيانات

### إجمالي الجداول: 42
### إجمالي الكيانات: 42
### إجمالي الحقول: 400+

---

## 🏗️ المجموعات الرئيسية

### 1. إدارة النظام والمستخدمين (System & User Management)
#### الجداول:
- **User** - المستخدمون
  - الحقول: UserId, Username, Email, PasswordHash, FullName, PhoneNumber, GroupId, BranchId
  - العمليات: Create, Read, Update, Delete, Login, ChangePassword, ResetPassword

- **Group** - مجموعات المستخدمين
  - الحقول: GroupId, GroupCode, GroupName, Description
  - العمليات: Create, Read, Update, Delete

- **Privilege** - الصلاحيات
  - الحقول: PrivilegeId, Code, Name, Description, FormName, Category
  - العمليات: Assign, Revoke, List

- **GroupPrivilege** - ربط المجموعات بالصلاحيات
  - العمليات: Assign Privilege to Group, Remove Privilege

- **UserDevice** - أجهزة المستخدمين
  - الحقول: DeviceId, UserId, DeviceName, DeviceType, IPAddress, LastAccessDate
  - العمليات: Register Device, Track Usage

- **UserLog** - سجل نشاط المستخدمين
  - الحقول: LogId, UserId, ActionType, Description, TableName, LogDate
  - العمليات: Log Action, View Activity Report

- **Audit** - سجل التدقيق
  - الحقول: AuditId, UserId, EntityName, EntityId, Action, OldValues, NewValues, AuditDate
  - العمليات: Track Changes, Generate Audit Report

---

### 2. إعدادات النظام (System Configuration)
#### الجداول:
- **Branch** - الفروع
  - الحقول: BranchId, BranchCode, BranchName, Address, City, Country, Manager
  - العمليات: Create Branch, Update Branch, Assign Warehouses

- **Currency** - العملات
  - الحقول: CurrencyId, CurrencyCode, CurrencyName, Symbol, ExchangeRate
  - العمليات: Add Currency, Update Exchange Rate

- **CostCenter** - مراكز التكاليف
  - الحقول: CostCenterId, CostCenterCode, CostCenterName, Department, Manager, BranchId
  - العمليات: Create Cost Center, Allocate Costs, Generate Cost Report

- **SystemParameter** - معاملات النظام
  - الحقول: ParameterId, ParameterName, ParameterValue, Category, DataType
  - العمليات: Get Parameter, Set Parameter

---

### 3. المحاسبة (Accounting)
#### الجداول:
- **ChartOfAccount** - شجرة الحسابات
  - الحقول: AccountId, AccountNumber, AccountNameAr/En, AccountType, ParentAccountId, BranchId
  - الأنواع: Asset (الأصول), Liability (الالتزامات), Equity (حقوق المالكين), Income (الدخل), Expense (المصروفات)
  - العمليات: 
    - Create Account
    - Update Account
    - Delete Account
    - View Chart Structure
    - Generate Account List

- **JournalType** - أنواع اليوميات
  - الحقول: JournalTypeId, JournalTypeCode, JournalTypeNameAr/En
  - الأنواع: General Journal, Sales Journal, Purchase Journal, Payroll Journal
  - العمليات: Create Journal Type, Assign to Branches

- **Journal** - اليوميات
  - الحقول: JournalId, JournalNumber, JournalTypeId, JournalDate, PostingDate, FiscalPeriodId, JournalStatus
  - الحالات: Draft, Posted, Reversed
  - العمليات:
    - Create Journal Entry
    - Post Journal (ترحيل)
    - Reverse Journal (عكس)
    - View Journal
    - Generate Journal Report

- **JournalEntry** - قيود اليوميات
  - الحقول: JournalEntryId, JournalId, AccountId, DebitAmount, CreditAmount, Description
  - العمليات:
    - Add Entry Line
    - Remove Entry Line
    - Validate Debit/Credit Balance
    - Calculate Totals

- **FiscalPeriod** - الفترات المحاسبية
  - الحقول: FiscalPeriodId, PeriodCode, PeriodName, StartDate, EndDate, IsClosed
  - العمليات: Create Period, Close Period, Open Period

- **OpeningBalance** - الأرصدة الافتتاحية
  - الحقول: OpeningBalanceId, AccountId, FiscalPeriodId, OpeningAmount, CreatedDate
  - العمليات: Set Opening Balance, Adjust Opening Balance

- **AccountBalance** - أرصدة الحسابات
  - الحقول: AccountBalanceId, AccountId, FiscalPeriodId, DebitBalance, CreditBalance
  - العمليات: Calculate Balance, Generate Balance Sheet

- **LedgerReport** - تقارير الأستاذ
  - الحقول: LedgerReportId, AccountId, FiscalPeriodId, OpeningBalance, TotalDebits, TotalCredits, ClosingBalance
  - العمليات: Generate Ledger, Export Report

---

### 4. المخزون (Inventory)
#### الجداول:
- **ItemCategory** - فئات المواد
  - الحقول: ItemCategoryId, ItemCategoryNameAr/En, ItemCategoryDescription
  - العمليات:
    - Create Category
    - Update Category
    - Delete Category
    - List Categories

- **Item** - المواد/المنتجات
  - الحقول: ItemId, ItemCode, ItemNameAr/En, ItemCategoryId, ItemCost, ItemPrice, IsActive
  - العمليات:
    - Create Item
    - Update Item
    - Deactivate Item
    - View Item Details
    - Track Item History

- **ItemUnit** - وحدات المواد
  - الحقول: ItemUnitId, ItemId, UnitNameAr/En, UnitFactor, UnitPrice
  - الأمثلة: كيس, صندوق, قطعة, متر
  - العمليات:
    - Add Unit
    - Update Unit
    - Delete Unit
    - Convert Between Units

- **Warehouse** - المستودعات
  - الحقول: WarehouseId, WarehouseNameAr/En, BranchId, WarehouseCapacity
  - العمليات:
    - Create Warehouse
    - Update Capacity
    - View Warehouse Info
    - Monitor Stock Levels

- **ItemBalance** - رصيد المواد
  - الحقول: ItemBalanceId, ItemId, WarehouseId, BalanceQuantity, AverageCost
  - العمليات:
    - Update Balance
    - Get Available Quantity
    - Calculate Stock Value
    - Generate Stock Report

- **ItemMovement** - حركات المواد
  - الحقول: ItemMovementId, ItemId, WarehouseId, WarehouseIdTo, MovementType, MovementQuantity, MovementDate
  - أنواع الحركات: Inbound, Outbound, Transfer, Adjustment
  - العمليات:
    - Record Movement
    - Transfer Between Warehouses
    - Adjust Stock
    - View Movement History
    - Generate Movement Report

- **ItemBatch** - دفعات المواد
  - الحقول: ItemBatchId, ItemId, WarehouseId, BatchNumber, BatchQuantity, ExpiryDate
  - العمليات:
    - Create Batch
    - Track Batch
    - Monitor Expiry
    - Generate Batch Report

- **InventoryCount** - جرد المخزون
  - الحقول: InventoryCountId, CountNumber, WarehouseId, CountDate, CountStatus
  - العمليات:
    - Start Count
    - Record Count Details
    - Finalize Count
    - Generate Variance Report

- **InventoryCountDetail** - تفاصيل الجرد
  - الحقول: InventoryCountDetailId, InventoryCountId, ItemId, SystemQuantity, PhysicalQuantity, Difference
  - العمليات:
    - Add Count Line
    - Calculate Variance
    - Adjust Quantities

---

### 5. المبيعات (Sales)
#### الجداول:
- **CustomerType** - أنواع العملاء
  - الحقول: CustomerTypeId, CustomerTypeCode, CustomerTypeNameAr/En
  - الأنواع: Retail, Wholesale, Corporate

- **Customer** - العملاء
  - الحقول: CustomerId, CustomerCode, CustomerNameAr/En, TaxId, Address, City, Phone, Email
  - العمليات:
    - Create Customer
    - Update Customer Information
    - Activate/Deactivate
    - View Customer History

- **CustomerContact** - جهات اتصال العملاء
  - الحقول: ContactId, CustomerId, ContactName, ContactPhone, ContactEmail
  - العمليات: Add Contact, Update Contact, Delete Contact

- **SalesOrder** - أوامر البيع
  - الحقول: SalesOrderId, SalesOrderNumber, CustomerId, OrderDate, RequiredDate, SalesmanId, TotalAmount
  - الحالات: Draft, Confirmed, Shipped, Delivered, Cancelled
  - العمليات:
    - Create Sales Order
    - Confirm Order
    - Ship Order
    - Generate Picking List
    - Create Invoice

- **SalesOrderDetail** - تفاصيل أوامر البيع
  - الحقول: DetailId, SalesOrderId, ItemId, Quantity, UnitPrice, LineTotal
  - العمليات:
    - Add Line Item
    - Update Quantity/Price
    - Calculate Line Total
    - Check Stock Availability

- **SalesInvoice** - فواتير البيع
  - الحقول: SalesInvoiceId, SalesInvoiceNumber, CustomerId, InvoiceDate, DueDate, TotalAmount
  - الحالات: Draft, Issued, Paid, Cancelled
  - العمليات:
    - Create Invoice
    - Issue Invoice
    - Send Invoice
    - Record Payment
    - Generate Aging Report

- **SalesInvoiceDetail** - تفاصيل فواتير البيع
  - الحقول: DetailId, SalesInvoiceId, ItemId, Quantity, UnitPrice, LineTotal, TaxAmount
  - العمليات: Add Line, Update Line, Calculate Tax

- **SalesReturn** - مرتجعات البيع
  - الحقول: SalesReturnId, SalesReturnNumber, CustomerId, ReturnDate, TotalAmount
  - الحالات: Draft, Processed, Credited
  - العمليات:
    - Create Return
    - Process Return
    - Issue Credit Note
    - Update Stock

- **SalesReturnDetail** - تفاصيل مرتجعات البيع
  - الحقول: DetailId, SalesReturnId, ItemId, ReturnQuantity, UnitPrice

- **SalesPayment** - سدادات البيع
  - الحقول: SalesPaymentId, CustomerId, PaymentDate, PaymentAmount, PaymentMethod, ChequeNumber
  - طرق الدفع: Cash, Cheque, Transfer, Credit Card
  - العمليات:
    - Record Payment
    - Match Payment to Invoice
    - Generate Receipt
    - Reconcile Payments

- **Salesman** - رجال البيع
  - الحقول: SalesmanId, SalesmanCode, SalesmanNameAr/En, Phone, Email, BranchId, Commission
  - العمليات:
    - Create Salesman
    - Assign Territory
    - Track Sales
    - Calculate Commission

- **CustomerBalance** - رصيد العملاء
  - الحقول: CustomerBalanceId, CustomerId, FiscalPeriodId, DebitBalance, CreditBalance
  - العمليات:
    - Calculate Balance
    - Generate Aging Report
    - Track Payment Status

---

### 6. المشتريات (Purchase)
#### الجداول:
- **VendorType** - أنواع الموردين
  - الحقول: VendorTypeId, VendorTypeCode, VendorTypeNameAr/En

- **Vendor** - الموردون
  - الحقول: VendorId, VendorCode, VendorNameAr/En, TaxId, Address, City, Phone, Email
  - العمليات:
    - Create Vendor
    - Update Vendor Info
    - Activate/Deactivate
    - View Purchase History

- **VendorContact** - جهات اتصال الموردين
  - الحقول: ContactId, VendorId, ContactName, ContactPhone, ContactEmail

- **Quotation** - عروض الأسعار
  - الحقول: QuotationId, QuotationNumber, VendorId, QuotationDate, ValidUntil, TotalAmount
  - الحالات: Draft, Sent, Accepted, Rejected, Expired
  - العمليات:
    - Request Quotation
    - Receive Quotation
    - Compare Quotations
    - Accept Quotation

- **QuotationDetail** - تفاصيل عروض الأسعار
  - الحقول: DetailId, QuotationId, ItemId, Quantity, UnitPrice, LineTotal

- **PurchaseOrder** - أوامر الشراء
  - الحقول: PurchaseOrderId, PurchaseOrderNumber, VendorId, OrderDate, RequiredDate, TotalAmount
  - الحالات: Draft, Confirmed, Received, Cancelled
  - العمليات:
    - Create Purchase Order
    - Confirm Order
    - Receive Goods
    - Create Invoice
    - Track Delivery

- **PurchaseOrderDetail** - تفاصيل أوامر الشراء
  - الحقول: DetailId, PurchaseOrderId, ItemId, Quantity, UnitPrice, LineTotal

- **PurchaseInvoice** - فواتير الشراء
  - الحقول: PurchaseInvoiceId, PurchaseInvoiceNumber, VendorId, InvoiceDate, DueDate, TotalAmount
  - الحالات: Draft, Received, Paid, Cancelled
  - العمليات:
    - Create Invoice
    - Match to PO
    - Record Payment
    - Generate Aging Report

- **PurchaseInvoiceDetail** - تفاصيل فواتير الشراء
  - الحقول: DetailId, PurchaseInvoiceId, ItemId, Quantity, UnitPrice, LineTotal

- **PurchaseReturn** - مرتجعات الشراء
  - الحقول: PurchaseReturnId, PurchaseReturnNumber, VendorId, ReturnDate, TotalAmount
  - العمليات:
    - Create Return
    - Process Return
    - Issue Debit Note
    - Update Stock

- **PurchaseReturnDetail** - تفاصيل مرتجعات الشراء
  - الحقول: DetailId, PurchaseReturnId, ItemId, ReturnQuantity, UnitPrice

- **PurchasePayment** - سدادات الشراء
  - الحقول: PurchasePaymentId, VendorId, PaymentDate, PaymentAmount, PaymentMethod
  - العمليات:
    - Record Payment
    - Match to Invoice
    - Generate Payment Slip
    - Reconcile Payments

- **VendorBalance** - رصيد الموردين
  - الحقول: VendorBalanceId, VendorId, FiscalPeriodId, DebitBalance, CreditBalance
  - العمليات:
    - Calculate Balance
    - Generate Aging Report
    - Track Payment Status

---

## 🔄 العمليات الرئيسية والتطبيقات المطلوبة

### 1. لوحة التحكم (Dashboard)
- إجمالي المبيعات
- إجمالي المشتريات
- رصيد المخزون
- المستخدمون النشطون
- آخر العمليات

### 2. إدارة المحاسبة (Accounting Management)
- شجرة الحسابات - إنشاء، تحديث، حذف
- اليوميات - إنشاء، ترحيل، عكس
- الأرصدة - حساب، تحديث
- التقارير - الأستاذ، الميزانية العمومية

### 3. إدارة المخزون (Inventory Management)
- المواد - إضافة، تحديث، حذف
- المستودعات - إدارة المستويات
- الحركات - تسجيل، تحويل
- الجرد - عد الأصناف، تصحيح الفروقات

### 4. إدارة المبيعات (Sales Management)
- العملاء - قاعدة بيانات العملاء
- أوامر البيع - إنشاء، تأكيد
- الفواتير - إنشاء، إصدار
- السدادات - تسجيل، مطابقة

### 5. إدارة المشتريات (Purchase Management)
- الموردون - قاعدة بيانات الموردين
- أوامر الشراء - إنشاء، تأكيد
- الفواتير - استقبال، دفع
- المرتجعات - معالجة المرتجعات

### 6. إدارة النظام (System Administration)
- المستخدمون - إنشاء، تعديل الصلاحيات
- المجموعات - تعريف المجموعات والصلاحيات
- التدقيق - عرض سجلات التدقيق
- الإعدادات - معاملات النظام

### 7. التقارير (Reports)
- تقارير المحاسبة - الأستاذ، الميزانية العمومية، قائمة الدخل
- تقارير المبيعات - مبيعات حسب العميل، حسب المنتج
- تقارير المشتريات - مشتريات حسب المورد
- تقارير المخزون - أرصدة المخزون، الحركات

---

## 📋 خريطة العمليات والواجهات المطلوبة

| الوحدة | العمليات | الواجهات المطلوبة |
|--------|---------|-------------------|
| **المحاسبة** | إنشاء الحسابات، الترحيل | Chart Management, Journal Entry, Reports |
| **المخزون** | إضافة مواد، تسجيل حركات | Item Management, Warehouse Dashboard, Stock Movements |
| **المبيعات** | أوامر، فواتير، سدادات | Customer Management, Sales Orders, Invoicing |
| **المشتريات** | أوامر، فواتير، ورائق | Vendor Management, Purchase Orders, Receiving |
| **النظام** | مستخدمون، صلاحيات | User Management, System Settings, Audit Trail |

