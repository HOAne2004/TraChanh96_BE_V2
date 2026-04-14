# 🧋 Beverage Ordering System (Backend)
A backend system for an online beverage ordering platform, designed to simulate **real-world order processing workflows, business rule enforcement, and modular backend architecture**.
---
## 🚀 Tech Stack
- ASP.NET Core (Web API)
- Entity Framework Core
- PostgreSQL (Supabase)
- JWT Authentication
- Docker (basic)
---
## 🧠 Architecture Overview
The project follows a **layered Clean Architecture approach**:
- **API Layer**: Handles HTTP requests and responses  
- **Application Layer**: Contains business workflows and services  
- **Domain Layer**: Defines core entities and business rules  
- **Infrastructure Layer**: Handles database and external services  
> The system is structured as a **modular monolith** for simplicity and maintainability.
---
## 🔄 Order Lifecycle
The system models a simplified order lifecycle:
Pending → Paid → Completed
↘ Cancelled (timeout)
- Orders are automatically cancelled if not confirmed within a time window  
- State transitions are controlled at the application level  
---
## ⚙️ Key Features
### 🔐 Authentication & Authorization
- JWT-based authentication  
- Role-based access control  
---

### 🧾 Order Management
- Create and manage orders  
- Apply business rules to prevent invalid actions  
- Maintain consistent order states  
---
### ⏱️ Background Processing
- Implemented using `IHostedService`  
- Automatically cancels unconfirmed orders after a defined period  
---
### 📍 Geolocation Integration
- Converts user addresses into coordinates  
- Supports delivery-related calculations  
---
## 🗄️ Database Design
- PostgreSQL deployed via Supabase  
- Relational schema with entities such as:
  - Users
  - Orders
  - OrderItems
  - Products  
---
## ▶️ How to Run
```bash
git clone https://github.com/HOAne2004/TraChanh96_BE_V2
cd TraChanh96_BE_V2
dotnet restore
dotnet run
##📌 Notes
This project focuses on backend design and business logic
Some advanced features (caching, message queue, distributed processing) are not included
👤 Author

Le Huy Hoan
