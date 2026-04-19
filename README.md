# 🅿️ Parking Lot Management API

A production-ready RESTful API for managing parking lots, built with **ASP.NET Core 8**, following **Onion Architecture** and real-world backend engineering practices.

---

## 🛠️ Tech Stack

| Layer | Technology |
|---|---|
| Framework | ASP.NET Core 8 Web API |
| Database | PostgreSQL + Entity Framework Core 8 |
| Caching | Redis (StackExchange.Redis) |
| Real-time | SignalR |
| Auth | JWT Bearer + ASP.NET Identity |
| Validation | FluentValidation |
| Docs | Swagger / OpenAPI |
| Architecture | Onion Architecture |

---

## ✅ Features

- **Ticket lifecycle** — Issue, Pay, and Exit flow with full state machine (Issued → Active → Paid → Closed)
- **Time-based pricing engine** — Hourly rate calculation with midnight-crossing support and minimum charge rules
- **Concurrency control** — Optimistic concurrency via EF Core RowVersion on ParkingSpot to prevent double-booking
- **Redis caching** — Floor occupancy cached and invalidated on every ticket operation
- **Real-time updates** — SignalR hub broadcasts floor availability changes to connected clients
- **Role-based access** — Admin and Attendant roles with JWT-secured endpoints
- **API Key middleware** — Global key guard for admin panel routes
- **Global exception handling** — Centralized error response middleware

---

## 🏗️ Architecture

```
src/
├── ParkingLot.API           → Controllers, Hubs, Middlewares
├── ParkingLot.Application   → Services, DTOs, Interfaces, Validators
├── ParkingLot.Domain        → Entities, Enums, Exceptions, Repository Interfaces
└── ParkingLot.Infrastructure → EF Core, Redis, Identity, JWT, Repositories
```

---

## 🚀 Getting Started

### Prerequisites
- .NET 8 SDK
- PostgreSQL
- Redis

### 1. Clone the repo
```bash
git clone https://github.com/KhanishAlasgarov/parking-lot-api.git
cd parking-lot-api
```

### 2. Configure environment

Create `src/ParkingLot.API/appsettings.Development.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=parkinglot_db;Username=postgres;Password=YOUR_PASSWORD"
  },
  "JwtSettings": {
    "Secret": "YOUR_SECRET_MIN_32_CHARS",
    "Issuer": "ParkingLotApi",
    "Audience": "ParkingLotClients",
    "ExpiryMinutes": 60
  },
  "ApiKeySettings": {
    "GlobalKey": "YOUR_API_KEY"
  },
  "Redis": {
    "ConnectionString": "localhost:6379"
  }
}
```

### 3. Apply migrations
```bash
cd src/ParkingLot.API
dotnet ef database update
```

### 4. Run
```bash
dotnet run
```

Swagger UI: `https://localhost:{port}/swagger`

---

## 📡 API Endpoints

### Auth
| Method | Endpoint | Description | Auth |
|---|---|---|---|
| POST | `/api/v1/auth/login` | Login, get JWT token | Public |

### Tickets
| Method | Endpoint | Description | Auth |
|---|---|---|---|
| POST | `/api/v1/tickets` | Issue a parking ticket | JWT |
| GET | `/api/v1/tickets/{number}` | Get ticket by number | JWT |
| POST | `/api/v1/tickets/{number}/pay` | Pay a ticket | JWT |
| POST | `/api/v1/tickets/{number}/exit` | Exit vehicle | JWT |

### Floors
| Method | Endpoint | Description | Auth |
|---|---|---|---|
| GET | `/api/v1/floors/{floorId}/availability` | Get real-time floor occupancy | JWT |

### Admin
| Method | Endpoint | Description | Auth |
|---|---|---|---|
| POST | `/api/v1/admin/floors` | Create a floor | Admin + API Key |
| POST | `/api/v1/admin/spots` | Create a parking spot | Admin + API Key |
| PUT | `/api/v1/admin/rates/{rateId}` | Update parking rate | Admin + API Key |
| POST | `/api/v1/admin/attendants` | Register attendant | Admin + API Key |
| DELETE | `/api/v1/admin/attendants/{id}` | Deactivate attendant | Admin + API Key |

### Real-time (SignalR)
| Hub | Event | Description |
|---|---|---|
| `/ws/occupancy` | `FloorUpdated` | Fires when floor occupancy changes |

---

## 🔐 Authentication Flow

1. Call `POST /api/v1/auth/login` → receive JWT token
2. Add header: `Authorization: Bearer {token}`
3. Admin routes additionally require: `X-Api-Key: {key}`

---

## 💡 Key Design Decisions

**Optimistic concurrency on spot booking** — Two attendants booking the same spot simultaneously triggers a `DbUpdateConcurrencyException`. The system retries up to 3 times before returning a `SpotNotAvailableException`.

**Pricing engine** — Rates are time-of-day based. Duration under 30 minutes is billed as 1 full hour. Multi-day stays are split at midnight and each segment is calculated independently.

**Cache-then-notify pattern** — After any ticket operation, Redis cache is invalidated and the updated occupancy is immediately pushed via SignalR to all connected clients.
