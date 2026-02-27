 # Restro — Resort & Chalet Booking Website 

<img width="2000" height="592" alt="Black and Beige Minimalist Elegant Cosmetics Logo (1)" src="https://github.com/user-attachments/assets/9c2bcc92-3e91-49ba-a63f-73f4c50e676a" />

---

## ✨Overview

**Restro** is a booking website that allows users to:
- 🔎browse available places (chalets/resorts),
- 🏡view place details,
- 📅pick booking dates,
- 🧾 Complete a booking flow (Invoice ✅ / Success 🎉)

The project is intentionally organized to show strong understanding of **clean boundaries**:
- **MVC Presentation** for UI and request handling,
- **Domain** for business entities and contracts,
- **Infrastructure** for persistence and repository implementations.
---

## ✅Key Features

### 👤Customer Experience
- Browse and view place details.
- Booking flow with dedicated pages (e.g., **Invoice** and **Success**).
- Prevent selecting invalid/unavailable dates (booked days are handled server-side).

### 🔐Authentication
- Login & Registration pages with dedicated models and views.

### 🗄️Data & Persistence
- EF Core migrations for schema evolution (including user and booking invoice-related changes).
- Repository pattern (interfaces in Domain, implementations in Infrastructure).

---

## 🧰Tech Stack

- **Backend:** C#, ASP.NET Core MVC
- **UI:** Razor Views (cshtml)
- **Data Access:** Entity Framework Core (DbContext + Migrations)
- **Database:** SQL Server
- **Architecture:** Layered (MVC + Domain + Infrastructure), Repository Pattern, SOLID-friendly DI

---
## 🎥 Demo Video

[![Restro Demo](https://img.youtube.com/vi/R7ASx4bJz5Q/maxresdefault.jpg)](https://youtu.be/R7ASx4bJz5Q)

---
## 🛠️How to Run Locally

### Prerequisites
- **.NET SDK** 
- **SQL Server** (LocalDB or full instance)
- (Optional) **EF Core Tools**:
  - `dotnet tool install --global dotnet-ef`

### Steps
1. Clone the repo
2. Update the DB connection string in `appsettings.json`
3. Apply migrations:
   - `dotnet ef database update`
4. Run the project:
   - `dotnet run`
5. Open the app in your browser (the launch URL is in `Properties/launchSettings.json`)

---

## 🔮Future Enhancements

- Role-based authorization (Customer / Owner / Admin).
- Owner dashboard (manage places, pricing, availability).
- Payment gateway integration.
- Notifications (email/SMS), booking reminders.
- Automated tests (domain rules + booking overlap validation).
- CI pipeline (build + test + migrations checks).

