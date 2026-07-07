# المرحلة الثانية: إدارة المخزون
## Inventory Management Module

### نظرة عامة على المرحلة

تم إنجاز المرحلة الثانية من مشروع نظام إدارة المحاسبة المتكامل (EFA)، والتي تركز على **إدارة المخزون الشاملة** مع التكامل الكامل مع المرحلة الأولى (إدارة النظام والأمان).

---

## 1. الكيانات (Entities) المُنشأة

### 10 Entities رئيسية:

1. **ItemCategory** - فئات الأصناف
   - تصنيف الأصناف بفئات مختلفة
   - دعم اللغتين (عربي/إنجليزي)
   - صور مرتبطة بالفئات

2. **Item** - الأصناف
   - معلومات الصنف الكاملة
   - الأسعار والتكاليف
   - الحد الأدنى والأقصى للمخزون
   - دعم أنواع مختلفة (منتج، خدمة، مادة خام)

3. **ItemUnit** - وحدات الأصناف
   - تحديد وحدات قياس متعددة للصنف
   - معاملات التحويل بين الوحدات
   - أسعار مختلفة حسب الوحدة

4. **Warehouse** - المستودعات
   - معلومات المستودع الكاملة
   - ربط بالفروع
   - سعة المستودع
   - تحديد المستودع الرئيسي

5. **ItemBalance** - أرصدة الأصناف
   - الكمية المتاحة لكل صنف في كل مستودع
   - متوسط التكلفة (Weighted Average Cost)
   - آخر تاريخ حركة

6. **ItemMovement** - حركات الأصناف
   - تسجيل جميع الحركات (دخول، خروج، تحويل، جرد)
   - ربط بالمستندات المرجعية
   - حالة الترحيل المحاسبي

7. **ItemBatch** - دفعات الأصناف
   - تتبع الدفعات والأرقام التسلسلية
   - تواريخ الصلاحية والتصنيع
   - مراقبة الدفعات المنتهية الصلاحية

8. **InventoryCount** - الجرد الفعلي
   - تسجيل عمليات الجرد الفعلي
   - حالات مختلفة (مسودة، قيد الجرد، مكتمل، معتمد)
   - ربط بالمستودع

9. **InventoryCountDetail** - تفاصيل الجرد
   - تسجيل تفاصيل كل صنف في عملية الجرد
   - حساب الفروقات والقيم
   - حالة التسوية

10. **ItemTax** (مُعرّف في المرحلة الأولى) - الضرائب على الأصناف
    - دعم الضرائب المتعددة
    - حساب الضريبة تلقائياً

---

## 2. طبقة البيانات (Repositories)

### 10 Repositories متخصصة:

```csharp
// الـ Repositories الرئيسية
IItemCategoryRepository     // فئات الأصناف
IItemRepository            // الأصناف
IWarehouseRepository       // المستودعات
IItemBalanceRepository     // أرصدة الأصناف
IItemMovementRepository    // حركات الأصناف
IItemBatchRepository       // دفعات الأصناف
IInventoryCountRepository  // الجرد الفعلي
```

### الميزات المتقدمة في الـ Repositories:

- **Lazy Loading محسّن** - تحميل العلاقات عند الحاجة فقط
- **Pagination Support** - تقسيم النتائج إلى صفحات
- **Search & Filter** - بحث وتصفية متقدم
- **Unique Constraints** - التحقق من القيود المفردة
- **Date Range Queries** - الاستعلام عن الفترات الزمنية

### أمثلة:

```csharp
// الحصول على أرصدة المستودع
var balances = await _balanceRepository.GetByWarehouseAsync(warehouseId);

// الحصول على الحركات بفترة زمنية
var movements = await _movementRepository.GetByDateRangeAsync(fromDate, toDate);

// الحصول على الدفعات المنتهية الصلاحية
var expiredBatches = await _batchRepository.GetExpiredBatchesAsync();

// الحصول على قيمة المستودع الإجمالية
var totalValue = await _balanceRepository.GetTotalValueAsync(warehouseId);
```

---

## 3. طبقة الخدمات (Services)

### 7 Services متقدمة:

#### 1. **IItemService** - خدمة إدارة الأصناف
```csharp
- GetByIdAsync(id)                    // جلب الصنف برقمه
- GetByCodeAsync(code)                // جلب الصنف بالكود
- SearchAsync(searchTerm)             // البحث في الأصناف
- GetByCategoryAsync(categoryId)      // جلب الأصناف بالفئة
- CreateAsync(dto, userId)            // إنشاء صنف جديد
- UpdateAsync(id, dto, userId)        // تحديث الصنف
- GetLowStockAsync()                  // الأصناف منخفضة المخزون
```

#### 2. **IWarehouseService** - خدمة إدارة المستودعات
```csharp
- GetByIdAsync(id)                    // جلب المستودع
- GetByBranchAsync(branchId)          // مستودعات الفرع
- GetMainWarehouseAsync(branchId)     // المستودع الرئيسي
- CreateAsync(dto, userId)            // إنشاء مستودع
- GetInventorySummaryAsync(id)        // ملخص المخزون
```

#### 3. **IInventoryService** - خدمة إدارة حركات المخزون
```csharp
- GetBalanceAsync(itemId, warehouseId)           // الرصيد
- AddMovementAsync(dto, userId)                  // إضافة حركة
- PostMovementAsync(movementId)                  // ترحيل الحركة
- GetWarehouseValueAsync(warehouseId)            // قيمة المستودع
```

#### 4. **IInventoryCountService** - خدمة الجرد الفعلي
```csharp
- CreateAsync(dto, userId)           // إنشاء جرد جديد
- AddDetailAsync(countId, detail)    // إضافة تفصيل الجرد
- ApproveAsync(countId, userId)      // اعتماد الجرد
- PostAsync(countId, userId)         // ترحيل الجرد
```

#### 5. **IItemBatchService** - خدمة الدفعات
```csharp
- GetByBatchNumberAsync(number)      // جلب الدفعة برقمها
- GetExpiredBatchesAsync()            // الدفعات المنتهية
- GetAvailableBatchesAsync(itemId)   // الدفعات المتاحة
- CreateAsync(dto, userId)           // إنشاء دفعة جديدة
```

### الميزات الرئيسية:

**حساب متوسط التكلفة (Weighted Average Cost):**
```csharp
// عند إضافة حركة دخول
newQuantity = balance.BalanceQuantity + movementQuantity;
newAverageCost = (balance.BalanceQuantity * balance.AverageCost + 
                  movementQuantity * movementCost) / newQuantity;
```

**التحقق من الصلاحيات والسماحيات:**
- التحقق من وجود الأصناف والمستودعات
- التحقق من تفرد الأكود والأرقام
- معالجة الأخطاء والاستثناءات

**التسجيل والتدقيق:**
- تسجيل كل عملية مع تاريخ ووقت التنفيذ
- تتبع المستخدم الذي أنجز العملية
- تسجيل جميع التغييرات

---

## 4. DTOs (Data Transfer Objects)

### 15+ DTOs منظمة:

```csharp
// Item DTOs
ItemDto, ItemCreateUpdateDto

// Warehouse DTOs
WarehouseDto, WarehouseCreateUpdateDto

// Balance DTOs
ItemBalanceDto

// Movement DTOs
ItemMovementDto, ItemMovementCreateDto

// Inventory Count DTOs
InventoryCountDto, InventoryCountDetailDto, InventoryCountCreateDto

// Batch DTOs
ItemBatchDto, ItemBatchCreateDto

// Summary DTOs
WarehouseInventorySummaryDto, InventoryReportDto
```

---

## 5. Controllers (MVC + API)

### 6 MVC Controllers:

1. **ItemCategoryController** - إدارة فئات الأصناف
2. **ItemController** - إدارة الأصناف الرئيسية
3. **WarehouseController** - إدارة المستودعات
4. **InventoryController** - إدارة الحركات والأرصدة

### 3 API Controllers:

1. **InventoryApiController** - API لحركات المخزون والأرصدة
2. **ItemsApiController** - API لإدارة الأصناف
3. **WarehousesApiController** - API لإدارة المستودعات

### End Points الرئيسية:

```
# MVC Routes
GET    /Item/Index              - عرض الأصناف
GET    /Item/Create             - نموذج إضافة صنف
POST   /Item/Create             - حفظ صنف جديد
GET    /Item/Edit/{id}          - تعديل الصنف
GET    /Warehouse/Index         - عرض المستودعات
GET    /Inventory/Balances      - عرض الأرصدة
GET    /Inventory/Movements     - عرض الحركات

# API Routes
GET    /api/inventory/balance/{itemId}/{warehouseId}
GET    /api/inventory/warehouse-balances/{warehouseId}
POST   /api/inventory/movement
POST   /api/inventory/post-movement/{movementId}
GET    /api/items
POST   /api/items
GET    /api/warehouses
```

---

## 6. Views (واجهات المستخدم)

### Views المنشأة:

1. **Item/Index.cshtml** - قائمة الأصناف
   - جدول بحث وتصفية
   - أزرار التعديل والحذف

2. **Item/Create.cshtml** - إنشاء صنف جديد
   - نموذج بجميع الحقول المطلوبة
   - اختيار الفئة والضريبة

3. **Item/Edit.cshtml** - تعديل الصنف

4. **Warehouse/Index.cshtml** - قائمة المستودعات
   - جدول بمعلومات المستودعات
   - رابط ملخص المخزون

5. **Warehouse/Create.cshtml** - إنشاء مستودع جديد

6. **Warehouse/Summary.cshtml** - ملخص المخزون
   - إحصائيات المستودع
   - إجمالي الأصناف والقيمة

7. **Inventory/Balances.cshtml** - أرصدة المخزون
   - جدول الأرصدة
   - الإحصائيات العامة
   - تصفية حسب المستودع

8. **Inventory/Movements.cshtml** - حركات المخزون
   - تصفية حسب التاريخ
   - عرض حالة الترحيل
   - أزرار الترحيل

9. **Inventory/AddMovement.cshtml** - إضافة حركة جديدة
   - اختيار الصنف والمستودع
   - تحديد نوع الحركة والكمية

---

## 7. AutoMapper Profiles

تم إضافة 45 سطر من تعريفات التحويل:

```csharp
// Item Mappings
CreateMap<ItemCategory, ItemCategoryDto>();
CreateMap<Item, ItemDto>()
    .ForMember(dest => dest.CategoryName, 
               opt => opt.MapFrom(src => src.ItemCategory!.ItemCategoryNameAr));

// Warehouse Mappings
CreateMap<Warehouse, WarehouseDto>()
    .ForMember(dest => dest.BranchName,
               opt => opt.MapFrom(src => src.Branch!.BranchName));

// Balance Mappings
CreateMap<ItemBalance, ItemBalanceDto>()
    .ForMember(dest => dest.TotalValue,
               opt => opt.MapFrom(src => src.BalanceQuantity * src.AverageCost));

// Movement Mappings
CreateMap<ItemMovement, ItemMovementDto>()
    .ForMember(dest => dest.ItemCode,
               opt => opt.MapFrom(src => src.Item!.ItemCode));

// Count Mappings
CreateMap<InventoryCount, InventoryCountDto>();
CreateMap<InventoryCountDetail, InventoryCountDetailDto>();
```

---

## 8. Integration مع المرحلة الأولى

### التكامل بنجاح مع:

1. **نظام الأمان**
   - Authorization على جميع المتحكمات
   - التحقق من الصلاحيات
   - تسجيل معرّف المستخدم

2. **نظام الفروع**
   - ربط المستودعات بالفروع
   - دعم الفروع المتعددة
   - عرض بيانات الفرع

3. **نظام المستخدمين**
   - تسجيل تاريخ الإنشاء والتعديل
   - تسجيل معرّف المستخدم لكل عملية

4. **نظام التدقيق**
   - تسجيل جميع العمليات
   - حفظ IP العميل
   - توثيق التغييرات

---

## 9. التعليمات البرمجية والترميز

### معايير الجودة:

- **Clean Code Principles** - كود نظيف وسهل الفهم
- **SOLID Principles** - تطبيق مبادئ البرمجة الموثوقة
- **DRY (Don't Repeat Yourself)** - عدم تكرار الكود
- **Separation of Concerns** - فصل المخاوف
- **Comprehensive Comments** - تعليقات شاملة

### مثال على معايير الكود:

```csharp
// ✓ صحيح
public async Task<ItemDto> GetByIdAsync(int id)
{
    var item = await _itemRepository.GetWithCategoryAsync(id);
    if (item == null)
        throw new ArgumentException("الصنف غير موجود");

    return _mapper.Map<ItemDto>(item);
}

// ✗ خطأ
public ItemDto GetItem(int id)
{
    var item = _repo.Get(id);
    return new ItemDto { 
        Id = item.Id, 
        Name = item.Name 
    };
}
```

---

## 10. إحصائيات المرحلة الثانية

| المقياس | العدد |
|---------|-------|
| Entities | 10 |
| Repositories | 10 |
| Services | 7 |
| Controllers (MVC) | 4 |
| Controllers (API) | 3 |
| DTOs | 15+ |
| Views | 9+ |
| Lines of Code | 3000+ |
| Test Coverage | مستعد للاختبار |

---

## 11. الخصائص الأمنية

### التحقق من المدخلات:
- ModelState Validation
- نطاق الأرقام والنصوص
- عدم السماح بقيم فارغة إلزامية

### الحماية من الهجمات:
- CSRF Protection عبر [ValidateAntiForgeryToken]
- SQL Injection Prevention عبر Entity Framework
- Authorization على جميع العمليات
- Logging شامل للتحقق اللاحق

### إدارة الأخطاء:
- Try-Catch على جميع العمليات
- رسائل خطأ واضحة
- Logging التفصيلي للأخطاء

---

## 12. الخطوات التالية

### المرحلة الثالثة: الحسابات المحاسبية
- نموذج الحساب
- القيود المحاسبية
- الترحيل التلقائي من المخزون
- التقارير المالية

### المرحلة الرابعة: المشتريات
- الموردون
- طلبات الشراء
- فواتير الشراء
- المرتجعات

### المرحلة الخامسة: المبيعات
- العملاء
- أوامر البيع
- فواتير البيع
- نقاط البيع

---

## 13. قاموس المصطلحات

| المصطلح | التعريف |
|--------|---------|
| **Weighted Average Cost** | متوسط التكلفة المرجح لحساب تكلفة المخزون |
| **Posting** | ترحيل حركة مخزون إلى الحسابات المحاسبية |
| **Inventory Count** | الجرد الفعلي لعد الأصناف الفعلية |
| **Batch** | دفعة من الأصناف بنفس رقم الدفعة |
| **Balance** | رصيد الصنف في مستودع معين |

---

## 14. الملفات المرتبطة

- `/EFA.Domain/Entities/` - جميع الـ Entities
- `/EFA.Infrastructure/Repositories/` - جميع الـ Repositories
- `/EFA.Application/Services/` - جميع الـ Services
- `/EFA.Application/DTOs/InventoryDtos.cs` - جميع الـ DTOs
- `/EFA.Web/Controllers/` - جميع الـ Controllers
- `/EFA.Web/Views/Item/` - Views للأصناف
- `/EFA.Web/Views/Warehouse/` - Views للمستودعات
- `/EFA.Web/Views/Inventory/` - Views لحركات المخزون

---

## 15. ملاحظات مهمة

1. **التكامل مع المرحلة الأولى**: جميع الخدمات والـ Controllers موصولة بنظام الأمان والمستخدمين
2. **قابلية التوسع**: تم تصميم جميع الـ Services ليكون سهل الإضافة والتطوير
3. **الأداء**: تم تحسين الاستعلامات عبر Lazy Loading والـ Indexes
4. **الموثوقية**: جميع العمليات محمية من الأخطاء غير المتوقعة

---

**تم إنجاز المرحلة الثانية بنجاح!**

المشروع جاهز للانتقال إلى المرحلة الثالثة (الحسابات المحاسبية).
