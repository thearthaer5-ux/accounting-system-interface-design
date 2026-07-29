import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs'
import {
  BarChart3,
  Users,
  Package,
  ShoppingCart,
  ShoppingBag,
  Settings,
  FileText,
  TrendingUp,
} from 'lucide-react'
import Link from 'next/link'

const modules = [
  {
    title: 'Dashboard',
    description: 'لوحة التحكم الرئيسية',
    icon: BarChart3,
    href: '/dashboard',
    color: 'bg-blue-500',
  },
  {
    title: 'Accounting',
    description: 'إدارة المحاسبة والحسابات',
    icon: FileText,
    href: '/dashboard/accounting',
    color: 'bg-green-500',
  },
  {
    title: 'Inventory',
    description: 'إدارة المخزون والمواد',
    icon: Package,
    href: '/dashboard/inventory',
    color: 'bg-purple-500',
  },
  {
    title: 'Sales',
    description: 'إدارة المبيعات والعملاء',
    icon: ShoppingCart,
    href: '/dashboard/sales',
    color: 'bg-orange-500',
  },
  {
    title: 'Purchase',
    description: 'إدارة المشتريات والموردين',
    icon: ShoppingBag,
    href: '/dashboard/purchase',
    color: 'bg-red-500',
  },
  {
    title: 'Users',
    description: 'إدارة المستخدمين والصلاحيات',
    icon: Users,
    href: '/dashboard/users',
    color: 'bg-indigo-500',
  },
  {
    title: 'Reports',
    description: 'التقارير والإحصائيات',
    icon: TrendingUp,
    href: '/dashboard/reports',
    color: 'bg-pink-500',
  },
  {
    title: 'Settings',
    description: 'إعدادات النظام',
    icon: Settings,
    href: '/dashboard/settings',
    color: 'bg-gray-500',
  },
]

const stats = [
  { label: 'إجمالي العملاء', value: '0', subtext: 'عميل' },
  { label: 'إجمالي الموردين', value: '0', subtext: 'مورد' },
  { label: 'المواد في المخزون', value: '0', subtext: 'صنف' },
  { label: 'المستخدمون النشطون', value: '0', subtext: 'مستخدم' },
]

export default function DashboardPage() {
  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-900 via-slate-800 to-slate-900 p-6">
      <div className="mx-auto max-w-7xl">
        {/* Header */}
        <div className="mb-8">
          <h1 className="text-4xl font-bold text-white mb-2">
            نظام المحاسبة المتكامل
          </h1>
          <p className="text-slate-400">
            مرحباً بك في لوحة التحكم الرئيسية
          </p>
        </div>

        {/* Stats Grid */}
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
          {stats.map((stat, index) => (
            <Card key={index} className="bg-slate-800 border-slate-700">
              <CardHeader className="pb-3">
                <CardTitle className="text-sm font-medium text-slate-300">
                  {stat.label}
                </CardTitle>
              </CardHeader>
              <CardContent>
                <div className="text-3xl font-bold text-white">
                  {stat.value}
                </div>
                <p className="text-xs text-slate-400 mt-1">{stat.subtext}</p>
              </CardContent>
            </Card>
          ))}
        </div>

        {/* Main Modules Grid */}
        <Tabs defaultValue="overview" className="w-full">
          <TabsList className="bg-slate-800 border-slate-700">
            <TabsTrigger value="overview">نظرة عامة</TabsTrigger>
            <TabsTrigger value="modules">الوحدات</TabsTrigger>
          </TabsList>

          <TabsContent value="overview" className="mt-6">
            <Card className="bg-slate-800 border-slate-700">
              <CardHeader>
                <CardTitle>ملخص النظام</CardTitle>
                <CardDescription>
                  نظرة عامة على وحدات النظام والعمليات الرئيسية
                </CardDescription>
              </CardHeader>
              <CardContent>
                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <div className="p-4 bg-slate-700 rounded-lg">
                    <h3 className="font-semibold text-white mb-2">المحاسبة</h3>
                    <ul className="text-sm text-slate-300 space-y-1">
                      <li>• شجرة الحسابات</li>
                      <li>• اليوميات والقيود</li>
                      <li>• الأرصدة والتقارير</li>
                    </ul>
                  </div>
                  <div className="p-4 bg-slate-700 rounded-lg">
                    <h3 className="font-semibold text-white mb-2">المخزون</h3>
                    <ul className="text-sm text-slate-300 space-y-1">
                      <li>• المواد والفئات</li>
                      <li>• المستودعات</li>
                      <li>• الحركات والأرصدة</li>
                    </ul>
                  </div>
                  <div className="p-4 bg-slate-700 rounded-lg">
                    <h3 className="font-semibold text-white mb-2">المبيعات</h3>
                    <ul className="text-sm text-slate-300 space-y-1">
                      <li>• العملاء والعروض</li>
                      <li>• أوامر والفواتير</li>
                      <li>• السدادات والمرتجعات</li>
                    </ul>
                  </div>
                  <div className="p-4 bg-slate-700 rounded-lg">
                    <h3 className="font-semibold text-white mb-2">المشتريات</h3>
                    <ul className="text-sm text-slate-300 space-y-1">
                      <li>• الموردين والعروض</li>
                      <li>• أوامر الشراء</li>
                      <li>• الفواتير والسدادات</li>
                    </ul>
                  </div>
                </div>
              </CardContent>
            </Card>
          </TabsContent>

          <TabsContent value="modules" className="mt-6">
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
              {modules.map((module) => {
                const Icon = module.icon
                return (
                  <Link key={module.title} href={module.href}>
                    <Card className="bg-slate-800 border-slate-700 hover:border-slate-500 cursor-pointer transition-all hover:shadow-lg">
                      <CardHeader>
                        <div className={`w-10 h-10 ${module.color} rounded-lg flex items-center justify-center mb-3`}>
                          <Icon className="w-5 h-5 text-white" />
                        </div>
                        <CardTitle className="text-lg">
                          {module.title}
                        </CardTitle>
                        <CardDescription className="text-slate-400">
                          {module.description}
                        </CardDescription>
                      </CardHeader>
                      <CardContent>
                        <p className="text-xs text-slate-500">
                          اضغط للدخول إلى الوحدة
                        </p>
                      </CardContent>
                    </Card>
                  </Link>
                )
              })}
            </div>
          </TabsContent>
        </Tabs>

        {/* Recent Activity */}
        <Card className="bg-slate-800 border-slate-700 mt-8">
          <CardHeader>
            <CardTitle>النشاطات الأخيرة</CardTitle>
            <CardDescription>آخر التحديثات في النظام</CardDescription>
          </CardHeader>
          <CardContent>
            <div className="text-center py-8 text-slate-400">
              <p>لا توجد نشاطات حالياً</p>
            </div>
          </CardContent>
        </Card>
      </div>
    </div>
  )
}
