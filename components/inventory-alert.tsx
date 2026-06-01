"use client"

import { useState, useEffect } from "react"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select"
import { Download, Printer, FileSpreadsheet, Eye, Package, ArrowUpDown } from "lucide-react"
import { apiClient } from "@/lib/api-client"

interface AlertItem {
  id: number
  productName: string
  sku: string
  currentStock: number
  minStock: number
}

export function InventoryAlert() {
  const [items, setItems] = useState<AlertItem[]>([])
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    apiClient<{ data: AlertItem[] }>("/api/dashboard/inventory-alerts")
      .then((res) => setItems(res.data))
      .catch(() => {})
      .finally(() => setLoading(false))
  }, [])

  const columns = [
    { key: "productName", label: "منتج" },
    { key: "sku", label: "SKU" },
    { key: "currentStock", label: "المخزون الحالى" },
  ]

  return (
    <Card className="mb-6">
      <CardHeader className="flex flex-row items-center justify-between">
        <div className="flex items-center gap-2">
          <Select defaultValue="25">
            <SelectTrigger className="w-20 h-8 text-xs">
              <SelectValue />
            </SelectTrigger>
            <SelectContent>
              <SelectItem value="25">25</SelectItem>
              <SelectItem value="50">50</SelectItem>
              <SelectItem value="100">100</SelectItem>
              <SelectItem value="200">200</SelectItem>
              <SelectItem value="500">500</SelectItem>
              <SelectItem value="1000">1000</SelectItem>
              <SelectItem value="all">الكل</SelectItem>
            </SelectContent>
          </Select>
          <span className="text-xs text-gray-500">إدخالات</span>
          <span className="text-xs text-gray-500">عرض</span>
        </div>
        <div className="flex items-center gap-2">
          <Button variant="outline" size="sm" className="text-xs gap-1 bg-transparent">
            <Download className="w-3 h-3" />
            تصدير إلى CSV
          </Button>
          <Button variant="outline" size="sm" className="text-xs gap-1 bg-transparent">
            <FileSpreadsheet className="w-3 h-3" />
            تصدير إلى Excel
          </Button>
          <Button variant="outline" size="sm" className="text-xs gap-1 bg-transparent">
            <Printer className="w-3 h-3" />
            طباعة
          </Button>
          <Button variant="outline" size="sm" className="text-xs gap-1 bg-transparent">
            <Eye className="w-3 h-3" />
            رؤية العمود
          </Button>
        </div>
        <div className="flex items-center gap-2">
          <CardTitle className="text-base">تنبيه المخزون</CardTitle>
          <Package className="w-5 h-5 text-blue-600" />
        </div>
      </CardHeader>
      <CardContent>
        <table className="w-full text-sm">
          <thead>
            <tr className="border-b text-right">
              {columns.map((col) => (
                <th key={col.key} className="p-2 font-medium text-gray-600">
                  <div className="flex items-center gap-1 justify-end">
                    <ArrowUpDown className="w-3 h-3 text-gray-400" />
                    {col.label}
                  </div>
                </th>
              ))}
            </tr>
          </thead>
          <tbody>
            {loading ? (
              <tr>
                <td colSpan={3} className="text-center py-8">
                  <div className="flex justify-center gap-1">
                    <div className="w-2 h-2 bg-blue-400 rounded-full animate-bounce" />
                    <div className="w-2 h-2 bg-blue-400 rounded-full animate-bounce" style={{ animationDelay: "0.1s" }} />
                    <div className="w-2 h-2 bg-blue-400 rounded-full animate-bounce" style={{ animationDelay: "0.2s" }} />
                  </div>
                </td>
              </tr>
            ) : items.length === 0 ? (
              <tr>
                <td colSpan={3} className="text-center py-8">
                  <div className="flex flex-col items-center gap-2">
                    <Package className="w-8 h-8 text-gray-300" />
                    <span className="text-gray-400">لا توجد منتجات منخفضة المخزون</span>
                  </div>
                </td>
              </tr>
            ) : (
              items.map((item) => (
                <tr key={item.id} className="border-b hover:bg-gray-50">
                  <td className="p-2">{item.productName}</td>
                  <td className="p-2">{item.sku}</td>
                  <td className="p-2">
                    <span className="text-red-600 font-medium">{item.currentStock}</span>
                  </td>
                </tr>
              ))
            )}
          </tbody>
        </table>
        <div className="flex items-center justify-between mt-4 text-xs text-gray-500">
          <div className="flex gap-2">
            <button className="hover:text-blue-600 px-2 py-1 rounded border">السابق</button>
            <button className="hover:text-blue-600 px-2 py-1 rounded border">التالى</button>
          </div>
          <span>عرض 0 إلى 0 من {items.length} إدخالات</span>
        </div>
      </CardContent>
    </Card>
  )
}
