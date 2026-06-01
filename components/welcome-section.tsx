import { CalendarDays } from "lucide-react"

interface User {
  id: number
  username: string
  email: string
  fullName: string
  fullNameAr: string | null
  roles: string[]
}

export function WelcomeSection({ user }: { user: User | null }) {
  const displayName = user?.fullNameAr || user?.fullName || "مستخدم"
  const today = new Date()
  const days = ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"]
  const months = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"]
  const dateStr = `${days[today.getDay()]}, ${today.getDate()} ${months[today.getMonth()]} ${today.getFullYear()}`

  return (
    <div className="flex items-center justify-end gap-4 mb-6">
      <div className="text-right">
        <h1 className="text-xl font-semibold text-gray-800">مرحباً {displayName}</h1>
        <div className="flex items-center gap-2 text-sm text-gray-500">
          <CalendarDays className="w-4 h-4" />
          <span>التصفية حسب التاريخ</span>
        </div>
        <p className="text-sm text-gray-400">{dateStr}</p>
      </div>
      <div className="w-12 h-12 bg-gradient-to-br from-yellow-300 to-orange-400 rounded-full flex items-center justify-center">
        <span className="text-2xl">👋</span>
      </div>
    </div>
  )
}
