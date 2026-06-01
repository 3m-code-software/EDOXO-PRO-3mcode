"use client"

import { useState, useEffect } from "react"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { BarChart3, Menu } from "lucide-react"
import { apiClient } from "@/lib/api-client"

interface AnnualDataPoint {
  month: string
  salesAmount: number
  purchaseAmount: number
  profit: number
}

export function AnnualChart() {
  const [data, setData] = useState<AnnualDataPoint[]>([])
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    apiClient<{ data: AnnualDataPoint[] }>("/api/dashboard/annual-chart")
      .then((res) => setData(res.data))
      .catch(() => {})
      .finally(() => setLoading(false))
  }, [])

  const maxValue = Math.max(...data.map((d) => d.salesAmount), 1)

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
          <CardTitle className="text-base">السنة المالية الحالية</CardTitle>
          <BarChart3 className="w-5 h-5 text-blue-600" />
        </div>
      </CardHeader>
      <CardContent>
        <div className="h-48 relative">
          <div className="absolute left-0 top-0 text-xs text-gray-400">أعلى المبيعات EGP</div>
          <div className="absolute left-0 bottom-0 text-xs text-gray-400">0</div>
          <div className="flex items-end justify-between h-full pt-6 pb-4 px-4">
            {loading
              ? Array.from({ length: 12 }).map((_, i) => (
                  <div key={i} className="w-3 bg-gray-200 rounded-full animate-pulse h-4" />
                ))
              : data.map((point, index) => {
                  const height = maxValue > 0 ? Math.max((point.salesAmount / maxValue) * 180, 4) : 4
                  return (
                    <div key={index} className="flex flex-col items-center gap-1">
                      <div
                        className="w-3 bg-blue-500 rounded-full transition-all duration-500"
                        style={{ height: `${height}px` }}
                      />
                    </div>
                  )
                })}
          </div>
          <div className="absolute bottom-0 left-0 right-0 border-t border-dashed border-gray-200"></div>
        </div>
        <div className="flex justify-between text-xs text-gray-400 mt-2 px-2">
          {data.length > 0
            ? data.map((point, i) => (
                <span key={i} className="text-[10px]">
                  {point.month}
                </span>
              ))
            : Array.from({ length: 12 }).map((_, i) => (
                <span key={i} className="text-[10px] text-gray-300">
                  --
                </span>
              ))}
        </div>
      </CardContent>
    </Card>
  )
}
