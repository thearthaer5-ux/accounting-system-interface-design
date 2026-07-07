# نظام إدارة المحاسبة المتكامل (EFA)

## نظرة عامة

نظام محاسبي متكامل مبني على ASP.NET Core MVC يوفر إدارة شاملة للعمليات المحاسبية والمالية. تم تطوير المرحلة الأولى بالتركيز على **إدارة النظام والأمان**.

## المتطلبات الأساسية

- **Visual Studio 2022** أو أحدث
- **.NET 8 SDK**
- **SQL Server 2019** أو أحدث
- **Windows 10/11** أو Windows Server

## البنية المعمارية

المشروع مقسم إلى 4 مشاريع رئيسية:

```
EFA.sln
├── EFA.Domain          - نماذج وكيانات المجال
├── EFA.Infrastructure  - طبقة البيانات والمستودعات
├── EFA.Application     - خدمات الأعمال والـ DTOs
└── EFA.Web            - تطبيق الويب (MVC)
```

## الخطوات الأولى

### 1. استنساخ وفتح المشروع

```bash
# فتح الحل في Visual Studio
File > Open > Project/Solution > EFA.sln
```

### 2. تحديث قاعدة البيانات

```bash
# في Package Manager Console:
Add-Migration InitialCreate
Update-Database
```

أو استخدم الأوامر:

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### 3. إعداد سلسلة الاتصال

تحديث `appsettings.json` في مشروع `EFA.Web`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=EFA016;Trusted_Connection=true;Encrypt=false;TrustServerCertificate=true;"
  }
}
```

### 4. تشغيل التطبيق

```bash
# اضغط F5 أو استخدم:
dotnet run
```

التطبيق سيفتح على `https://localhost:7xxx`

## ميزات المرحلة الأولى

### 1. إدارة المستخدمين
- ✓ التسجيل والدخول الآمن
- ✓ إدارة حسابات المستخدمين
- ✓ تغيير كلمة المرور
- ✓ تفعيل/تعطيل المستخدمين
- ✓ تتبع آخر دخول

**المسارات:**
- `/Account/Login` - تسجيل الدخول
- `/Account/Register` - إنشاء حساب جديد
- `/Account/Profile` - الملف الشخصي
- `/Account/ChangePassword` - تغيير كلمة المرور
- `/UserManagement/Index` - قائمة المستخدمين

### 2. إدارة المجموعات والصلاحيات
- ✓ إنشاء وتعديل مجموعات الصلاحيات
- ✓ تعيين صلاحيات للمجموعات
- ✓ ربط المستخدمين بالمجموعات
- ✓ التحقق من الصلاحيات تلقائياً

**المسارات:**
- `/Group/Index` - قائمة المجموعات
- `/Group/Create` - إنشاء مجموعة جديدة
- `/Group/Edit/{id}` - تعديل المجموعة
- `/Group/AssignPrivileges/{id}` - تعيين الصلاحيات

### 3. إدارة الفروع
- ✓ إنشاء وتعديل فروع الشركة
- ✓ تحديد الفرع الرئيسي
- ✓ إدارة معلومات الفرع

**المسارات:**
- `/Branch/Index` - قائمة الفروع
- `/Branch/Create` - إنشاء فرع جديد
- `/Branch/Edit/{id}` - تعديل الفرع

### 4. إدارة العملات
- ✓ إنشاء وتعديل العملات
- ✓ تحديث أسعار الصرف
- ✓ تحديد العملة الافتراضية

**المسارات:**
- `/Currency/Index` - قائمة العملات
- `/Currency/Create` - إنشاء عملة جديدة
- `/Currency/Edit/{id}` - تعديل العملة

### 5. نظام التدقيق والأمان
- ✓ تسجيل جميع العمليات
- ✓ تتبع التغييرات
- ✓ المصادقة والتفويض
- ✓ حماية من SQL Injection و XSS

## الجداول الرئيسية

| الجدول | الوصف |
|--------|-------|
| `Users` | بيانات المستخدمين |
| `Groups` | مجموعات الصلاحيات |
| `Privileges` | الصلاحيات المتاحة |
| `GroupPrivileges` | ربط المجموعات بالصلاحيات |
| `Branches` | فروع الشركة |
| `Currencies` | العملات |
| `UserDevices` | أجهزة المستخدمين |
| `UserLogs` | سجل نشاط المستخدمين |
| `Audit` | سجل التدقيق |
| `CostCenters` | مراكز التكلفة |
| `SystemParameters` | معاملات النظام |

## هيكل الملفات

```
EFA.Domain/
├── Entities/
│   ├── User.cs
│   ├── Group.cs
│   ├── Privilege.cs
│   ├── Branch.cs
│   ├── Currency.cs
│   └── ...

EFA.Infrastructure/
├── Data/
│   └── EFADbContext.cs
├── Repositories/
│   ├── IGenericRepository.cs
│   ├── GenericRepository.cs
│   ├── IUserRepository.cs
│   └── ...

EFA.Application/
├── Services/
│   ├── IUserService.cs
│   ├── UserService.cs
│   ├── GroupService.cs
│   └── ...
├── DTOs/
│   ├── UserDto.cs
│   ├── GroupDto.cs
│   └── ...
└── Profiles/
    └── AutoMapperProfile.cs

EFA.Web/
├── Controllers/
│   ├── AccountController.cs
│   ├── UserManagementController.cs
│   ├── GroupController.cs
│   └── ...
├── Views/
│   ├── Account/
│   ├── UserManagement/
│   ├── Group/
│   └── ...
└── wwwroot/
    ├── css/
    └── js/
```

## أمثلة الاستخدام

### تسجيل مستخدم جديد

```csharp
var createUserDto = new CreateUserDto
{
    Username = "user123",
    Email = "user@example.com",
    Password = "SecurePassword123!",
    FullName = "أحمد محمد"
};

var result = await _userService.RegisterAsync(createUserDto);
if (result.Success)
{
    Console.WriteLine("تم التسجيل بنجاح");
}
```

### التحقق من الصلاحيات

```csharp
// في Controller
var hasPermission = await _userService.HasPrivilegeAsync(userId, "User_Edit");
if (!hasPermission)
{
    return Forbid();
}
```

### إنشاء مجموعة صلاحيات

```csharp
var createGroupDto = new CreateGroupDto
{
    GroupCode = "ADMIN",
    GroupName = "المسؤولون",
    Description = "مجموعة المسؤولين في النظام"
};

var result = await _groupService.CreateGroupAsync(createGroupDto);
```

## معايير الأمان المطبقة

1. **المصادقة:**
   - تسجيل دخول آمن مع SHA256 Hashing
   - إدارة الجلسات

2. **التفويض:**
   - Role-Based Access Control (RBAC)
   - Permission-Based Access Control
   - Attribute-based Authorization

3. **حماية البيانات:**
   - SQL Injection Prevention (EF Core Parameterized Queries)
   - XSS Protection (Razor View Encoding)
   - CSRF Protection (AntiForgeryToken)

4. **التدقيق:**
   - تسجيل شامل للعمليات
   - تتبع التغييرات
   - سجل الأخطاء

## الملحقات والمكتبات المستخدمة

```xml
- Microsoft.EntityFrameworkCore 8.0.0
- Microsoft.EntityFrameworkCore.SqlServer 8.0.0
- AutoMapper 13.0.0
- Bootstrap 5
- jQuery
```

## المرحلة الثانية (قادمة)

ستركز على:
- إدارة المخزون والأصناف
- حركات المخزون والأرصدة
- إدارة الجرد

## المرحلة الثالثة (قادمة)

ستركز على:
- إدارة الحسابات المحاسبية
- شجرة الحسابات
- القيود والترحيل المحاسبي

## استكشاف الأخطاء

### خطأ الاتصال بقاعدة البيانات

```
Unable to connect to SQL Server
```

**الحل:**
- تحقق من اسم الخادم وقاعدة البيانات
- تأكد من تشغيل SQL Server
- تحقق من الصلاحيات

### خطأ في الترحيل

```
There are pending model changes
```

**الحل:**
```bash
Add-Migration FixName
Update-Database
```

### الصفحة غير موجودة

```
404 Not Found
```

**الحل:**
- تأكد من اسم الـ Controller والـ Action
- تحقق من المسارات في الـ Routes

## الدعم والمساعدة

للمزيد من المعلومات أو الدعم:
- تابع التوثيق
- راجع الأكواد المعلقة (Comments)
- استشر فريق الدعم

## الترخيص

جميع الحقوق محفوظة © 2024

---

**تم التطوير بواسطة:** فريق التطوير
**آخر تحديث:** 7 يوليو 2024
**الإصدار:** 1.0.0 (المرحلة الأولى)
