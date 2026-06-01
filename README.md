# EDOXO PRO

**Cloud ERP System** — Full-stack ERP with ASP.NET Core 9 Backend + Next.js 16 Frontend

---

## 📋 Overview

EDOXO PRO is a comprehensive ERP (Enterprise Resource Planning) system covering:

- **Sales Management** (invoices, quotes, drafts, returns)
- **Purchase Management** (orders, returns, receiving)
- **Inventory Management** (stock transfers, audits, damaged stock)
- **Contact Management** (customers, suppliers, delegates, groups)
- **Product Management** (categories, brands, units, variants, barcodes)
- **Financial** (payments, checks, expenses, profit/loss reports)
- **User Management** (roles, permissions, users)
- **Dashboard & Reports** (charts, KPIs, alerts)
- **Settings** (company info, invoices, barcodes, branches)

---

## 🏗 Architecture

### Clean Architecture (4 Layers)

```
┌─────────────────────────────────────┐
│         EdoxoPro.Api                │  ← API Layer (Controllers, Middleware)
├─────────────────────────────────────┤
│      EdoxoPro.Application           │  ← Application Layer (Services, DTOs)
├─────────────────────────────────────┤
│     EdoxoPro.Infrastructure         │  ← Infrastructure Layer (EF Core, Identity)
├─────────────────────────────────────┤
│        EdoxoPro.Domain              │  ← Domain Layer (Entities, Enums)
└─────────────────────────────────────┘
```

### Tech Stack

| Layer | Technology |
|-------|-----------|
| **Backend** | .NET 9, C# 12 |
| **API** | ASP.NET Core, JWT Bearer, Swagger |
| **ORM** | Entity Framework Core 9, SQL Server |
| **Auth** | ASP.NET Core Identity + JWT |
| **Mapping** | AutoMapper |
| **Validation** | FluentValidation |
| **Logging** | Serilog |
| **Frontend** | Next.js 16, React 19, TypeScript |
| **Styling** | Tailwind CSS v4, shadcn/ui |
| **Charts** | Recharts |
| **Forms** | React Hook Form + Zod |

---

## 🚀 Getting Started

### Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
- [Node.js 20+](https://nodejs.org/)
- [pnpm](https://pnpm.io/) (or npm)

---

### Backend Setup

#### 1. Clone & Restore

```bash
git clone https://github.com/3m-code-software/EDOXO-PRO-3mcode.git
cd backend
dotnet restore
```

#### 2. Configure Connection String

Edit `src/EdoxoPro.Api/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=EdoxoProDb;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Jwt": {
    "Secret": "your-super-secret-key-at-least-32-characters-long!!",
    "Issuer": "EdoxoPro",
    "Audience": "EdoxoProClients",
    "ExpiryInMinutes": 60
  }
}
```

#### 3. Apply Migrations & Run

```bash
# Option A: Auto-migration on startup (DatabaseSeeder handles initial seed)
dotnet run --project src/EdoxoPro.Api

# Option B: Manual migration
dotnet ef database update --project src/EdoxoPro.Infrastructure --startup-project src/EdoxoPro.Api
dotnet run --project src/EdoxoPro.Api
```

**API runs at:** `http://localhost:5062`  
**Swagger UI:** `http://localhost:5062/swagger`

---

### Frontend Setup

```bash
cd ..  # back to root
pnpm install
pnpm dev
```

**Runs at:** `http://localhost:3000`

---

## 📁 Project Structure

```
EDOXO-PRO-3mcode/
├── backend/
│   ├── EdoxoPro.sln
│   └── src/
│       ├── EdoxoPro.Domain/         # 32 Entities, 10 Enums
│       │   ├── Entities/            # Product, Sale, Purchase, Customer, etc.
│       │   ├── Enums/               # SaleStatus, PaymentStatus, etc.
│       │   └── Events/              # Domain events
│       │
│       ├── EdoxoPro.Application/    # Business Logic
│       │   ├── Common/              # ApiResponse<T>, PagedResult<T>, FilterRequest
│       │   ├── DTOs/                # Request/Response DTOs (grouped by module)
│       │   ├── Interfaces/          # 29 Service interfaces
│       │   ├── Services/            # 28 Service implementations
│       │   ├── Mapping/             # AutoMapper profiles
│       │   └── Validators/          # FluentValidation rules
│       │
│       ├── EdoxoPro.Infrastructure/ # Data Access
│       │   ├── Data/
│       │   │   ├── AppDbContext.cs
│       │   │   ├── Configurations/  # 26 Entity type configurations
│       │   │   └── Seed/            # DatabaseSeeder (admin, roles, defaults)
│       │   ├── Identity/            # AppIdentityUser, AppIdentityRole
│       │   └── Repositories/        # GenericRepository<T>, ProductRepository, UserRepository
│       │
│       └── EdoxoPro.Api/           # REST API
│           ├── Controllers/         # 26 API Controllers
│           ├── Middleware/          # ExceptionMiddleware, RequestLoggingMiddleware
│           ├── Filters/             # PermissionFilter
│           └── Program.cs           # App bootstrap
│
├── app/                             # Next.js App Router
│   ├── layout.tsx                   # RTL Arabic root layout
│   ├── page.tsx                     # Dashboard overview
│   ├── contacts/                    # Suppliers, Customer Groups, Map
│   └── user-management/             # Delegates, Roles, Users
│
├── components/                      # React Components
│   ├── ui/                          # shadcn/ui primitives
│   ├── header.tsx, sidebar.tsx      # Layout
│   ├── sales-chart.tsx              # Charts (Recharts)
│   └── ...                          # Dashboard widgets
│
├── lib/                             # Utilities
├── public/                          # Static assets
└── styles/                          # Global styles
```

---

## 🌐 API Endpoints

| Module | Base URL | Endpoints |
|--------|----------|-----------|
| **Auth** | `/api/auth` | login, register, refresh, change-password, profile |
| **Dashboard** | `/api/dashboard` | summary, sales-chart, annual-chart, recent-orders, pending-shipments, inventory-alerts, payment-dues |
| **Products** | `/api/products` | CRUD + barcode lookup |
| **Brands** | `/api/brands` | CRUD |
| **Categories** | `/api/categories` | CRUD |
| **Units** | `/api/units` | CRUD |
| **Customers** | `/api/customers` | CRUD |
| **Suppliers** | `/api/suppliers` | CRUD + export |
| **Customer Groups** | `/api/customer-groups` | CRUD |
| **Delegates** | `/api/delegates` | CRUD |
| **Sales** | `/api/sales` | CRUD, approve, pay, drafts, quotes, export |
| **Sale Returns** | `/api/sales-returns` | CRUD |
| **Purchases** | `/api/purchases` | CRUD, receive |
| **Purchase Returns** | `/api/purchase-returns` | CRUD |
| **Stock Transfers** | `/api/stock-transfers` | CRUD, confirm |
| **Damaged Stock** | `/api/damaged-stock` | CRUD |
| **Inventory Audits** | `/api/inventory-audits` | CRUD, start, complete |
| **Expenses** | `/api/expenses` | CRUD |
| **Expense Categories** | `/api/expense-categories` | CRUD |
| **Checks** | `/api/checks` | CRUD, update-status |
| **Reports** | `/api/reports` | profit-loss, sales, inventory, top-selling |
| **Users** | `/api/users` | CRUD, activate |
| **Roles** | `/api/roles` | CRUD |
| **Notifications** | `/api/notifications` | list, mark-read, mark-all-read |
| **Settings** | `/api/settings` | company, invoice, barcode, branches |
| **Branches** | `/api/branches` | CRUD |

All endpoints (except auth login/register/refresh) require **JWT Bearer token**.

---

## 🔐 Authentication

### Default Admin Account (seeded)

| Field | Value |
|-------|-------|
| Email | `admin@edoxopro.com` |
| Password | `Admin@123` |

### Login

```bash
curl -X POST http://localhost:5062/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@edoxopro.com","password":"Admin@123"}'
```

Returns JWT token to use in subsequent requests:

```bash
curl -X GET http://localhost:5062/api/dashboard/summary \
  -H "Authorization: Bearer <token>"
```

---

## 🔧 Configuration

### appsettings.json (`backend/src/EdoxoPro.Api/`)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=EdoxoProDb;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Jwt": {
    "Secret": "your-32-char-min-secret",
    "Issuer": "EdoxoPro",
    "Audience": "EdoxoProClients",
    "ExpiryInMinutes": 60
  }
}
```

---

## 🛠 Development

### Build Backend

```bash
cd backend
dotnet build
```

### Add Migration

```bash
dotnet ef migrations add MigrationName \
  --project src/EdoxoPro.Infrastructure \
  --startup-project src/EdoxoPro.Api
```

### Run Frontend

```bash
pnpm dev
```

---

## 📜 License

Copyright © 2025 Zoftar. All rights reserved.
