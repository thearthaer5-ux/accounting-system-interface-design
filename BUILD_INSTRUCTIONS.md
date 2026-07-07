# تعليمات البناء والتشغيل

## المتطلبات الأساسية

- **Visual Studio 2022** Community Edition أو أحدث (مجاني)
- **.NET 8 SDK** من https://dotnet.microsoft.com/download
- **SQL Server Express 2019** أو أحدث (مجاني)
- **SQL Server Management Studio** (مجاني - اختياري)

## خطوات التثبيت والتشغيل

### 1. تثبيت المتطلبات

#### أ) تثبيت .NET 8 SDK
```bash
# تنزيل من الموقع الرسمي:
https://dotnet.microsoft.com/download/dotnet/8.0

# التحقق من التثبيت:
dotnet --version
```

#### ب) تثبيت SQL Server
```bash
# تنزيل SQL Server Express:
https://www.microsoft.com/en-us/sql-server/sql-server-downloads

# تثبيت SQL Server 2019 Express
```

#### ج) تثبيت Visual Studio 2022
```bash
# تنزيل Visual Studio Community:
https://visualstudio.microsoft.com/downloads/

# اختر المكونات:
✓ ASP.NET and web development
✓ .NET desktop development
✓ Data storage and processing (SQL Server tools)
```

### 2. فتح المشروع

```bash
# الطريقة 1: من Visual Studio
File > Open > Project/Solution > اختر EFA.sln

# الطريقة 2: من سطر الأوامر
cd C:\path\to\EFA
dotnet open
```

### 3. استعادة الحزم والمكتبات

```bash
# في Visual Studio:
Tools > NuGet Package Manager > Package Manager Console

# أو من سطر الأوامر:
cd C:\path\to\EFA.Web
dotnet restore
```

### 4. تكوين قاعدة البيانات

#### أ) تعديل سلسلة الاتصال

ملف: `EFA.Web/appsettings.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=EFA016;Trusted_Connection=true;Encrypt=false;TrustServerCertificate=true;"
  }
}
```

**ملاحظات:**
- `Server=.` = جهازك المحلي
- `Database=EFA016` = اسم قاعدة البيانات
- `Trusted_Connection=true` = استخدام المصادقة المدمجة في Windows

#### ب) تشغيل Migrations

```bash
# في Package Manager Console (Tools > NuGet Package Manager > Package Manager Console)

# اختر المشروع الافتراضي:
# Default project: EFA.Infrastructure

# الأوامر:
Add-Migration InitialCreate
Update-Database
```

أو من سطر الأوامر:

```bash
cd C:\path\to\EFA.Web

# تطبيق الترحيلات:
dotnet ef database update -p ..\EFA.Infrastructure
```

### 5. البيانات الافتراضية (اختيارية)

إنشاء مستخدم الإدارة الافتراضي:

```sql
-- قاعدة البيانات: EFA016

-- 1. إضافة مجموعة الإدارة
INSERT INTO Groups (GroupCode, GroupName, Description, IsActive, CreatedDate)
VALUES ('ADMIN', 'المسؤولون', 'مجموعة مسؤولي النظام', 1, GETUTCDATE())

-- 2. الحصول على ID المجموعة
DECLARE @GroupId INT = (SELECT GroupId FROM Groups WHERE GroupCode = 'ADMIN')

-- 3. إضافة مستخدم الإدارة
-- كلمة المرور المشفرة (كمثال):
INSERT INTO Users (Username, Email, PasswordHash, FullName, IsActive, CreatedDate, GroupId)
VALUES ('admin', 'admin@efa.local', 'HASHED_PASSWORD_HERE', 'مدير النظام', 1, GETUTCDATE(), @GroupId)

-- 4. إضافة الفرع الرئيسي
INSERT INTO Branches (BranchCode, BranchName, IsActive, IsHeadOffice, CreatedDate)
VALUES ('HQ', 'المقر الرئيسي', 1, 1, GETUTCDATE())

-- 5. إضافة العملة الافتراضية
INSERT INTO Currencies (CurrencyCode, CurrencyName, Symbol, IsDefault, IsActive, ExchangeRate, CreatedDate)
VALUES ('SAR', 'الريال السعودي', 'ر.س', 1, 1, 1.0, GETUTCDATE())
```

**ملاحظة:** لتشفير كلمة المرور، استخدم نفس الطريقة المستخدمة في `UserService.cs`

### 6. تشغيل التطبيق

#### في Visual Studio:
1. اضغط **F5** أو
2. Debug > Start Debugging أو
3. اضغط الزر الأخضر (Start Debugging)

#### من سطر الأوامر:
```bash
cd EFA.Web
dotnet run
```

### 7. الوصول إلى التطبيق

افتح المتصفح وانتقل إلى:
```
https://localhost:7001
```

**أو:**
```
http://localhost:5000
```

البيانات الافتراضية:
- **اسم المستخدم:** admin
- **كلمة المرور:** (تحتاج إلى تشفيرها أولاً)

## معالجة المشاكل الشائعة

### المشكلة 1: خطأ "Cannot connect to database"

**الحل:**
1. تأكد من تشغيل SQL Server
2. تحقق من سلسلة الاتصال في `appsettings.json`
3. جرب الاتصال من SQL Server Management Studio

### المشكلة 2: خطأ "There are pending model changes"

**الحل:**
```bash
# إنشاء ترحيل جديد
Add-Migration FixPendingChanges

# تطبيق الترحيل
Update-Database
```

### المشكلة 3: خطأ "The target database does not exist"

**الحل:**
```sql
-- في SQL Server Management Studio
CREATE DATABASE EFA016;
```

ثم شغل `Update-Database` مرة أخرى

### المشكلة 4: خطأ "Port is already in use"

**الحل:**
```bash
# تغيير البورت في launchSettings.json
# أو قتل العملية:
netstat -ano | findstr :5000
taskkill /PID <PID> /F
```

### المشكلة 5: خطأ "Restore failed" عند استعادة الحزم

**الحل:**
```bash
# مسح الحزم المؤقتة
dotnet nuget locals all --clear

# محاولة الاستعادة مرة أخرى
dotnet restore
```

## تطوير إضافي

### إضافة اسم مستخدم جديد من واجهة المستخدم

1. انتقل إلى `/Account/Register`
2. ملء البيانات المطلوبة
3. اضغط "إنشاء الحساب"

### إنشاء مجموعة صلاحيات جديدة

1. سجل دخول كمسؤول
2. انتقل إلى `/Group/Create`
3. ملء بيانات المجموعة
4. اضغط "إنشاء"

## أوامر مفيدة

```bash
# بناء المشروع
dotnet build

# تشغيل الاختبارات
dotnet test

# نشر التطبيق
dotnet publish -c Release

# عرض إصدار .NET
dotnet --version

# عرض المشاريع المثبتة
dotnet sln list

# إنشاء ترحيل جديد
dotnet ef migrations add <MigrationName> -p EFA.Infrastructure

# إزالة آخر ترحيل
dotnet ef migrations remove -p EFA.Infrastructure
```

## الخطوات التالية

بعد التشغيل الناجح:

1. **اختبر المستخدمين:**
   - اذهب إلى `/UserManagement/Index`
   - أنشئ مستخدم جديد
   - عدّل المستخدم

2. **اختبر الصلاحيات:**
   - اذهب إلى `/Group/Index`
   - أنشئ مجموعة جديدة
   - عيّن الصلاحيات

3. **اختبر الفروع:**
   - اذهب إلى `/Branch/Index`
   - أنشئ فرع جديد

4. **اختبر العملات:**
   - اذهب إلى `/Currency/Index`
   - أنشئ عملة جديدة

## موارد إضافية

- [ASP.NET Core Documentation](https://docs.microsoft.com/aspnet/core/)
- [Entity Framework Core](https://docs.microsoft.com/ef/core/)
- [SQL Server Documentation](https://docs.microsoft.com/sql/sql-server/)
- [Visual Studio Documentation](https://docs.microsoft.com/visualstudio/)

## الدعم

في حالة واجهت أي مشاكل:
1. راجع التوثيق في `README.md`
2. تحقق من أخطاء البناء في Output window
3. استشر فريق الدعم

---

**تم آخر تحديث:** 7 يوليو 2024
