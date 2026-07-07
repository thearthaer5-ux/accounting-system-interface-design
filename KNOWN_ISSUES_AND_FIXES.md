# المشاكل المعروفة والحلول الموصى بها

## تصنيف المشاكل

| الشدة | الأولوية | الحالة | العدد |
|------|---------|-------|-------|
| **حرجة** | عالية جداً | يجب إصلاح فوراً | 3 |
| **عالية** | عالية | يجب إصلاح قريباً | 5 |
| **متوسطة** | متوسطة | يمكن الانتظار | 7 |
| **منخفضة** | منخفضة | تحسينات مستقبلية | 4 |

---

## 🔴 المشاكل الحرجة (Critical)

### المشكلة 1: تحديث الأرصدة المحاسبية ليس فوري

**الشدة:** 🔴 حرجة
**التأثير:** قد تظهر أرصدة غير صحيحة للمستخدمين
**السبب الجذري:** الأرصدة تحسب مباشرة من JournalEntry عند كل استعلام

**الحل الموصى به:**
```csharp
// الحل 1: استخدام Materialized View في SQL
CREATE MATERIALIZED VIEW vw_CurrentAccountBalance AS
SELECT 
    AccountId,
    FiscalPeriodId,
    SUM(CASE WHEN DebitAmount > 0 THEN DebitAmount ELSE 0 END) as DebitBalance,
    SUM(CASE WHEN CreditAmount > 0 THEN CreditAmount ELSE 0 END) as CreditBalance,
    GETDATE() as LastUpdated
FROM JournalEntry
WHERE Status = 'Posted'
GROUP BY AccountId, FiscalPeriodId;

// الحل 2: تحديث AccountBalance عند ترحيل القيد
public async Task PostJournal(int journalId)
{
    var journal = await _journalRepository.GetByIdAsync(journalId);
    foreach (var entry in journal.JournalEntries)
    {
        var balance = await _accountBalanceRepository
            .GetByAccountAndPeriod(entry.AccountId, journal.FiscalPeriodId);
        
        balance.DebitBalance += entry.DebitAmount;
        balance.CreditBalance += entry.CreditAmount;
        
        await _accountBalanceRepository.UpdateAsync(balance);
    }
}
```

**مستوى الصعوبة:** عالية
**الوقت المتوقع:** 4 ساعات

---

### المشكلة 2: عدم وجود آلية لعكس القيود المحاسبية

**الشدة:** 🔴 حرجة
**التأثير:** لا يمكن تصحيح الأخطاء المحاسبية
**السبب الجذري:** لا توجد خاصية Reversal في JournalEntry

**الحل الموصى به:**
```csharp
public class JournalEntry
{
    // الخصائص الموجودة...
    
    // الخصائص المضافة:
    public int? ReversalOfEntryId { get; set; }
    public JournalEntry? ReversalOfEntry { get; set; }
    public List<JournalEntry> ReversingEntries { get; set; } = new();
    public string? ReversalReason { get; set; }
    public DateTime? ReversalDate { get; set; }
    public int? ReversedByUserId { get; set; }
}

// Service
public async Task<JournalEntry> ReverseEntry(int entryId, string reason, int userId)
{
    var originalEntry = await _entryRepository.GetByIdAsync(entryId);
    
    var reversingEntry = new JournalEntry
    {
        JournalId = originalEntry.JournalId,
        AccountId = originalEntry.AccountId,
        DebitAmount = originalEntry.CreditAmount,  // معكوس
        CreditAmount = originalEntry.DebitAmount,  // معكوس
        Description = $"Reversal of: {originalEntry.Description}",
        ReversalOfEntryId = entryId,
        ReversalReason = reason,
        ReversalDate = DateTime.Now,
        ReversedByUserId = userId
    };
    
    await _entryRepository.AddAsync(reversingEntry);
    await _entryRepository.SaveChangesAsync();
    
    // تحديث الأرصدة
    await UpdateAccountBalances(reversingEntry);
    
    return reversingEntry;
}
```

**مستوى الصعوبة:** عالية
**الوقت المتوقع:** 3 ساعات

---

### المشكلة 3: نقص في معالجة العملات المتعددة

**الشدة:** 🔴 حرجة (إذا كان التطبيق يدعم عملات متعددة)
**التأثير:** حسابات خاطئة عند استخدام عملات مختلفة
**السبب الجذري:** ChartOfAccount لا يدعم العملات المتعددة

**الحل الموصى به:**
```csharp
public class AccountCurrency
{
    public int AccountCurrencyId { get; set; }
    public int AccountId { get; set; }
    public ChartOfAccount Account { get; set; }
    
    public string CurrencyCode { get; set; }
    public Currency Currency { get; set; }
    
    public decimal DebitBalance { get; set; }
    public decimal CreditBalance { get; set; }
    public decimal RunningBalance { get; set; }
    
    public DateTime CreatedOn { get; set; }
    public DateTime? ModifiedOn { get; set; }
}

// تحديث DbContext
public DbSet<AccountCurrency> AccountCurrencies { get; set; }

// في JournalEntry
public decimal ExchangeRate { get; set; }  // سعر الصرف
public string OriginalCurrency { get; set; }  // العملة الأصلية
public decimal OriginalAmount { get; set; }  // المبلغ الأصلي
```

**مستوى الصعوبة:** عالية جداً
**الوقت المتوقع:** 5 ساعات

---

## 🟠 المشاكل العالية (High Priority)

### المشكلة 4: عدم وجود Soft Delete

**الشدة:** 🟠 عالية
**التأثير:** لا يمكن استرجاع البيانات المحذوفة، مشاكل في التدقيق
**السبب الجذري:** الحذف المباشر بدلاً من Soft Delete

**الحل الموصى به:**
```csharp
public abstract class AuditableEntity
{
    public int CreatedByUserId { get; set; }
    public DateTime CreatedOn { get; set; }
    public int? ModifiedByUserId { get; set; }
    public DateTime? ModifiedOn { get; set; }
    
    // Soft Delete
    public bool IsDeleted { get; set; }
    public int? DeletedByUserId { get; set; }
    public DateTime? DeletedOn { get; set; }
}

// في DbContext
public override int SaveChanges()
{
    var entries = ChangeTracker.Entries()
        .Where(e => e.State == EntityState.Deleted);
    
    foreach (var entry in entries)
    {
        if (entry.Entity is AuditableEntity auditableEntity)
        {
            entry.State = EntityState.Modified;
            auditableEntity.IsDeleted = true;
            auditableEntity.DeletedOn = DateTime.UtcNow;
            // وضع معرّف المستخدم من Claims
        }
    }
    
    return base.SaveChanges();
}
```

**مستوى الصعوبة:** متوسطة
**الوقت المتوقع:** 6 ساعات

---

### المشكلة 5: عدم وجود Transaction Logging

**الشدة:** 🟠 عالية
**التأثير:** صعوبة التدقيق والتتبع
**السبب الجذري:** لا يوجد جدول مفصل للعمليات

**الحل الموصى به:**
```csharp
public class TransactionLog
{
    public long TransactionLogId { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    
    public string EntityName { get; set; }
    public int EntityId { get; set; }
    public string Operation { get; set; }  // Create, Update, Delete
    
    public string OldValues { get; set; }  // JSON
    public string NewValues { get; set; }  // JSON
    
    public DateTime Timestamp { get; set; }
    public string IpAddress { get; set; }
    public string UserAgent { get; set; }
}

// Middleware أو Interceptor
public class TransactionLoggingInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, InterceptionResult<int> result, 
        CancellationToken cancellationToken = default)
    {
        var context = eventData.Context;
        var entries = context.ChangeTracker.Entries();
        
        foreach (var entry in entries)
        {
            if (entry.State != EntityState.Unchanged)
            {
                var log = new TransactionLog
                {
                    EntityName = entry.Entity.GetType().Name,
                    Operation = entry.State.ToString(),
                    OldValues = JsonConvert.SerializeObject(entry.OriginalValues),
                    NewValues = JsonConvert.SerializeObject(entry.CurrentValues),
                    Timestamp = DateTime.UtcNow
                };
                context.Set<TransactionLog>().Add(log);
            }
        }
        
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
```

**مستوى الصعوبة:** متوسطة
**الوقت المتوقع:** 4 ساعات

---

### المشكلة 6: عدم توجد Business Rule Validation

**الشدة:** 🟠 عالية
**التأثير:** بيانات غير صحيحة تُدخل النظام
**السبب الجذري:** Validation في DTOs فقط، ليس في Business Logic

**الحل الموصى به:**
```csharp
public class JournalEntryValidator : AbstractValidator<JournalEntryCreateDto>
{
    public JournalEntryValidator()
    {
        RuleFor(x => x.DebitAmount)
            .GreaterThan(0)
            .When(x => x.DebitAmount > 0)
            .WithMessage("Debit amount must be greater than 0");
        
        RuleFor(x => x.CreditAmount)
            .GreaterThan(0)
            .When(x => x.CreditAmount > 0)
            .WithMessage("Credit amount must be greater than 0");
        
        RuleFor(x => new { x.DebitAmount, x.CreditAmount })
            .Custom((amounts, context) =>
            {
                if (amounts.DebitAmount > 0 && amounts.CreditAmount > 0)
                {
                    context.AddFailure("Cannot have both debit and credit");
                }
            });
    }
}

// في Service
public async Task<JournalEntry> CreateEntry(JournalEntryCreateDto dto)
{
    var validator = new JournalEntryValidator();
    var result = await validator.ValidateAsync(dto);
    
    if (!result.IsValid)
    {
        throw new ValidationException(result.Errors);
    }
    
    // تطبيق Business Logic
    var accountType = (await _accountRepository.GetByIdAsync(dto.AccountId)).AccountType;
    
    if (accountType == "Asset" && dto.CreditAmount > 0)
    {
        throw new BusinessRuleException("Assets can only be debited");
    }
    
    // ... إنشاء القيد
}
```

**مستوى الصعوبة:** متوسطة
**الوقت المتوقع:** 3 ساعات

---

### المشكلة 7: نقص في Exception Handling

**الشدة:** 🟠 عالية
**التأثير:** crash غير متوقع، رسائل خطأ غير واضحة
**السبب الجذري:** محاولة التعامل مع جميع الحالات

**الحل الموصى به:**
```csharp
// Global Exception Handler Middleware
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }
    
    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        
        var response = new ErrorResponse();
        
        switch (exception)
        {
            case ValidationException ve:
                context.Response.StatusCode = 400;
                response = new ErrorResponse 
                { 
                    StatusCode = 400, 
                    Message = "Validation error",
                    Errors = ve.Errors.Select(e => e.ErrorMessage).ToList()
                };
                break;
                
            case BusinessRuleException bre:
                context.Response.StatusCode = 422;
                response = new ErrorResponse 
                { 
                    StatusCode = 422, 
                    Message = bre.Message 
                };
                break;
                
            case NotFoundException nfe:
                context.Response.StatusCode = 404;
                response = new ErrorResponse 
                { 
                    StatusCode = 404, 
                    Message = nfe.Message 
                };
                break;
                
            default:
                context.Response.StatusCode = 500;
                response = new ErrorResponse 
                { 
                    StatusCode = 500, 
                    Message = "An internal error occurred" 
                };
                break;
        }
        
        return context.Response.WriteAsJsonAsync(response);
    }
}

// في Program.cs
app.UseMiddleware<ExceptionHandlingMiddleware>();
```

**مستوى الصعوبة:** منخفضة
**الوقت المتوقع:** 2 ساعة

---

## 🟡 المشاكل المتوسطة (Medium Priority)

### المشكلة 8: عدم وجود Pagination في القوائم

**الشدة:** 🟡 متوسطة
**الحل المقترح:** إضافة PaginationParams لكل قائمة

---

### المشكلة 9: التقارير قد تكون بطيئة

**الشدة:** 🟡 متوسطة
**الحل المقترح:** إضافة Indexes على الحقول المستخدمة في الفلاترة

---

### المشكلة 10: عدم وجود Caching

**الشدة:** 🟡 متوسطة
**الحل المقترح:** إضافة Redis أو In-Memory Caching

---

## 🟢 المشاكل المنخفضة (Low Priority)

### المشكلة 11: UI بسيطة جداً

**الشدة:** 🟢 منخفضة
**الحل المقترح:** تحسينات التصميم واستخدام المكتبات

---

## جدول أولويات الإصلاح

| رقم | المشكلة | الشدة | الوقت | الأولوية |
|-----|--------|------|------|---------|
| 1 | تحديث الأرصدة | 🔴 | 4 س | 1 |
| 2 | عكس القيود | 🔴 | 3 س | 2 |
| 3 | العملات المتعددة | 🔴 | 5 س | 3 |
| 4 | Soft Delete | 🟠 | 6 س | 4 |
| 5 | Transaction Logging | 🟠 | 4 س | 5 |
| 6 | Business Rules | 🟠 | 3 س | 6 |
| 7 | Exception Handling | 🟠 | 2 س | 7 |

**إجمالي الجهد:** ~27 ساعة عمل

---

## الخلاصة

معظم المشاكل يمكن إصلاحها بسهولة مع تطبيق الحلول المقترحة. الأولوية الأولى يجب أن تكون تحديث الأرصدة المحاسبية وآلية عكس القيود.
