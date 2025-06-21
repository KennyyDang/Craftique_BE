# 🛍️ Craftique Shop - Backend API

Welcome to **Craftique Shop**, a modern e-commerce backend solution built with ASP.NET Core 8 and Entity Framework Core using **Code First** approach.

> 🔧 This project was inspired by Apple Mart and extended for enhanced database seeding, role management, and modular API development.

---

## 🚀 Technologies Used

- ✅ ASP.NET Core 8.0  
- ✅ Entity Framework Core (Code First)  
- ✅ SQL Server  
- ✅ JWT Authentication  
- ✅ Identity Role + User Management  
- ✅ RESTful APIs  

---

## 📁 Project Structure
Craftique_BE/
├── CraftiqueBE.API/ # Web API Project
│ ├── Controllers/
│ ├── Data/
│ ├── Entities/
│ ├── Models/
│ └── DbInitializer.cs
├── .github/workflows/ # GitHub Actions (CI)
├── README.md

---

## 💡 Getting Started

### 1. Clone the repository

```bash
git clone https://github.com/KennyyDang/Craftique_BE.git
cd CraftiqueBE.API
```
### 2. Install EF Core Packages
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Tools
### 3. Configure SQL Server in appsettings.json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=CraftiqueDB;User Id=sa;Password=yourpassword;TrustServerCertificate=True"
}
### 4. Migrations & Database Setup
dotnet ef migrations add InitialCreate
dotnet ef database update
### 5. Auto-Seed Roles, Users, and Products
The file DbInitializer.cs contains:

Admin, Staff, Customer, Shipper roles

Demo users

Sample products, categories, product variants

Sample images and attributes

👉 Automatically runs when the app starts.
# 🔐 Authentication
Uses JWT Bearer Token authentication

Users are assigned roles like Admin, Customer, etc.

Built-in support for Identity + RoleManager + UserManager
## 🧪 API Testing
You can test the API using Swagger or Postman.

Swagger: run project and go to https://localhost:<port>/swagger
### 📦 Build & Run
dotnet build
dotnet run
#### 🙌 Contributors
@KennyyDang
