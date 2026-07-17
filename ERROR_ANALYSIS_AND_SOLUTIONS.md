# تحليل الأخطاء والحلول (Error Analysis & Solutions)

## نظام المحاسبة المتكامل (EFA - Enterprise Financial Accounting)
**تاريخ التقرير:** 2026-07-14

---

## الأخطاء المكتشفة والحلول

### ❌ الخطأ 1: Metadata File Not Found
```
"Metadata file 'D:\...\EFA.Domain\obj\Debug\net8.0\ref\EFA.Domain.dll' could not be found"
```

**السبب:**
- مجلد Build Cache (obj/bin) معطوب أو غير متزامن
- مشكلة في الاعتماديات بين المشاريع
- Build لم ينته بنجاح من قبل

**الحل:**
```bash
# الخطوة 1: اغلق Visual Studio تماماً

# الخطوة 2: احذف مجلدات obj و bin
del /s /q obj bin  (Windows)
rm -rf obj bin     (Mac/Linux)

# الخطوة 3: فتح Visual Studio من جديد

# الخطوة 4: Rebuild Solution
Build → Clean Solution
Build → Rebuild Solution
```

---

### ❌ الخطأ 2: Ambiguity Between CostCenter Properties
```
"Ambiguity between 'CostCenter.CostCenterId' and 'CostCenter.CostCenterId'"
"Ambiguity between 'CostCenter.CostCenterCode' and 'CostCenter.CostCenterCode'"
"Ambiguity between 'CostCenter.Description' and 'CostCenter.Description'"
```

**السبب:**
- هناك نسختين من Entity CostCenter في المشروع
- ملف CostCenter.cs موجود في مكانين مختلفين
- Compiler يرى نفس الكيان مرتين

**الحل:**

الخطوة 1: ابحث عن جميع ملفات CostCenter
```bash
Get-ChildItem -Path . -Filter "CostCenter.cs" -Recurse  (PowerShell)
find . -name "CostCenter.cs"                             (bash)
```

الخطوة 2: احذف النسخة المكررة
- اترك نسخة واحدة فقط في: `EFA.Domain/Entities/CostCenter.cs`
- احذف أي نسخة مكررة أخرى

الخطوة 3: تأكد من عدم وجود:
- `EFA.Domain/Entities/Phase3/CostCenter.cs`
- `EFA.Domain/Entities/CostCenter.cs` (نسخة ثانية)

---

### ❌ الخطأ 3: Branch Does Not Contain Definition for Warehouses
```
"'Branch' does not contain a definition for 'Warehouses' and no accessible 
extension method 'Warehouses' accepting a first argument of type 'Branch' 
could be found"
```

**السبب:**
- Entity Branch ناقص منه الخاصية `ICollection<Warehouse>`
- Navigation Property لم تُعرَّف بشكل صحيح
- العلاقة بين Branch و Warehouse لم تكتمل

**الحل:**

افتح ملف: `EFA.Domain/Entities/Branch.cs`

تأكد من وجود الخاصية:
```csharp
public class Branch
{
    public int BranchId { get; set; }
    public string BranchCode { get; set; }
    public string BranchName { get; set; }
    public string Address { get; set; }
    // ... باقي الخصائص
    
    // أضف هذا السطر إذا كان ناقساً:
    public virtual ICollection<Warehouse> Warehouses { get; set; } 
        = new List<Warehouse>();
    
    // وتأكد أيضاً من وجود CostCenters:
    public virtual ICollection<CostCenter> CostCenters { get; set; }
        = new List<CostCenter>();
}
```

---

### ❌ الخطأ 4: AutoMapper Version Conflict
```
"Detected package version outside of dependency constraint: 
AutoMapper.Extensions.Microsoft.DependencyInjection 12.0.1 requires 
AutoMapper (= 12.0.1) but version AutoMapper 13.0.1 was resolved."
```

**السبب:**
- الإصدار المطلوب: AutoMapper 12.0.1
- الإصدار المثبت: AutoMapper 13.0.1
- عدم تطابق الإصدارات بين الحزم

**الحل:**

الخيار 1: استخدام Package Manager Console (Visual Studio)
```powershell
Update-Package AutoMapper -Version 12.0.1
Update-Package AutoMapper.Extensions.Microsoft.DependencyInjection -Version 12.0.1
```

الخيار 2: استخدام CLI
```bash
# احذف الحزم الحالية
dotnet remove package AutoMapper
dotnet remove package AutoMapper.Extensions.Microsoft.DependencyInjection

# ثبّت الإصدارات الصحيحة
dotnet add package AutoMapper --version 12.0.1
dotnet add package AutoMapper.Extensions.Microsoft.DependencyInjection --version 12.0.1
```

الخيار 3: تعديل .csproj يدويّاً
```xml
<ItemGroup>
    <PackageReference Include="AutoMapper" Version="12.0.1" />
    <PackageReference Include="AutoMapper.Extensions.Microsoft.DependencyInjection" Version="12.0.1" />
</ItemGroup>
```

ثم: `dotnet restore`

---

### ❌ الخطأ 5: Security Vulnerability
```
"Package 'AutoMapper' 13.0.1 has a known high severity vulnerability
https://github.com/advisories/GHSA-rvv3-g6hj-g44x"
```

**السبب:**
- الإصدار 13.0.1 يحتوي على ثغرة أمنية معروفة
- Vulnerability من النوع High Severity
- يجب تحديث أو خفض الإصدار

**الحل:**
استخدام إصدار آمن (12.0.1):
```bash
dotnet add package AutoMapper --version 12.0.1
```

---

## 📊 جدول تلخيصي للأخطاء والحلول

| الخطأ | المشكلة | الحل السريع |
|-------|--------|-----------|
| Metadata Not Found | مجلد obj معطوب | Clean & Rebuild Solution |
| Ambiguity (CostCenter) | كيان مكرر | احذف النسخة المكررة |
| Warehouses Missing | Navigation Property ناقصة | أضف ICollection<Warehouse> |
| Version Conflict | إصدارات متضاربة | استخدم AutoMapper 12.0.1 |
| Vulnerability | ثغرة أمنية | حدّث إلى إصدار آمن |

---

## ✅ خطوات الإصلاح الشاملة

### الخطوة 1: تنظيف البناء
```bash
dotnet clean
# أو في Visual Studio: Build → Clean Solution
```

### الخطوة 2: حذف مجلدات البناء
```bash
# Windows
del /s /q obj bin

# Mac/Linux
rm -rf obj bin
```

### الخطوة 3: استعادة الحزم
```bash
dotnet restore
```

### الخطوة 4: البحث عن الملفات المكررة
```bash
Get-ChildItem -Path . -Filter "CostCenter.cs" -Recurse
```

### الخطوة 5: تحديث AutoMapper
```bash
dotnet remove package AutoMapper
dotnet remove package AutoMapper.Extensions.Microsoft.DependencyInjection
dotnet add package AutoMapper --version 12.0.1
dotnet add package AutoMapper.Extensions.Microsoft.DependencyInjection --version 12.0.1
```

### الخطوة 6: إضافة الخصائص الناقصة
- افتح: `EFA.Domain/Entities/Branch.cs`
- أضف: `public virtual ICollection<Warehouse> Warehouses { get; set; }`

### الخطوة 7: البناء
```bash
dotnet build
```

---

## 🎯 التحقق من النجاح

بعد تطبيق الحلول، يجب أن ترى:

```
✅ Build succeeded.
✅ 0 Warning(s)
✅ 0 Error(s)
```

إذا رأيت هذه الرسالة، فالمشروع جاهز للتشغيل!

---

## 📝 ملاحظات إضافية

1. **عند وجود مشاكل جديدة:**
   - حاول الحل الأساسي أولاً: Clean & Rebuild
   - ثم جرّب: `dotnet clean && dotnet restore && dotnet build`

2. **للتحقق من الحزم:**
   ```bash
   dotnet list package
   ```

3. **للحصول على تفاصيل الخطأ:**
   ```bash
   dotnet build --verbose
   ```

4. **في حالة الاستمرار الفشل:**
   - اغلق Visual Studio
   - احذف مجلد `.vs` المخفي (موجود في root المشروع)
   - فتح المشروع من جديد

---

## 🔗 المراجع المفيدة

- [AutoMapper Documentation](https://docs.automapper.org)
- [Entity Framework Core](https://docs.microsoft.com/ef)
- [.NET Build Issues](https://github.com/dotnet/roslyn/wiki/Building-Testing-and-Debugging)

---

**تم الإعداد:** 2026-07-14  
**الحالة:** شامل وجاهز للتطبيق
