# 🍛 FoodDonationPlatform

> Anna Bacho, Bharat Bachao — A community food donation platform built for India.

![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-8.0-512BD4?style=for-the-badge&logo=dotnet)
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=csharp)
![SQLite](https://img.shields.io/badge/SQLite-003B57?style=for-the-badge&logo=sqlite)
![Bootstrap](https://img.shields.io/badge/Bootstrap-5.3-7952B3?style=for-the-badge&logo=bootstrap)
![Made in India](https://img.shields.io/badge/Made%20in-India%20🇮🇳-FF9933?style=for-the-badge)

---

## 📌 About

FoodDonationPlatform connects food donors with people in need across Indian
cities. It helps reduce food waste from events, restaurants, and households
by making surplus food available to the community.

Built with ASP.NET Core 8 MVC, Entity Framework Core, and SQLite.

---

## ✨ Features

### 👨‍💼 Admin (Donor) Role
- Post food donations with full details
- Manage own listings — edit, delete, change status
- View and manage all incoming orders
- Update order status: Pending → Confirmed → Ready → Completed
- Admin dashboard with stats

### 👤 Local User Role
- Browse all available food listings
- Search and filter by category, location, keyword
- Place orders and track order status
- Cancel pending orders
- Also donate food items
- User dashboard with personalized feed

---

## 🗄️ Tech Stack

| Layer      | Technology                  |
|------------|-----------------------------|
| Framework  | ASP.NET Core 8 MVC          |
| Language   | C# 12                       |
| ORM        | Entity Framework Core 8     |
| Database   | SQLite                      |
| Auth       | ASP.NET Core Identity       |
| Frontend   | Bootstrap 5.3 + Bootstrap Icons |

---

## 🚀 Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

### Run Locally
```bash
# 1. Clone the repo
git clone https://github.com/YOUR_USERNAME/FoodDonationPlatform.git

# 2. Enter project folder
cd FoodDonationPlatform

# 3. Restore packages
dotnet restore

# 4. Run the app
dotnet run
```

Open browser at **http://localhost:5000**

Database is automatically created and seeded on first run.

---

## 🔑 Demo Login Credentials

| Role           | Email                  | Password     |
|----------------|------------------------|--------------|
| **Admin**      | admin@annadata.in      | Admin@123456 |
| **Local User** | user@annadata.in       | User@123456  |

---

## 📁 Project Structure
```
FoodDonationPlatform/
├── Controllers/
│   ├── AccountController.cs       # Login, Register, Logout
│   ├── HomeController.cs          # Role-based Dashboard
│   ├── FoodListingsController.cs  # CRUD for food listings
│   └── OrdersController.cs        # Order management
├── Models/
│   ├── Models.cs                  # ApplicationUser, FoodListing, Order
│   └── ViewModels.cs              # All ViewModels
├── Data/
│   ├── ApplicationDbContext.cs    # EF Core DbContext
│   └── DbSeeder.cs                # Seeds Indian demo data
├── Views/
│   ├── Shared/_Layout.cshtml      # Main layout + navbar
│   ├── Home/                      # Landing, Dashboards
│   ├── Account/                   # Login, Register
│   ├── FoodListings/              # Browse, Create, Edit, Details
│   └── Orders/                    # Place Order, Order History
└── wwwroot/
    ├── css/site.css               # Custom styles
    └── js/site.js                 # Client-side scripts
```

---

## 🗺️ Cities Covered

Delhi • Mumbai • Bengaluru • Chennai • Hyderabad • Jaipur • Lucknow • Pune • Kolkata • Ahmedabad

---

## 📄 License

This project is open source under the [MIT License](LICENSE).

---

<div align="center">
Made with ❤️ in Bharat 🇮🇳
</div>
