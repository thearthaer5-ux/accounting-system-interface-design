# المرحلة الثالثة: إدارة الحسابات المحاسبية
## Accounting Management System

---

## مقدمة عن النظام

نظام الحسابات المحاسبية المتكامل يوفر جميع الأدوات اللازمة لإدارة العمليات المحاسبية الكاملة من إنشاء شجرة الحسابات إلى توليد التقارير المالية الشاملة.

---

## الكيانات الرئيسية (8 Entities)

### 1. ChartOfAccount (شجرة الحسابات)
```csharp
- AccountId: معرّف فريد للحساب
- AccountNumber: رقم الحساب (مثل 1000، 1100، 1110)
- AccountNameAr/En: أسماء الحساب
- AccountType: نوع الحساب (Asset, Liability, Equity, Income, Expense)
- AccountLevel: مستوى الحساب (Header, Detail)
- ParentAccountId: علاقة هرمية مع الحساب الأب
- IsActive: حالة الحساب
- OpeningBalance: الرصيد الافتتاحي
```

**الاستخدام:**
- إنشاء هيكل الحسابات الهرمي
- تحديد نوع كل حساب
- تعيين الأرصدة الافتتاحية

### 2. Journal (اليوميات المحاسبية)
```csharp
- JournalId: معرّف اليومية
- JournalNumber: رقم اليومية الفريد
- JournalTypeId: نوع اليومية (عامة، شراء، مبيعات)
- JournalDate: تاريخ اليومية
- PostingDate: تاريخ الترحيل
- JournalStatus: الحالة (Draft, Posted, Reversed)
- TotalDebit/Credit: إجمالي المدين والدائن
```

**الحالات:**
- **Draft**: مسودة - قيد الإنشاء والتحديل
- **Posted**: مرحل - تم تطبيقه على الحسابات
- **Reversed**: معكوس - تم عكسه

### 3. JournalEntry (سند القيد)
```csharp
- JournalEntryId: معرّف السند
- VoucherNumber: رقم السند
- JournalId: معرّف اليومية
- AccountId: معرّف الحساب
- CostCenterId: مركز التكلفة (اختياري)
- DebitAmount/CreditAmount: المبالغ
- Description: الوصف
```

**الخصائص:**
- كل قيد يجب أن يرتبط بحساب
- يمكن ربط مركز تكلفة اختياري
- كل قيد يحتوي على مدين أو دائن أو كليهما

### 4. FiscalPeriod (الفترات المحاسبية)
```csharp
- FiscalPeriodId: معرّف الفترة
- PeriodName: اسم الفترة (مثل يناير 2024)
- FiscalYear: السنة المالية
- PeriodNumber: رقم الفترة (1-12)
- StartDate/EndDate: فترة التاريخ
- PeriodStatus: الحالة (Open, Closed, Locked)
```

**الإدارة:**
- فترة مالية واحدة مفتوحة في كل وقت
- إغلاق الفترة يمنع تعديل القيود

### 5. OpeningBalance (الأرصدة الافتتاحية)
```csharp
- OpeningBalanceId: معرّف الرصيد
- AccountId: معرّف الحساب
- FiscalPeriodId: معرّف الفترة
- DebitBalance/CreditBalance: الأرصدة
- Status: الحالة (Draft, Posted)
```

**الغرض:**
- تسجيل الأرصدة الافتتاحية لكل حساب
- يجب ترحيل الأرصدة قبل البدء بالعمليات

### 6. CostCenter (مراكز التكاليف)
```csharp
- CostCenterId: معرّف المركز
- CostCenterCode: كود المركز
- CostCenterNameAr/En: أسماء المركز
- BranchId: الفرع المرتبط
- IsActive: حالة المركز
```

**الاستخدام:**
- توزيع التكاليف على أقسام
- مكافحة التكاليف حسب الفرع

### 7. AccountBalance (أرصدة الحسابات)
```csharp
- AccountBalanceId: معرّف الرصيد
- AccountId: معرّف الحساب
- FiscalPeriodId: معرّف الفترة
- DebitBalance/CreditBalance: الأرصدة
- LastUpdated: تاريخ آخر تحديث
```

**التحديث التلقائي:**
- يتم تحديثها عند ترحيل القيود
- تحسب الرصيد الحالي لكل حساب

### 8. LedgerReport (تقارير الأستاذ)
```csharp
- LedgerReportId: معرّف التقرير
- AccountId: معرّف الحساب
- TransactionDate: تاريخ العملية
- VoucherNumber: رقم السند
- DebitAmount/CreditAmount: المبالغ
- RunningBalance: الرصيد المتراكم
```

---

## الـ Repositories (10 Repositories)

### 1. IChartOfAccountRepository
```csharp
GetByAccountNumberAsync(string accountNumber)
GetHierarchyAsync(int? parentId)
SearchAccountsAsync(string searchTerm)
GetAccountsByTypeAsync(string accountType)
HasSubAccountsAsync(int accountId)
```

### 2. IJournalRepository
```csharp
GetByNumberAsync(string journalNumber)
GetByPeriodAsync(int fiscalPeriodId)
GetByStatusAsync(string status)
GetByDateRangeAsync(DateTime startDate, DateTime endDate)
GetTotalDebitAsync(int journalId)
GetTotalCreditAsync(int journalId)
```

### 3. IJournalEntryRepository
```csharp
GetByJournalAsync(int journalId)
GetByAccountAsync(int accountId)
GetByDateRangeAsync(DateTime startDate, DateTime endDate)
GetAccountBalanceAsync(int accountId, DateTime? asOfDate)
```

### 4. IOpeningBalanceRepository
```csharp
GetByAccountAndPeriodAsync(int accountId, int fiscalPeriodId)
GetByPeriodAsync(int fiscalPeriodId)
GetDraftBalancesAsync(int fiscalPeriodId)
```

### 5. IFiscalPeriodRepository
```csharp
GetCurrentPeriodAsync()
GetByYearAndNumberAsync(int year, int periodNumber)
GetByYearAsync(int year)
GetPeriodByDateAsync(DateTime date)
```

### 6. IAccountBalanceRepository
```csharp
GetByAccountAndPeriodAsync(int accountId, int? fiscalPeriodId)
GetByPeriodAsync(int? fiscalPeriodId)
GetNetBalanceAsync(int accountId, int? fiscalPeriodId)
```

### 7. ILedgerReportRepository
```csharp
GetAccountLedgerAsync(int accountId, DateTime? startDate, DateTime? endDate)
GetByPeriodAsync(int fiscalPeriodId)
```

### 8. IJournalTypeRepository
```csharp
GetByCodeAsync(string code)
```

---

## الـ Services (5 Services)

### 1. IChartOfAccountService
```csharp
// العمليات الأساسية
GetByIdAsync(int id)
GetByAccountNumberAsync(string accountNumber)
GetAllAsync()
GetHierarchyAsync(int? parentId)
GetByTypeAsync(string accountType)

// إنشاء وتحديث
CreateAsync(ChartOfAccountCreateUpdateDto dto)
UpdateAsync(int id, ChartOfAccountCreateUpdateDto dto)
DeleteAsync(int id)

// التحقق
ValidateAccountAsync(int accountId)
```

**مثال الاستخدام:**
```csharp
var chart = await _chartService.GetHierarchyAsync();
// النتيجة: قائمة هرمية بجميع الحسابات
```

### 2. IJournalService
```csharp
GetByIdAsync(int id)
GetByNumberAsync(string journalNumber)
GetByPeriodAsync(int fiscalPeriodId)
GetByStatusAsync(string status)

CreateAsync(JournalCreateUpdateDto dto, int userId)
PostJournalAsync(int journalId, int userId)
ReverseJournalAsync(int journalId, int userId)
```

**عملية الترحيل:**
1. إنشاء يومية بحالة Draft
2. إضافة القيود (يجب أن تكون متوازنة)
3. ترحيل اليومية (تحويل الحالة إلى Posted)

### 3. IFiscalPeriodService
```csharp
GetCurrentPeriodAsync()
GetByIdAsync(int id)
GetByYearAsync(int year)

CreateAsync(FiscalPeriodCreateUpdateDto dto)
ClosePeriodAsync(int periodId)
OpenPeriodAsync(int periodId)
```

### 4. IAccountBalanceService
```csharp
GetAccountBalanceAsync(int accountId, int? fiscalPeriodId)
GenerateTrialBalanceAsync(int fiscalPeriodId)
```

**ميزان المراجعة:**
- تقرير يجمع أرصدة جميع الحسابات
- يجب أن يكون المجموع المدين = المجموع الدائن

### 5. IOpeningBalanceService
```csharp
GetByPeriodAsync(int fiscalPeriodId)
CreateAsync(OpeningBalanceCreateUpdateDto dto)
PostOpeningBalancesAsync(int fiscalPeriodId, int userId)
```

---

## الـ DTOs

### ChartOfAccountDto
```csharp
int AccountId
string AccountNumber
string AccountNameAr / AccountNameEn
string AccountType
string AccountLevel
bool IsActive
decimal OpeningBalance
```

### JournalDto
```csharp
int JournalId
string JournalNumber
int JournalTypeId
string JournalTypeName
DateTime JournalDate
string JournalStatus
decimal TotalDebit / TotalCredit
int EntryCount
```

### TrialBalanceReportDto
```csharp
int FiscalPeriodId
string PeriodName
DateTime GeneratedDate
List<TrialBalanceDto> Balances
decimal TotalDebit / TotalCredit
bool IsBalanced
```

### TrialBalanceDto
```csharp
int AccountId
string AccountNumber / AccountName
string AccountType
decimal DebitAmount / CreditAmount
```

---

## المسارات الأساسية

### إنشاء شجرة الحسابات
```
1. إنشاء حسابات رئيسية (Assets, Liabilities, etc)
2. إنشاء حسابات فرعية تحت كل رئيسي
3. تحديد نوع كل حساب
4. تعيين الأرصدة الافتتاحية
```

### عملية الترحيل المحاسبي
```
1. إنشاء يومية جديدة (حالة Draft)
2. إضافة قيود (مدين/دائن)
3. التحقق من التوازن (مدين = دائن)
4. ترحيل اليومية (حالة Posted)
5. تحديث أرصدة الحسابات
```

### توليد ميزان المراجعة
```
1. تحديد الفترة المالية
2. جمع أرصدة جميع الحسابات
3. التحقق من التوازن
4. طباعة التقرير
```

---

## API Endpoints

### Chart of Accounts
```
GET    /api/accounting/accounts
GET    /api/accounting/accounts/{id}
GET    /api/accounting/accounts/number/{accountNumber}
GET    /api/accounting/accounts/type/{type}
GET    /api/accounting/accounts/hierarchy
POST   /api/accounting/accounts
PUT    /api/accounting/accounts/{id}
```

### Journals
```
GET    /api/accounting/journals
GET    /api/accounting/journals/{id}
GET    /api/accounting/journals/by-period/{periodId}
POST   /api/accounting/journals
POST   /api/accounting/journals/{id}/post
POST   /api/accounting/journals/{id}/reverse
```

### Reports
```
GET    /api/accounting/reports/trial-balance
```

### Fiscal Periods
```
GET    /api/accounting/periods/current
GET    /api/accounting/periods/by-year/{year}
POST   /api/accounting/periods
POST   /api/accounting/periods/{id}/close
```

---

## معايير الجودة

- تم تطبيق Repository Pattern بالكامل
- جميع العمليات محمية بـ Authorization
- تسجيل شامل للعمليات (Logging)
- معالجة كاملة للأخطاء
- تحقق من التوازن المحاسبي
- دعم الفترات المالية المتعددة

---

## أمثلة عملية

### إنشاء حساب جديد
```csharp
var dto = new ChartOfAccountCreateUpdateDto
{
    AccountNumber = "1000",
    AccountNameAr = "الأصول",
    AccountType = "Asset",
    AccountLevel = "Header"
};

var accountId = await _chartService.CreateAsync(dto);
```

### إنشاء يومية مع قيود
```csharp
var dto = new JournalCreateUpdateDto
{
    JournalNumber = "J001",
    JournalTypeId = 1,
    Entries = new List<JournalEntryCreateDto>
    {
        new() { AccountId = 1, DebitAmount = 1000 },
        new() { AccountId = 2, CreditAmount = 1000 }
    }
};

var journalId = await _journalService.CreateAsync(dto, userId);
await _journalService.PostJournalAsync(journalId, userId);
```

---

## الملفات المضافة

- `ChartOfAccount.cs`, `Journal.cs`, `JournalEntry.cs` - الكيانات
- `AccountingRepositories.cs` - 7 Repositories
- `AccountingServices.cs` - 5 Services متقدمة
- `AccountingDtos.cs` - 15+ DTOs
- `AccountingController.cs` - MVC Controller
- `AccountingApiController.cs` - REST API Controller
- Views متعددة للتقارير والعمليات

---

جميع الملفات موثقة بالكامل وجاهزة للاستخدام الفوري.
