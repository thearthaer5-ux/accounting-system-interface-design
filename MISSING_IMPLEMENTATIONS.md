# الميزات والعمليات الناقصة في النظام

## جدول الملخص

| الميزة | المرحلة | الأولوية | الحالة |
|-------|--------|---------|-------|
| Unit Tests | جميع المراحل | عالية جداً | ❌ لم تطبق |
| Advanced Reports | الحسابات | عالية | ⚠️ جزئية |
| Caching Strategy | عام | متوسطة | ❌ لم تطبق |
| API Documentation | عام | متوسطة | ⚠️ بسيطة |
| Multi-Currency Complete | الحسابات | عالية | ⚠️ جزئية |
| Closing Entries | الحسابات | عالية جداً | ❌ لم تطبق |
| Purchase Module | المرحلة 4 | عالية جداً | ⏳ مخطط |
| Sales Module | المرحلة 5 | عالية جداً | ⏳ مخطط |

---

## 1. العمليات المحاسبية الناقصة

### 1.1 الإغلاق المحاسبي (Period Closing)

**الوصف:** إغلاق الفترة المالية ونقل الأرصدة للفترة الجديدة

**الجداول المطلوبة:**
```
- ClosingEntries (قيود الإغلاق)
- ClosingProcess (سجل عملية الإغلاق)
- ClosingJournal (اليومية المؤقتة)
```

**الخدمات المطلوبة:**
```csharp
public interface IClosingService
{
    Task<bool> ValidateClosingEligibility(int fiscalPeriodId);
    Task<ClosingResult> ExecuteClosing(int fiscalPeriodId, int userId);
    Task<bool> ReverseClosing(int fiscalPeriodId);
    Task<ClosingReport> GetClosingReport(int fiscalPeriodId);
}
```

### 1.2 نقل الأرصدة (Carryforward)

**الوصف:** نقل الأرصدة من الفترة السابقة كأرصدة افتتاحية

**العمليات المطلوبة:**
- حساب الأرصدة الافتتاحية من حسابات الفترة السابقة
- إنشاء قيود افتتاحية تلقائية
- التحقق من التوازن

### 1.3 حسابات النتائج المتراكمة (Retained Earnings)

**الوصف:** حساب الأرباح المتراكمة والخسائر

**الجداول المطلوبة:**
```
- RetainedEarnings (الأرباح المتراكمة)
```

---

## 2. عمليات المخزون الناقصة

### 2.1 تحذيرات الصلاحية

**الخدمات المطلوبة:**
```csharp
public interface IExpirityAlertService
{
    Task<List<ExpiringBatch>> GetExpiringItems(int daysThreshold);
    Task<List<ExpiredBatch>> GetExpiredItems();
    Task<bool> BlockExpiredItems();
    Task SendNotifications(List<ExpiringBatch> items);
}
```

### 2.2 إعادة الترتيب التلقائية (Reorder Point)

**الجداول المطلوبة:**
```
- ItemReorderPoint (نقاط إعادة الترتيب)
- AutoPurchaseOrder (أوامر الشراء التلقائية)
```

### 2.3 تتبع الدفعات المتقدم (Batch Traceability)

**الخدمات المطلوبة:**
```csharp
public interface IBatchTraceService
{
    Task<BatchTrace> TraceForward(int batchId); // تتبع الدفعة للأمام
    Task<BatchTrace> TraceBackward(int batchId); // تتبع الدفعة للخلف
    Task<List<BatchIssue>> IdentifyIssues(int batchId);
}
```

### 2.4 تحليل الأداء (Performance Analysis)

**الخدمات المطلوبة:**
```csharp
public interface IInventoryAnalysisService
{
    Task<SlowMovingItems> GetSlowMovingItems();
    Task<FastMovingItems> GetFastMovingItems();
    Task<InventoryTurnover> CalculateTurnover(int itemId, int fiscalPeriodId);
    Task<StockoutAnalysis> AnalyzeStockouts();
}
```

---

## 3. الميزات الأمنية الناقصة

### 3.1 Rate Limiting (تحديد معدل الطلب)

**المطلوب:**
```csharp
// Middleware للـ Rate Limiting
services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("default", opt =>
    {
        opt.Window = TimeSpan.FromSeconds(60);
        opt.PermitLimit = 100;
    });
});
```

### 3.2 API Key Authentication

**المطلوب:**
```csharp
public class ApiKey
{
    public int ApiKeyId { get; set; }
    public string Key { get; set; }
    public int UserId { get; set; }
    public DateTime ExpiryDate { get; set; }
    public bool IsActive { get; set; }
}
```

### 3.3 Two-Factor Authentication

**المطلوب:**
```csharp
public class UserTwoFactor
{
    public int UserId { get; set; }
    public string Method { get; set; } // SMS, Email, Authenticator
    public bool IsEnabled { get; set; }
    public string Secret { get; set; }
}
```

### 3.4 Session Management

**المطلوب:**
```csharp
public class UserSession
{
    public int SessionId { get; set; }
    public int UserId { get; set; }
    public string SessionToken { get; set; }
    public string IpAddress { get; set; }
    public string UserAgent { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime LastActivityOn { get; set; }
    public bool IsActive { get; set; }
}
```

---

## 4. التقارير المالية الناقصة

### 4.1 الميزانية العمومية (Balance Sheet)

**المطلوب:**
```csharp
public class BalanceSheetReport
{
    public decimal Assets { get; set; }
    public decimal Liabilities { get; set; }
    public decimal Equity { get; set; }
    
    // يجب أن يكون: Assets = Liabilities + Equity
}
```

### 4.2 قائمة الدخل (Income Statement)

**المطلوب:**
```csharp
public class IncomeStatementReport
{
    public decimal TotalRevenue { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal NetIncome { get; set; }
    // NetIncome = TotalRevenue - TotalExpenses
}
```

### 4.3 تقرير التدفقات النقدية (Cash Flow)

**المطلوب:**
```csharp
public class CashFlowReport
{
    public decimal OperatingCashFlow { get; set; }
    public decimal InvestingCashFlow { get; set; }
    public decimal FinancingCashFlow { get; set; }
    public decimal NetCashFlow { get; set; }
}
```

### 4.4 تقرير الديون المستحقة (Aging Report)

**المطلوب:**
```csharp
public class AgingReport
{
    public decimal Current { get; set; }      // 0-30 أيام
    public decimal Days30_60 { get; set; }    // 30-60 أيام
    public decimal Days60_90 { get; set; }    // 60-90 أيام
    public decimal Days90Plus { get; set; }   // أكثر من 90 يوم
}
```

---

## 5. الحقول المفقودة في الكيانات الموجودة

### 5.1 في User Entity
```csharp
// مفقود:
public string MiddleName { get; set; }
public string LastName { get; set; }
public string PhoneNumber { get; set; }
public string Email { get; set; }
public string Photo { get; set; }
public bool IsActive { get; set; }
public DateTime? LastLoginDate { get; set; }
public DateTime? PasswordChangeDate { get; set; }
public int FailedLoginAttempts { get; set; }
public bool IsLockedOut { get; set; }
```

### 5.2 في Item Entity
```csharp
// مفقود:
public string Barcode { get; set; }
public string SKU { get; set; }
public decimal MinimumStock { get; set; }
public decimal MaximumStock { get; set; }
public string Description { get; set; }
public bool IsActive { get; set; }
public bool TrackSerialNumber { get; set; }
public bool TrackBatch { get; set; }
public int LeadTimeDays { get; set; }
```

### 5.3 في JournalEntry Entity
```csharp
// مفقود:
public string Reference { get; set; }
public string Description { get; set; }
public int? ReferenceDocumentId { get; set; }
public string ReferenceDocumentType { get; set; }
public bool IsCancelled { get; set; }
public int? CancelledByUserId { get; set; }
public DateTime? CancelledDate { get; set; }
```

---

## 6. العلاقات الناقصة

### 6.1 بين ItemMovement و JournalEntry
```
يجب أن تكون العلاقة واحد لواحد:
- كل حركة مخزون يجب أن تنتج عن قيد محاسبي
- كل قيد محاسبي متعلق بالمخزون يجب أن ينتج حركة مخزون
```

### 6.2 بين Purchase و Accounting
```
يجب أن تكون العلاقة:
PurchaseInvoice → ItemMovement → JournalEntry → AccountBalance
```

### 6.3 بين Sales و Accounting
```
يجب أن تكون العلاقة:
SalesInvoice → ItemMovement → JournalEntry → AccountBalance
```

---

## 7. العمليات الحسابية الناقصة

### 7.1 حساب الضرائب المتقدم

**المطلوب:**
```csharp
public class TaxCalculationService
{
    // ضريبة المبيعات
    public decimal CalculateSalesTax(decimal amount, string taxType);
    
    // ضريبة الشراء (يمكن استردادها)
    public decimal CalculatePurchaseTax(decimal amount, string taxType);
    
    // صافي الضريبة
    public decimal CalculateNetTax(decimal salesTax, decimal purchaseTax);
}
```

### 7.2 حساب الخصومات المعقدة

**المطلوب:**
```csharp
public class DiscountCalculationService
{
    // خصم بناءً على الكمية
    public decimal CalculateQuantityDiscount(int quantity);
    
    // خصم بناءً على العميل
    public decimal CalculateCustomerDiscount(int customerId);
    
    // خصم بناءً على فترة زمنية
    public decimal CalculateSeasonalDiscount(DateTime date);
    
    // خصوم متعددة مركبة
    public decimal CalculateCombinedDiscount(params discount[] discounts);
}
```

### 7.3 حساب التكاليف غير المباشرة

**المطلوب:**
```csharp
public class IndirectCostService
{
    // توزيع المصروفات العامة
    public decimal AllocateOverhead(int itemId, decimal quantity);
    
    // حساب سعر التكلفة الكامل
    public decimal CalculateFullCost(int itemId);
}
```

---

## 8. ميزات التقديم والدفع الناقصة

### 8.1 استحقاق الديون

**المطلوب:**
```csharp
public class ReceivableService
{
    Task<decimal> CalculateAccruedAmount(int customerId, DateTime asOfDate);
    Task<List<InvoiceAging>> GetAging(int branchId);
    Task<decimal> GetBadDebtProvision(int branchId);
}
```

### 8.2 التزامات الدفع

**المطلوب:**
```csharp
public class PayableService
{
    Task<decimal> CalculateAccruedPayable(int vendorId, DateTime asOfDate);
    Task<List<PaymentSchedule>> GetPaymentSchedules(int vendorId);
    Task<bool> IsPaymentDue(int invoiceId);
}
```

---

## 9. ميزات Dashboard و Analytics

### 9.1 لوحة التحكم

**المطلوب:**
```csharp
public class DashboardService
{
    Task<FinancialSummary> GetFinancialSummary(DateTime asOfDate);
    Task<InventorySummary> GetInventorySummary(int branchId);
    Task<SalesSummary> GetSalesSummary(DateRange dateRange);
    Task<TopItems> GetTopSellingItems(int count);
    Task<CashPosition> GetCashPosition();
}
```

### 9.2 تحليل الأداء

**المطلوب:**
```csharp
public class PerformanceAnalyticsService
{
    Task<SalesGrowth> CalculateSalesGrowth(DateRange dateRange);
    Task<ProfitMargin> CalculateProfitMargin(int productId);
    Task<CustomerSatisfaction> GetCustomerSatisfactionMetrics();
}
```

---

## 10. الاختبارات المطلوبة

### 10.1 Unit Tests
```
عدد المتوقع: 200+ tests

الفئات:
- Services Tests: 60+
- Repository Tests: 40+
- Entity Validation Tests: 30+
- Business Logic Tests: 40+
- Calculation Tests: 30+
```

### 10.2 Integration Tests
```
عدد المتوقع: 100+ tests

الفئات:
- Database Integration: 30+
- API Integration: 40+
- Cross-Module Integration: 30+
```

---

## تقدير الجهد

| الميزة | الجهد (ساعات) | الأولوية |
|-------|----------|--------|
| Unit Tests | 60 | عالية جداً |
| Purchase Module | 80 | عالية جداً |
| Sales Module | 90 | عالية جداً |
| Advanced Reports | 40 | عالية |
| Closing Entries | 30 | عالية |
| API Documentation | 20 | متوسطة |
| Performance Optimization | 30 | متوسطة |
| Caching Strategy | 25 | متوسطة |

**المجموع الإجمالي:** ~375 ساعة عمل

---

## الخطوات التالية الموصى بها

1. **الفورية (بعد مراجعة هذا التقرير):**
   - البدء في المرحلة الرابعة (المشتريات)
   - البدء في إضافة Unit Tests بالتوازي

2. **قصيرة المدى:**
   - إكمال المرحلة الخامسة (المبيعات)
   - إضافة التقارير المالية المتقدمة

3. **متوسطة المدى:**
   - تطبيق Caching Strategy
   - Dashboard متقدمة
   - API Documentation

---

**تم إعداد هذا التقرير بناءً على تحليل شامل للمشروع الحالي.**
