# دليل البدء السريع - المرحلة الثانية

## البدء مع إدارة المخزون

---

## 1. المتطلبات الأساسية

قبل البدء، تأكد من توفر:

```
✓ Visual Studio 2022
✓ .NET 8 SDK
✓ SQL Server 2019 أو أحدث
✓ SQL Server Management Studio (SSMS)
✓ NuGet Packages (سيتم تثبيتها تلقائياً)
```

---

## 2. خطوات الإعداد

### الخطوة 1: فتح المشروع

```
1. افتح Visual Studio 2022
2. File > Open > Project/Solution
3. اختر ملف EFA.sln
4. انتظر تحميل المشروع والـ NuGet Packages
```

### الخطوة 2: التحقق من Connection String

افتح ملف `appsettings.json` وتحقق من:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=EFA_DB;Trusted_Connection=true;"
  }
}
```

قم بتحديث اسم الخادم والقاعدة إن لزم الأمر.

### الخطوة 3: تحديث قاعدة البيانات

افتح **Package Manager Console** واشغل:

```powershell
Add-Migration AddInventoryModule
Update-Database
```

هذا سينشئ جميع الجداول الجديدة.

### الخطوة 4: تشغيل التطبيق

```
اضغط F5 أو Ctrl+F5
التطبيق سيفتح على: https://localhost:7001
```

---

## 3. تسجيل الدخول

```
اسم المستخدم: admin
كلمة المرور: Admin@123456
```

---

## 4. الميزات المتاحة

### إدارة الأصناف
```
📍 الموقع: القائمة الرئيسية > الأصناف
✓ عرض جميع الأصناف
✓ إضافة صنف جديد
✓ تعديل بيانات الصنف
✓ حذف الأصناف غير المستخدمة
✓ البحث والتصفية
✓ عرض الأصناف منخفضة المخزون
```

### إدارة المستودعات
```
📍 الموقع: القائمة الرئيسية > المستودعات
✓ عرض جميع المستودعات
✓ إضافة مستودع جديد
✓ تعديل بيانات المستودع
✓ عرض ملخص المخزون
✓ دعم المستودعات المتعددة
```

### إدارة المخزون
```
📍 الموقع: القائمة الرئيسية > المخزون
✓ عرض أرصدة المخزون
✓ إضافة حركات مخزون جديدة
✓ الترحيل المحاسبي للحركات
✓ عرض سجل الحركات
✓ تصفية حسب التاريخ
✓ حساب متوسط التكلفة تلقائياً
```

---

## 5. أمثلة عملية

### مثال 1: إنشاء صنف جديد

```
1. انقر على "الأصناف" من القائمة
2. اضغط "صنف جديد"
3. أدخل البيانات:
   - الكود: PROD-001
   - الاسم: منتج تجريبي
   - الفئة: اختر الفئة المناسبة
   - الكود: 100.00
   - السعر: 150.00
4. اضغط "حفظ"
```

### مثال 2: إضافة حركة مخزون

```
1. انقر على "المخزون" > "حركة جديدة"
2. اختر:
   - الصنف: المنتج الذي أنشأته
   - المستودع: المستودع الرئيسي
   - نوع الحركة: دخول
   - الكمية: 100
   - التكلفة: 100.00
3. اضغط "حفظ"
4. اضغط "ترحيل" لترحيل الحركة المحاسبية
```

### مثال 3: عرض أرصدة المخزون

```
1. انقر على "المخزون" > "الأرصدة"
2. سترى جدول يحتوي على:
   - كود الصنف والاسم
   - اسم المستودع
   - الكمية المتاحة
   - متوسط التكلفة
   - القيمة الإجمالية
   - آخر حركة
```

---

## 6. API Endpoints

### الحصول على الأصناف

```http
GET /api/items
```

### الحصول على أرصدة المستودع

```http
GET /api/inventory/warehouse-balances/1
```

### إضافة حركة مخزون

```http
POST /api/inventory/movement
Content-Type: application/json

{
  "itemId": 1,
  "warehouseId": 1,
  "warehouseIdTo": null,
  "movementType": 1,
  "movementQuantity": 100,
  "movementCost": 100.00,
  "referenceDocumentType": "Manual",
  "notes": "حركة يدوية"
}
```

### ترحيل الحركة

```http
POST /api/inventory/post-movement/1
```

---

## 7. استكشاف الأخطاء الشائعة

### مشكلة: لا يمكن الاتصال بقاعدة البيانات

**الحل:**
```
1. تحقق من تشغيل SQL Server
2. تأكد من اسم الخادم في appsettings.json
3. جرب: (local)\\SQLEXPRESS أو . بدلاً من اسم الخادم الفعلي
```

### مشكلة: خطأ في Migration

**الحل:**
```powershell
# احذف المحاولة السابقة
Remove-Migration

# أنشئ من جديد
Add-Migration AddInventoryModule
Update-Database
```

### مشكلة: الأصناف لا تظهر بعد الإنشاء

**الحل:**
```
1. تحقق من أن الصنف تم حفظه
2. تأكد من أن الصنف نشط (IsActive = true)
3. حاول تحديث الصفحة (F5)
```

---

## 8. الملفات المهمة

| الملف | الوصف |
|-------|---------|
| `appsettings.json` | إعدادات الاتصال والتطبيق |
| `EFA.sln` | ملف الحل الرئيسي |
| `EFA.Domain/` | الكيانات والنماذج |
| `EFA.Infrastructure/` | قاعدة البيانات والـ Repositories |
| `EFA.Application/` | الخدمات والـ DTOs |
| `EFA.Web/` | المتحكمات والعروض |

---

## 9. الهيكل الديناميكي

```
EFA/
├── EFA.Domain/
│   └── Entities/
│       ├── Item.cs
│       ├── Warehouse.cs
│       ├── ItemBalance.cs
│       ├── ItemMovement.cs
│       └── ...
├── EFA.Infrastructure/
│   ├── Data/
│   │   └── EFADbContext.cs
│   └── Repositories/
│       ├── ItemRepository.cs
│       ├── WarehouseRepository.cs
│       └── ...
├── EFA.Application/
│   ├── Services/
│   │   ├── ItemService.cs
│   │   ├── WarehouseService.cs
│   │   └── ...
│   └── DTOs/
│       └── InventoryDtos.cs
└── EFA.Web/
    ├── Controllers/
    │   ├── ItemController.cs
    │   ├── WarehouseController.cs
    │   ├── InventoryController.cs
    │   └── Api/
    ├── Views/
    │   ├── Item/
    │   ├── Warehouse/
    │   └── Inventory/
    └── wwwroot/
```

---

## 10. الخطوات التالية

بعد فهمك للمرحلة الثانية، يمكنك:

1. **استكشاف الكود** في جميع الملفات
2. **تعديل الواجهات** حسب احتياجاتك
3. **إضافة حقول جديدة** للكيانات
4. **إنشاء Reports** مخصصة
5. **التطوير للمرحلة الثالثة**: الحسابات المحاسبية

---

## 11. معلومات مفيدة

### تغيير اللغة

جميع الرسائل والعناوين تظهر بالعربية افتراضياً. 
إذا أردت تغييرها، عدّل:

```csharp
// في Controllers
ModelState.AddModelError("", "رسالة الخطأ");

// في Views
<h1>@Strings.ItemsTitle</h1>
```

### إضافة حقول جديدة

لإضافة حقل جديد للصنف مثلاً:

```csharp
// 1. أضفه في Entity
public class Item {
    public string SKU { get; set; }  // حقل جديد
}

// 2. أضفه في DTO
public class ItemDto {
    public string SKU { get; set; }
}

// 3. أضفه في AutoMapper
CreateMap<Item, ItemDto>();

// 4. أضفه في View
<input asp-for="SKU" class="form-control" />

// 5. شغّل Migration
Add-Migration AddSKUField
Update-Database
```

---

## 12. الدعم

للمزيد من التفاصيل:

- اقرأ `PHASE2_DOCUMENTATION.md` للتوثيق الشامل
- اقرأ `README.md` للدليل العام
- تحقق من التعليقات في الكود

---

**استمتع بتطوير نظام المخزون!**

هل تريد بدء المرحلة الثالثة: الحسابات المحاسبية؟
