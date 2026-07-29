import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs'
import { Button } from '@/components/ui/button'
import {
  Plus,
  FileText,
  BarChart3,
  Settings,
  Eye,
} from 'lucide-react'
import Link from 'next/link'

export default function AccountingPage() {
  const chartOfAccounts = [
    { id: 1, number: '1000', name: 'الأصول الثابتة', type: 'Asset', balance: 0 },
    { id: 2, number: '1100', name: 'الأصول المتداولة', type: 'Asset', balance: 0 },
    { id: 3, number: '2000', name: 'الالتزامات', type: 'Liability', balance: 0 },
    { id: 4, number: '3000', name: 'حقوق المالكين', type: 'Equity', balance: 0 },
    { id: 5, number: '4000', name: 'الإيرادات', type: 'Income', balance: 0 },
    { id: 6, number: '5000', name: 'المصروفات', type: 'Expense', balance: 0 },
  ]

  const journals = [
    { id: 1, number: 'JNL001', type: 'General Journal', date: '2026-01-15', status: 'Posted', total: 0 },
    { id: 2, number: 'SJL001', type: 'Sales Journal', date: '2026-01-14', status: 'Draft', total: 0 },
    { id: 3, number: 'PJL001', type: 'Purchase Journal', date: '2026-01-13', status: 'Posted', total: 0 },
  ]

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'Posted':
        return 'bg-green-100 text-green-800'
      case 'Draft':
        return 'bg-yellow-100 text-yellow-800'
      case 'Reversed':
        return 'bg-red-100 text-red-800'
      default:
        return 'bg-gray-100 text-gray-800'
    }
  }

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-900 via-slate-800 to-slate-900 p-6">
      <div className="mx-auto max-w-7xl">
        {/* Header */}
        <div className="mb-8 flex items-center justify-between">
          <div>
            <h1 className="text-3xl font-bold text-white mb-2">
              إدارة المحاسبة
            </h1>
            <p className="text-slate-400">
              شجرة الحسابات، اليوميات، والتقارير المحاسبية
            </p>
          </div>
          <Link href="/dashboard">
            <Button variant="outline">العودة</Button>
          </Link>
        </div>

        {/* Quick Actions */}
        <div className="grid grid-cols-1 md:grid-cols-4 gap-4 mb-8">
          <Card className="bg-slate-800 border-slate-700 cursor-pointer hover:border-slate-500 transition-all">
            <CardHeader className="pb-3">
              <div className="flex items-center justify-between">
                <CardTitle className="text-sm font-medium text-slate-300">
                  إضافة حساب جديد
                </CardTitle>
                <Plus className="w-4 h-4 text-green-500" />
              </div>
            </CardHeader>
          </Card>

          <Card className="bg-slate-800 border-slate-700 cursor-pointer hover:border-slate-500 transition-all">
            <CardHeader className="pb-3">
              <div className="flex items-center justify-between">
                <CardTitle className="text-sm font-medium text-slate-300">
                  قيد يومية جديد
                </CardTitle>
                <FileText className="w-4 h-4 text-blue-500" />
              </div>
            </CardHeader>
          </Card>

          <Card className="bg-slate-800 border-slate-700 cursor-pointer hover:border-slate-500 transition-all">
            <CardHeader className="pb-3">
              <div className="flex items-center justify-between">
                <CardTitle className="text-sm font-medium text-slate-300">
                  تقرير الأستاذ
                </CardTitle>
                <BarChart3 className="w-4 h-4 text-purple-500" />
              </div>
            </CardHeader>
          </Card>

          <Card className="bg-slate-800 border-slate-700 cursor-pointer hover:border-slate-500 transition-all">
            <CardHeader className="pb-3">
              <div className="flex items-center justify-between">
                <CardTitle className="text-sm font-medium text-slate-300">
                  الفترات المحاسبية
                </CardTitle>
                <Settings className="w-4 h-4 text-orange-500" />
              </div>
            </CardHeader>
          </Card>
        </div>

        {/* Main Content */}
        <Tabs defaultValue="chart" className="w-full">
          <TabsList className="bg-slate-800 border-slate-700">
            <TabsTrigger value="chart">شجرة الحسابات</TabsTrigger>
            <TabsTrigger value="journals">اليوميات</TabsTrigger>
            <TabsTrigger value="balances">الأرصدة</TabsTrigger>
            <TabsTrigger value="reports">التقارير</TabsTrigger>
          </TabsList>

          {/* Chart of Accounts Tab */}
          <TabsContent value="chart" className="mt-6">
            <Card className="bg-slate-800 border-slate-700">
              <CardHeader>
                <div className="flex items-center justify-between">
                  <div>
                    <CardTitle>شجرة الحسابات</CardTitle>
                    <CardDescription>قائمة الحسابات الرئيسية والفرعية</CardDescription>
                  </div>
                  <Button className="bg-green-600 hover:bg-green-700">
                    <Plus className="w-4 h-4 mr-2" />
                    إضافة حساب
                  </Button>
                </div>
              </CardHeader>
              <CardContent>
                <div className="overflow-x-auto">
                  <table className="w-full text-sm">
                    <thead className="bg-slate-700">
                      <tr>
                        <th className="px-4 py-3 text-right text-slate-300">رقم الحساب</th>
                        <th className="px-4 py-3 text-right text-slate-300">اسم الحساب</th>
                        <th className="px-4 py-3 text-right text-slate-300">النوع</th>
                        <th className="px-4 py-3 text-right text-slate-300">الرصيد</th>
                        <th className="px-4 py-3 text-right text-slate-300">الإجراءات</th>
                      </tr>
                    </thead>
                    <tbody>
                      {chartOfAccounts.map((account) => (
                        <tr key={account.id} className="border-t border-slate-700 hover:bg-slate-700/50">
                          <td className="px-4 py-3 text-slate-300 font-mono">{account.number}</td>
                          <td className="px-4 py-3 text-slate-300">{account.name}</td>
                          <td className="px-4 py-3">
                            <span className="px-2 py-1 bg-blue-900 text-blue-200 rounded text-xs">
                              {account.type}
                            </span>
                          </td>
                          <td className="px-4 py-3 text-slate-300">{account.balance}</td>
                          <td className="px-4 py-3 text-slate-300">
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

          {/* Journals Tab */}
          <TabsContent value="journals" className="mt-6">
            <Card className="bg-slate-800 border-slate-700">
              <CardHeader>
                <div className="flex items-center justify-between">
                  <div>
                    <CardTitle>اليوميات</CardTitle>
                    <CardDescription>قائمة اليوميات والقيود</CardDescription>
                  </div>
                  <Button className="bg-blue-600 hover:bg-blue-700">
                    <Plus className="w-4 h-4 mr-2" />
                    قيد جديد
                  </Button>
                </div>
              </CardHeader>
              <CardContent>
                <div className="overflow-x-auto">
                  <table className="w-full text-sm">
                    <thead className="bg-slate-700">
                      <tr>
                        <th className="px-4 py-3 text-right text-slate-300">رقم اليومية</th>
                        <th className="px-4 py-3 text-right text-slate-300">النوع</th>
                        <th className="px-4 py-3 text-right text-slate-300">التاريخ</th>
                        <th className="px-4 py-3 text-right text-slate-300">الحالة</th>
                        <th className="px-4 py-3 text-right text-slate-300">الإجمالي</th>
                        <th className="px-4 py-3 text-right text-slate-300">الإجراءات</th>
                      </tr>
                    </thead>
                    <tbody>
                      {journals.map((journal) => (
                        <tr key={journal.id} className="border-t border-slate-700 hover:bg-slate-700/50">
                          <td className="px-4 py-3 text-slate-300 font-mono">{journal.number}</td>
                          <td className="px-4 py-3 text-slate-300">{journal.type}</td>
                          <td className="px-4 py-3 text-slate-300">{journal.date}</td>
                          <td className="px-4 py-3">
                            <span className={`px-2 py-1 rounded text-xs ${getStatusColor(journal.status)}`}>
                              {journal.status}
                            </span>
                          </td>
                          <td className="px-4 py-3 text-slate-300">{journal.total}</td>
                          <td className="px-4 py-3 text-slate-300">
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

          {/* Balances Tab */}
          <TabsContent value="balances" className="mt-6">
            <Card className="bg-slate-800 border-slate-700">
              <CardHeader>
                <CardTitle>أرصدة الحسابات</CardTitle>
                <CardDescription>أرصدة الحسابات للفترة المحاسبية الحالية</CardDescription>
              </CardHeader>
              <CardContent>
                <div className="text-center py-8 text-slate-400">
                  <p>لا توجد أرصدة محسوبة حالياً</p>
                </div>
              </CardContent>
            </Card>
          </TabsContent>

          {/* Reports Tab */}
          <TabsContent value="reports" className="mt-6">
            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
              <Card className="bg-slate-800 border-slate-700 cursor-pointer hover:border-slate-500">
                <CardHeader>
                  <CardTitle>تقرير الأستاذ</CardTitle>
                  <CardDescription>تفاصيل الحسابات</CardDescription>
                </CardHeader>
                <CardContent>
                  <Button className="w-full">عرض التقرير</Button>
                </CardContent>
              </Card>

              <Card className="bg-slate-800 border-slate-700 cursor-pointer hover:border-slate-500">
                <CardHeader>
                  <CardTitle>الميزانية العمومية</CardTitle>
                  <CardDescription>الأصول والالتزامات</CardDescription>
                </CardHeader>
                <CardContent>
                  <Button className="w-full">عرض التقرير</Button>
                </CardContent>
              </Card>

              <Card className="bg-slate-800 border-slate-700 cursor-pointer hover:border-slate-500">
                <CardHeader>
                  <CardTitle>قائمة الدخل</CardTitle>
                  <CardDescription>الإيرادات والمصروفات</CardDescription>
                </CardHeader>
                <CardContent>
                  <Button className="w-full">عرض التقرير</Button>
                </CardContent>
              </Card>

              <Card className="bg-slate-800 border-slate-700 cursor-pointer hover:border-slate-500">
                <CardHeader>
                  <CardTitle>تقرير الأرصدة التجريبية</CardTitle>
                  <CardDescription>التحقق من التوازن</CardDescription>
                </CardHeader>
                <CardContent>
                  <Button className="w-full">عرض التقرير</Button>
                </CardContent>
              </Card>
            </div>
          </TabsContent>
        </Tabs>
      </div>
    </div>
  )
}
