# 🥗 FoodShare — Food Donation Platform

A full-featured ASP.NET Core MVC application for community food sharing with role-based access control.

---

## 🚀 Quick Setup

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Any IDE: Visual Studio 2022, VS Code, or Rider

### Run the App

```bash
cd FoodDonationPlatform
dotnet restore
dotnet run
```

Open your browser at **https://localhost:5001** or **http://localhost:5000**

The database is automatically created and seeded on first run.

---

## 🔑 Demo Accounts

| Role       | Email                       | Password      |
|------------|-----------------------------|---------------|
| **Admin**  | admin@fooddonation.com      | Admin@123456  |
| **LocalUser** | user@fooddonation.com    | User@123456   |

---

## 🏗️ Project Structure

```
FoodDonationPlatform/
├── Controllers/
│   ├── AccountController.cs      # Login, Register, Logout
│   ├── HomeController.cs         # Dashboard (role-aware)
│   ├── FoodListingsController.cs # CRUD for food listings
│   └── OrdersController.cs       # Order management
│
├── Models/
│   ├── Models.cs                 # ApplicationUser, FoodListing, Order + enums
│   └── ViewModels.cs             # All ViewModels
│
├── Data/
│   ├── ApplicationDbContext.cs   # EF Core DbContext
│   └── DbSeeder.cs               # Seeds roles, users, and sample data
│
├── Views/
│   ├── Shared/_Layout.cshtml     # Main layout with navbar
│   ├── Home/                     # Landing, AdminDashboard, UserDashboard
│   ├── Account/                  # Login, Register, AccessDenied
│   ├── FoodListings/             # Index, Browse, Create, Edit, Details
│   └── Orders/                   # PlaceOrder, UserOrders, AdminOrders, Details
│
├── wwwroot/
│   ├── css/site.css              # Custom styles (Bootstrap-based)
│   └── js/site.js                # Client-side enhancements
│
├── Migrations/                   # EF Core migration (auto-applied)
├── Program.cs                    # App entry point + DI setup
└── appsettings.json              # SQLite connection string
```

---

## 🎯 Features by Role

### 👨‍💼 Admin (Donor) Role
- ✅ Post food donations with full details
- ✅ View & manage own listings (edit, delete, change status)
- ✅ View all incoming orders on their listings
- ✅ Update order status: Pending → Confirmed → Ready → Completed
- ✅ Admin dashboard with stats and recent activity
- ❌ Cannot place orders on food items

### 👤 Local User Role
- ✅ Browse all available food listings with search & filters
- ✅ Place orders on available food items
- ✅ Track order status with visual progress bar
- ✅ Cancel pending orders
- ✅ Also donate food (same posting functionality as Admin)
- ✅ User dashboard with personalized feed

---

## 🗄️ Database Schema

### Tables
| Table | Description |
|-------|-------------|
| `AspNetUsers` | Extended Identity users (FullName, Address) |
| `AspNetRoles` | Roles: Admin, LocalUser |
| `FoodListings` | Food donations with status, expiry, location |
| `Orders` | Order records linking users to food listings |

### Food Listing Status Flow
```
Available → PartiallyReserved → FullyReserved → Completed
         ↘ Cancelled
         ↘ Expired (computed)
```

### Order Status Flow
```
Pending → Confirmed → ReadyForPickup → Completed
        ↘ Cancelled (by user or donor)
```

---

## 🔒 Security Features

- ASP.NET Core Identity for authentication
- Role-based authorization (`[Authorize(Roles = "Admin")]`)
- Anti-forgery tokens on all POST forms
- Server-side ownership checks (users can only edit/delete their own content)
- Data annotations for server-side validation
- Input validation on all forms

---

## 🛠️ Technology Stack

| Component | Technology |
|-----------|-----------|
| Framework | ASP.NET Core 8 MVC |
| ORM | Entity Framework Core 8 |
| Database | SQLite (swap to SQL Server by changing connection string) |
| Auth | ASP.NET Core Identity |
| UI | Bootstrap 5.3 + Bootstrap Icons |
| Styling | Custom CSS (no external dependencies) |

---

## ⚙️ Configuration

To use **SQL Server** instead of SQLite:

1. Update `appsettings.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=FoodDonation;Trusted_Connection=True;"
}
```

2. In `Program.cs`, replace `UseSqlite` with `UseSqlServer`.

3. Run: `dotnet ef database update`

---

## 📝 Running Migrations Manually

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

The app auto-migrates and seeds on startup, so this is only needed if you modify models.
