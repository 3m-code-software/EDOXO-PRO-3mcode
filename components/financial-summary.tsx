"use client"

import { useState, useEffect } from "react"
import { TrendingUp, TrendingDown, RotateCcw, Wallet, FileText, ShoppingBag, Banknote } from "lucide-react"
import { apiClient } from "@/lib/api-client"

interface DashboardSummary {
  totalSales: number
  totalPurchases: number
  totalExpenses: number
  totalProducts: number
  totalCustomers: number
  totalSuppliers: number
  netProfit: number
  lowStockCount: number
}

export function FinancialSummary() {
  const [summary, setSummary] = useState<DashboardSummary | null>(null)
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    apiClient<{ data: DashboardSummary }>("/api/dashboard/summary")
      .then((res) => setSummary(res.data))
      .catch(() => {})
      .finally(() => setLoading(false))
  }, [])

  const formatCurrency = (value: number) => `L.E ${value.toLocaleString("en", { minimumFractionDigits: 2, maximumFractionDigits: 2 })}`

  if (loading) {
    return (
      <div className="mb-6">
        <div className="grid grid-cols-4 gap-4">
          {Array.from({ length: 4 }).map((_, i) => (
            <div key={i} className="bg-gray-200 rounded-lg p-4 animate-pulse h-24" />
          ))}
        </div>
      </div>
    )
  }

  const cards = [
    { icon: TrendingUp, label: "إجمالى المبيعات", value: formatCurrency(summary?.totalSales ?? 0), color: "bg-blue-600", iconBg: "bg-blue-500" },
    { icon: Banknote, label: "صافى الربح", value: formatCurrency(summary?.netProfit ?? 0), color: "bg-blue-600", iconBg: "bg-blue-500" },
    { icon: ShoppingBag, label: "إجمالى المشتريات", value: formatCurrency(summary?.totalPurchases ?? 0), color: "bg-teal-500", iconBg: "bg-teal-400" },
    { icon: TrendingDown, label: "مصروف", value: formatCurrency(summary?.totalExpenses ?? 0), color: "bg-white border", textColor: "text-gray-700", negative: true },
    { icon: Wallet, label: "المنتجات", value: `${summary?.totalProducts ?? 0}`, color: "bg-blue-500", iconBg: "bg-blue-400" },
    { icon: FileText, label: "العملاء", value: `${summary?.totalCustomers ?? 0}`, color: "bg-teal-500", iconBg: "bg-teal-400" },
    { icon: RotateCcw, label: "الموردين", value: `${summary?.totalSuppliers ?? 0}`, color: "bg-white border", textColor: "text-gray-700" },
    { icon: RotateCcw, label: "مخزون منخفض", value: `${summary?.lowStockCount ?? 0}`, color: "bg-white border", textColor: "text-gray-700" },
  ]

  return (
    <div className="mb-6">
      <div className="grid grid-cols-4 gap-4">
        {cards.map((card, index) => (
          <div
            key={index}
            className={`${card.color} ${card.textColor || "text-white"} rounded-lg p-4 flex items-center justify-between`}
          >
            <div className="text-right">
              <p className="text-sm opacity-90">{card.label}</p>
              <p className="text-lg font-bold">{card.value}</p>
            </div>
            <div className={`w-10 h-10 ${card.iconBg || "bg-gray-200"} rounded-full flex items-center justify-center`}>
              <card.icon
                className={`w-5 h-5 ${card.negative ? "text-red-500" : card.textColor ? "text-gray-500" : "text-white"}`}
              />
            </div>
          </div>
        ))}
      </div>
    </div>
  )
}
