const API_BASE = process.env.NEXT_PUBLIC_API_URL || "http://localhost:5000"

class ApiError extends Error {
  constructor(public status: number, message: string) {
    super(message)
  }
}

async function refreshToken(): Promise<string | null> {
  const token = localStorage.getItem("refreshToken")
  if (!token) return null
  try {
    const res = await fetch(`${API_BASE}/api/auth/refresh`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ token }),
    })
    if (!res.ok) return null
    const data = await res.json()
    localStorage.setItem("token", data.data.token)
    localStorage.setItem("refreshToken", data.data.refreshToken)
    return data.data.token
  } catch {
    return null
  }
}

export async function apiClient<T>(
  endpoint: string,
  options: RequestInit = {}
): Promise<T> {
  const token = localStorage.getItem("token")
  const headers: Record<string, string> = {
    "Content-Type": "application/json",
    ...(options.headers as Record<string, string>),
  }
  if (token) headers["Authorization"] = `Bearer ${token}`

  let res = await fetch(`${API_BASE}${endpoint}`, { ...options, headers })

  if (res.status === 401 && token) {
    const newToken = await refreshToken()
    if (newToken) {
      headers["Authorization"] = `Bearer ${newToken}`
      res = await fetch(`${API_BASE}${endpoint}`, { ...options, headers })
    }
  }

  if (!res.ok) {
    const body = await res.json().catch(() => ({}))
    throw new ApiError(res.status, body.message || body.title || "Request failed")
  }

  return res.json()
}

export async function login(email: string, password: string) {
  const res = await fetch(`${API_BASE}/api/auth/login`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ username: email, password }),
  })
  const data = await res.json()
  if (!data.success) throw new ApiError(401, data.message)
  localStorage.setItem("token", data.data.token)
  localStorage.setItem("refreshToken", data.data.refreshToken)
  localStorage.setItem("user", JSON.stringify(data.data.user))
  return data.data
}

export function logout() {
  localStorage.removeItem("token")
  localStorage.removeItem("refreshToken")
  localStorage.removeItem("user")
  window.location.href = "/login"
}

export function getToken() {
  if (typeof window === "undefined") return null
  return localStorage.getItem("token")
}

export function isAuthenticated() {
  return !!getToken()
}
