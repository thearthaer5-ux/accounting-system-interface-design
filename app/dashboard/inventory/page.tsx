import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs'
import { Button } from '@/components/ui/button'
import { Plus, Package, Warehouse, TrendingDown, Eye } from 'lucide-react'
import Link from 'next/link'

export default function InventoryPage() {
  const items = [
    { id: 1, code: 'ITM001', name: 'منتج 1', category: 'فئة 1', quantity: 100, cost: 50, price: 75 },
    { id: 2, code: 'ITM002', name: 'منتج 2', category: 'فئة 2', quantity: 50, cost: 100, price: 150 },
    { id: 3, code: 'ITM003', name: 'منتج 3', category: 'فئة 1', quantity: 75, cost: 30, price: 45 },
  ]

  const warehouses = [
    { id: 1, name: 'المستودع الرئيسي', capacity: 1000, used: 225, location: 'الفرع الأساسي' },
    { id: 2, name: 'مستودع الفرع الثاني', capacity: 500, used: 150, location: 'الفرع الثاني' },
    { id: 3, name: 'مستودع المبيعات', capacity: 300, used: 100, location: 'قسم المبيعات' },
  ]

  const movements = [
    { id: 1, item: 'منتج 1', type: 'Inbound', quantity: 50, date: '2026-01-15', reference: 'PO001' },
    { id: 2, item: 'منتج 2', type: 'Outbound', quantity: 25, date: '2026-01-14', reference: 'SO001' },
    { id: 3, item: 'منتج 3', type: 'Transfer', quantity: 10, date: '2026-01-13', reference: 'TR001' },
  ]

  const getMovementColor = (type: string) => {
    switch (type) {
      case 'Inbound':
        return 'bg-green-900 text-green-200'
      case 'Outbound':
        return 'bg-red-900 text-red-200'
      case 'Transfer':
        return 'bg-blue-900 text-blue-200'
      default:
        return 'bg-gray-900 text-gray-200'
    }
  }

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-900 via-slate-800 to-slate-900 p-6">
      <div className="mx-auto max-w-7xl">
        {/* Header */}
        <div className="mb-8 flex items-center justify-between">
          <div>
            <h1 className="text-3xl font-bold text-white mb-2">
              إدارة المخزون
            </h1>
            <p className="text-slate-400">
              المواد، المستودعات، والحركات
            </p>
          </div>
          <Link href="/dashboard">
            <Button variant="outline">العودة</Button>
          </Link>
        </div>

        {/* Stats */}
        <div className="grid grid-cols-1 md:grid-cols-4 gap-4 mb-8">
          <Card className="bg-slate-800 border-slate-700">
            <CardHeader className="pb-3">
              <CardTitle className="text-sm text-slate-300">إجمالي المواد</CardTitle>
            </CardHeader>
            <CardContent>
              <div className="text-3xl font-bold text-white">3</div>
              <p className="text-xs text-slate-400 mt-1">صنف في المخزون</p>
            </CardContent>
          </Card>

          <Card className="bg-slate-800 border-slate-700">
            <CardHeader className="pb-3">
              <CardTitle className="text-sm text-slate-300">المستودعات</CardTitle>
            </CardHeader>
            <CardContent>
              <div className="text-3xl font-bold text-white">3</div>
              <p className="text-xs text-slate-400 mt-1">مستودع نشط</p>
            </CardContent>
          </Card>

          <Card className="bg-slate-800 border-slate-700">
            <CardHeader className="pb-3">
              <CardTitle className="text-sm text-slate-300">استخدام السعة</CardTitle>
            </CardHeader>
            <CardContent>
              <div className="text-3xl font-bold text-white">475</div>
              <p className="text-xs text-slate-400 mt-1">من 1800 وحدة</p>
            </CardContent>
          </Card>

          <Card className="bg-slate-800 border-slate-700">
            <CardHeader className="pb-3">
              <CardTitle className="text-sm text-slate-300">قيمة المخزون</CardTitle>
            </CardHeader>
            <CardContent>
              <div className="text-3xl font-bold text-white">22,250</div>
              <p className="text-xs text-slate-400 mt-1">بالتكلفة</p>
            </CardContent>
          </Card>
        </div>

        {/* Main Content */}
        <Tabs defaultValue="items" className="w-full">
          <TabsList className="bg-slate-800 border-slate-700">
            <TabsTrigger value="items">المواد</TabsTrigger>
            <TabsTrigger value="warehouses">المستودعات</TabsTrigger>
            <TabsTrigger value="movements">الحركات</TabsTrigger>
            <TabsTrigger value="inventory">جرد المخزون</TabsTrigger>
          </TabsList>

          {/* Items Tab */}
          <TabsContent value="items" className="mt-6">
            <Card className="bg-slate-800 border-slate-700">
              <CardHeader>
                <div className="flex items-center justify-between">
                  <div>
                    <CardTitle>المواد والمنتجات</CardTitle>
                    <CardDescription>قائمة المواد المخزنة</CardDescription>
                  </div>
                  <Button className="bg-green-600 hover:bg-green-700">
                    <Plus className="w-4 h-4 mr-2" />
                    إضافة مادة
                  </Button>
                </div>
              </CardHeader>
              <CardContent>
                <div className="overflow-x-auto">
                  <table className="w-full text-sm">
                    <thead className="bg-slate-700">
                      <tr>
                        <th className="px-4 py-3 text-right text-slate-300">الكود</th>
                        <th className="px-4 py-3 text-right text-slate-300">الاسم</th>
                        <th className="px-4 py-3 text-right text-slate-300">الفئة</th>
                        <th className="px-4 py-3 text-right text-slate-300">الكمية</th>
                        <th className="px-4 py-3 text-right text-slate-300">التكلفة</th>
                        <th className="px-4 py-3 text-right text-slate-300">السعر</th>
                        <th className="px-4 py-3 text-right text-slate-300">الإجراءات</th>
                      </tr>
                    </thead>
                    <tbody>
                      {items.map((item) => (
                        <tr key={item.id} className="border-t border-slate-700 hover:bg-slate-700/50">
                          <td className="px-4 py-3 text-slate-300 font-mono">{item.code}</td>
                          <td className="px-4 py-3 text-slate-300">{item.name}</td>
                          <td className="px-4 py-3 text-slate-300">{item.category}</td>
                          <td className="px-4 py-3 text-slate-300">{item.quantity}</td>
                          <td className="px-4 py-3 text-slate-300">{item.cost}</td>
                          <td className="px-4 py-3 text-slate-300">{item.price}</td>
                          <td className="px-4 py-3">
                            <button className="hover:text-slate-100">
                              <Eye className="w-4 h-4" />
                            </button>
                          </td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>
              </CardContent>
            </Card>
          </TabsContent>

          {/* Warehouses Tab */}
          <TabsContent value="warehouses" className="mt-6">
            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
              {warehouses.map((warehouse) => {
                const usagePercent = (warehouse.used / warehouse.capacity) * 100
                return (
                  <Card key={warehouse.id} className="bg-slate-800 border-slate-700">
                    <CardHeader>
                      <div className="flex items-center justify-between">
                        <div>
                          <CardTitle className="flex items-center gap-2">
                            <Warehouse className="w-4 h-4" />
                            {warehouse.name}
                          </CardTitle>
                          <CardDescription>{warehouse.location}</CardDescription>
                        </div>
                      </div>
                    </CardHeader>
                    <CardContent>
                      <div className="space-y-3">
                        <div>
                          <div className="flex justify-between text-sm mb-1">
                            <span className="text-slate-300">الاستخدام</span>
                            <span className="text-slate-300">
                              {warehouse.used} / {warehouse.capacity}
                            </span>
                          </div>
                          <div className="w-full bg-slate-700 rounded-full h-2">
                            <div
                              className={`h-2 rounded-full ${
                                usagePercent > 80
                                  ? 'bg-red-500'
                                  : usagePercent > 50
                                    ? 'bg-yellow-500'
                                    : 'bg-green-500'
                              }`}
                              style={{ width: `${usagePercent}%` }}
                            />
                          </div>
                        </div>
                        <p className="text-xs text-slate-400">
                          {usagePercent.toFixed(1)}% من السعة المتاحة
                        </p>
                      </div>
                    </CardContent>
                  </Card>
                )
              })}
            </div>
          </TabsContent>

          {/* Movements Tab */}
          <TabsContent value="movements" className="mt-6">
            <Card className="bg-slate-800 border-slate-700">
              <CardHeader>
                <div className="flex items-center justify-between">
                  <div>
                    <CardTitle>حركات المخزون</CardTitle>
                    <CardDescription>الحركات الأخيرة</CardDescription>
                  </div>
                  <Button className="bg-blue-600 hover:bg-blue-700">
                    <Plus className="w-4 h-4 mr-2" />
                    تسجيل حركة
                  </Button>
                </div>
              </CardHeader>
              <CardContent>
                <div className="overflow-x-auto">
                  <table className="w-full text-sm">
                    <thead className="bg-slate-700">
                      <tr>
                        <th className="px-4 py-3 text-right text-slate-300">المادة</th>
                        <th className="px-4 py-3 text-right text-slate-300">النوع</th>
                        <th className="px-4 py-3 text-right text-slate-300">الكمية</th>
                        <th className="px-4 py-3 text-right text-slate-300">التاريخ</th>
                        <th className="px-4 py-3 text-right text-slate-300">المرجع</th>
                      </tr>
                    </thead>
                    <tbody>
                      {movements.map((movement) => (
                        <tr key={movement.id} className="border-t border-slate-700 hover:bg-slate-700/50">
                          <td className="px-4 py-3 text-slate-300">{movement.item}</td>
                          <td className="px-4 py-3">
                            <span className={`px-2 py-1 rounded text-xs ${getMovementColor(movement.type)}`}>
                              {movement.type}
                            </span>
                          </td>
                          <td className="px-4 py-3 text-slate-300">{movement.quantity}</td>
                          <td className="px-4 py-3 text-slate-300">{movement.date}</td>
                          <td className="px-4 py-3 text-slate-300 font-mono">{movement.reference}</td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>
              </CardContent>
            </Card>
          </TabsContent>

          {/* Inventory Count Tab */}
          <TabsContent value="inventory" className="mt-6">
            <Card className="bg-slate-800 border-slate-700">
              <CardHeader>
                <div className="flex items-center justify-between">
                  <div>
                    <CardTitle>جرد المخزون</CardTitle>
                    <CardDescription>عمليات الجرد والمطابقة</CardDescription>
                  </div>
                  <Button className="bg-purple-600 hover:bg-purple-700">
                    <Plus className="w-4 h-4 mr-2" />
                    بدء جرد جديد
                  </Button>
                </div>
              </CardHeader>
              <CardContent>
                <div className="text-center py-8 text-slate-400">
                  <TrendingDown className="w-12 h-12 mx-auto mb-3 opacity-50" />
                  <p>لا توجد عمليات جرد حالياً</p>
                </div>
              </CardContent>
            </Card>
          </TabsContent>
        </Tabs>
      </div>
    </div>
  )
}
