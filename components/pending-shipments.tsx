"use client"

import { useState, useEffect } from "react"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select"
import { Download, Printer, FileSpreadsheet, Eye, Truck } from "lucide-react"
import { apiClient } from "@/lib/api-client"

interface PendingShipment {
  id: number
  invoiceNumber: string
  customerName: string
  total: number
  date: string
}

export function PendingShipments() {
  const [shipments, setShipments] = useState<PendingShipment[]>([])
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    apiClient<{ data: PendingShipment[] }>("/api/dashboard/pending-shipments")
      .then((res) => setShipments(res.data))
      .catch(() => {})
      .finally(() => setLoading(false))
  }, [])

  const formatDate = (d: string) => {
    const date = new Date(d)
    return date.toLocaleDateString("en-GB")
  }

  return (
    <Card className="mb-6 border-t-4 border-t-teal-400">
      <CardHeader className="flex flex-row items-center justify-between">
        <div className="flex items-center gap-2">
          <Input placeholder="بحث..." className="w-32 h-8 text-xs" />
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
          <CardTitle className="text-base">الشحنات المعلقة</CardTitle>
          <Truck className="w-5 h-5 text-teal-600" />
        </div>
      </CardHeader>
      <CardContent>
        <table className="w-full text-sm">
          <thead>
            <tr className="border-b text-right">
              <th className="p-2 font-medium text-gray-600">رقم الفاتورة</th>
              <th className="p-2 font-medium text-gray-600">العميل</th>
              <th className="p-2 font-medium text-gray-600">التاريخ</th>
              <th className="p-2 font-medium text-gray-600">المبلغ</th>
            </tr>
          </thead>
          <tbody>
            {loading ? (
              <tr>
                <td colSpan={4} className="text-center py-8">
                  <div className="flex justify-center gap-1">
                    <div className="w-2 h-2 bg-blue-400 rounded-full animate-bounce" />
                    <div className="w-2 h-2 bg-blue-400 rounded-full animate-bounce" style={{ animationDelay: "0.1s" }} />
                    <div className="w-2 h-2 bg-blue-400 rounded-full animate-bounce" style={{ animationDelay: "0.2s" }} />
                  </div>
                </td>
              </tr>
            ) : shipments.length === 0 ? (
              <tr>
                <td colSpan={4} className="text-center py-8">
                  <div className="flex flex-col items-center gap-2">
                    <Truck className="w-8 h-8 text-gray-300" />
                    <span className="text-gray-400">لا توجد شحنات معلقة</span>
                  </div>
                </td>
              </tr>
            ) : (
              shipments.map((s) => (
                <tr key={s.id} className="border-b hover:bg-gray-50">
                  <td className="p-2 font-medium">{s.invoiceNumber}</td>
                  <td className="p-2">{s.customerName}</td>
                  <td className="p-2">{formatDate(s.date)}</td>
                  <td className="p-2">L.E {s.total.toFixed(2)}</td>
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
          <span>عرض 0 إلى 0 من {shipments.length} إدخالات</span>
        </div>
      </CardContent>
    </Card>
  )
}
