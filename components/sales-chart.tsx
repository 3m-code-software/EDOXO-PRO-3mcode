"use client"

import { useState, useEffect } from "react"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { BarChart3, Menu } from "lucide-react"
import { apiClient } from "@/lib/api-client"

interface ChartDataPoint {
  label: string
  value: number
}

export function SalesChart() {
  const [salesData, setSalesData] = useState<ChartDataPoint[]>([])
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    apiClient<{ data: { salesData: ChartDataPoint[] } }>("/api/dashboard/sales-chart?days=30")
      .then((res) => setSalesData(res.data.salesData))
      .catch(() => {})
      .finally(() => setLoading(false))
  }, [])

  const maxValue = Math.max(...salesData.map((d) => d.value), 1)

  const formatDate = (label: string) => {
    const d = new Date(label)
    const months = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"]
    return `${d.getDate()} ${months[d.getMonth()]}`
  }

  return (
    <Card className="mb-6">
      <CardHeader className="flex flex-row items-center justify-between">
        <div className="flex items-center gap-2">
          <Menu className="w-4 h-4 text-gray-400" />
          <div className="flex items-center gap-2">
            <div className="w-3 h-3 bg-blue-600 rounded-full"></div>
            <span className="text-sm text-gray-600">EDOXO PRO (BL0001)</span>
          </div>
        </div>
        <div className="flex items-center gap-2">
          <CardTitle className="text-base">المبيعات فى آخر 30 يوماً</CardTitle>
          <BarChart3 className="w-5 h-5 text-blue-600" />
        </div>
      </CardHeader>
      <CardContent>
        <div className="h-48 relative">
          <div className="absolute left-0 top-0 text-xs text-gray-400">أعلى المبيعات EGP</div>
          <div className="absolute left-0 bottom-0 text-xs text-gray-400">0</div>
          <div className="flex items-end justify-between h-full pt-6 pb-4">
            {loading
              ? Array.from({ length: 30 }).map((_, i) => (
                  <div key={i} className="w-2 bg-gray-200 rounded-full animate-pulse h-4" />
                ))
              : salesData.map((point, index) => {
                  const height = maxValue > 0 ? Math.max((point.value / maxValue) * 180, 4) : 4
                  return (
                    <div key={index} className="flex flex-col items-center gap-1">
                      <div
                        className="w-2 bg-blue-500 rounded-full transition-all duration-500"
                        style={{ height: `${height}px` }}
                      />
                    </div>
                  )
                })}
          </div>
          <div className="absolute bottom-0 left-0 right-0 border-t border-dashed border-gray-200"></div>
        </div>
        <div className="flex justify-between text-xs text-gray-400 mt-2 overflow-x-auto">
          {salesData.length > 0
            ? salesData.map((point, i) => (
                <span key={i} className="transform -rotate-45 whitespace-nowrap text-[10px]">
                  {formatDate(point.label)}
                </span>
              ))
            : Array.from({ length: 30 }).map((_, i) => (
                <span key={i} className="text-[10px] text-gray-300">
                  --
                </span>
              ))}
        </div>
      </CardContent>
    </Card>
  )
}
