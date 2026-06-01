"use client"

import { useState, useEffect } from "react"
import { useRouter } from "next/navigation"
import { useAuth } from "@/lib/auth-context"
import { Header } from "@/components/header"
import { Sidebar } from "@/components/sidebar"
import { WelcomeSection } from "@/components/welcome-section"
import { QuickAccess } from "@/components/quick-access"
import { AlertBanner } from "@/components/alert-banner"
import { FinancialSummary } from "@/components/financial-summary"
import { SalesChart } from "@/components/sales-chart"
import { AnnualChart } from "@/components/annual-chart"
import { PaymentTables } from "@/components/payment-tables"
import { InventoryAlert } from "@/components/inventory-alert"
import { SalesOrders } from "@/components/sales-orders"
import { PendingShipments } from "@/components/pending-shipments"
import { CalculatorModal } from "@/components/calculator-modal"
import { ProfitModal } from "@/components/profit-modal"

export default function DashboardPage() {
  const { isAuthenticated, loading, user, logout } = useAuth()
  const router = useRouter()
  const [sidebarCollapsed, setSidebarCollapsed] = useState(false)
  const [calculatorOpen, setCalculatorOpen] = useState(false)
  const [profitOpen, setProfitOpen] = useState(false)

  useEffect(() => {
    if (!loading && !isAuthenticated) router.push("/login")
  }, [loading, isAuthenticated, router])

  if (loading || !isAuthenticated) return null

  return (
    <div className="min-h-screen bg-gray-100" dir="rtl">
      <Header
        onToggleSidebar={() => setSidebarCollapsed(!sidebarCollapsed)}
        onOpenCalculator={() => setCalculatorOpen(true)}
        onOpenProfit={() => setProfitOpen(true)}
        onLogout={logout}
        user={user}
      />
      <div className="flex">
        <Sidebar collapsed={sidebarCollapsed} onLogout={logout} />
        <main className="flex-1 p-6 overflow-auto">
          <WelcomeSection user={user} />
          <AlertBanner />
          <QuickAccess />
          <FinancialSummary />
          <SalesChart />
          <AnnualChart />
          <PaymentTables />
          <InventoryAlert />
          <SalesOrders />
          <PendingShipments />
          <footer className="text-center text-sm text-gray-500 py-4 mt-4">
            EDOXO PRO | Cloud ERP, Accounting, Sales, Inventory Software - V9.3 | Copyright © 2025 All rights reserved
          </footer>
        </main>
      </div>
      <CalculatorModal open={calculatorOpen} onClose={() => setCalculatorOpen(false)} />
      <ProfitModal open={profitOpen} onClose={() => setProfitOpen(false)} />
    </div>
  )
}
