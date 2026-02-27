# Restro — Resort & Chalet Booking Website (ASP.NET Core MVC)

A modern **ASP.NET Core MVC** web application for browsing resorts/chalets and creating bookings with a clean, layered codebase (Domain + Infrastructure + MVC).

> This README is written to be **GitHub-ready** and is aligned to your current solution layout (Controllers/Views/Domain/Infrastructure/Repositories + EF Core migrations).

---

## Table of Contents
- [Overview](#overview)
- [Key Features](#key-features)
- [Tech Stack](#tech-stack)
- [Architecture Overview](#architecture-overview)
- [Project Structure](#project-structure)
- [Database Design](#database-design)
- [How to Run Locally](#how-to-run-locally)
- [Configuration](#configuration)
- [Screenshots & Media](#screenshots--media)
- [Demo Video](#demo-video)
- [Future Enhancements](#future-enhancements)
- [Security Notes](#security-notes)

---

## Overview

**Restro** is a booking website that allows users to:
- browse available places (chalets/resorts),
- view place details,
- pick booking dates,
- and complete a booking flow that can include an invoice/success step.

The project is intentionally organized to show strong understanding of **clean boundaries**:
- **MVC Presentation** for UI and request handling,
- **Domain** for business entities and contracts,
- **Infrastructure** for persistence and repository implementations.

---

## Key Features

### Customer Experience
- Browse and view place details.
- Booking flow with dedicated pages (e.g., **Invoice** and **Success**).
- Prevent selecting invalid/unavailable dates (booked days are handled server-side).

### Authentication
- Login & Registration pages with dedicated models and views.

### Data & Persistence
- EF Core migrations for schema evolution (including user and booking invoice-related changes).
- Repository pattern (interfaces in Domain, implementations in Infrastructure).

---

## Tech Stack

- **Backend:** C#, ASP.NET Core MVC
- **UI:** Razor Views (cshtml)
- **Data Access:** Entity Framework Core (DbContext + Migrations)
- **Database:** SQL Server
- **Architecture:** Layered (MVC + Domain + Infrastructure), Repository Pattern, SOLID-friendly DI

---

## Architecture Overview

The solution follows a practical layered style:

- **Presentation (MVC):** Controllers + Razor Views + UI models  
- **Application/Queries:** query/use-case services (e.g., place query service)  
- **Domain:** entities + repository interfaces + business rules (where applicable)  
- **Infrastructure:** EF Core DbContext + repository implementations + migrations  

### Ready Diagram (included in this repo)
![Layered Architecture](docs/images/architecture/layered-architecture-reference.png)

Additional reference diagrams:
- ![Controller → Repo → Entity](docs/images/architecture/layers-controller-repo-reference.png)
- ![Architecture Patterns (Reference)](docs/images/architecture/architecture-patterns-reference.png)

> Note: These are **visual references** you already collected. Your real mapping is described in `docs/architecture/architecture-diagram.md`.

---

## Project Structure

High-level structure (as in your Solution Explorer screenshots):

- `Controllers/` (e.g., `LandingController`, `PlacesController`, …)
- `Views/`
  - `Views/Auth/` (`Login.cshtml`, `Register.cshtml`)
  - `Views/Bookings/` (`Invoice.cshtml`, `Success.cshtml`)
  - `Views/Home/` (`Index.cshtml`)
  - `Views/Landing/` (`Welcome.cshtml`)
  - `Views/Places/` (`Details.cshtml`)
- `Models/` (e.g., `LoginModel`, `RegisterModel`, `ErrorViewModel`)
- `Domain/`
  - `Bookings/` (e.g., `Booking`, `BookingStatus`, `IBookingRepository`)
  - `Places/` (e.g., `Place`, `Feature`, `IPlaceRepository`, `IFeatureRepository`)
  - `Users/` (e.g., `User`, `IUserRepository`)
- `Infrastructure/`
  - `Database/` (`AppDbContext`, constants/config helpers)
  - `Repositories/` (EF implementations, query services)
- `Migrations/` (EF Core migration history)

Structure reference images:
- ![Views & Models](docs/images/architecture/solution-structure-views-models.png)
- ![Infra & Migrations](docs/images/architecture/solution-structure-infra-migrations.png)
- ![Domain & Infrastructure](docs/images/architecture/solution-structure-domain-infrastructure.png)

---

## Database Design

This project uses **EF Core Migrations** to manage schema changes over time.

Typical core entities (based on your Domain layout):
- **Place** (a chalet/resort listing)
- **Feature** (amenities related to a place)
- **Booking** (date range + status + optional invoice fields)
- **User** (auth & profile data)

> Add your ERD screenshot here once ready.

---

## How to Run Locally

### Prerequisites
- **.NET SDK** (use the same version as your project `TargetFramework`)
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

## Configuration

### Connection Strings
Set your SQL Server connection string in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=...;Database=...;User Id=...;Password=...;TrustServerCertificate=True"
  }
}
```

> Don’t commit real passwords. Use user-secrets or environment variables for private settings.

---

## Screenshots & Media

> Add images under `docs/images/` and keep filenames consistent.

### UI Screenshots
- Home Page  
  `docs/images/ui/home.png`
- Place Details  
  `docs/images/ui/place-details.png`
- Booking Flow (Date Picker / Summary)  
  `docs/images/ui/booking-flow.png`
- Invoice + Success  
  `docs/images/ui/invoice.png`  
  `docs/images/ui/success.png`

### Architecture Diagram
- `docs/images/architecture/your-architecture.png`

### Database / ERD
- `docs/images/db/erd.png`

See: **`docs/README_MEDIA_GUIDE.md`** for exact naming + where to place things.

---

## Demo Video

GitHub doesn’t reliably embed videos inside `README.md`, so use one of these:

**Option A (Recommended): link the MP4**
- Put your video at: `docs/videos/demo.mp4`
- Link it:
  - 🎥 Demo video: [`docs/videos/demo.mp4`](docs/videos/demo.mp4)

**Option B (Best README experience): GIF preview + full video link**
- Add a short GIF:
  - `docs/images/demo/demo-preview.gif`
- Then link the full MP4:
  - Full video: [`docs/videos/demo.mp4`](docs/videos/demo.mp4)

---

## Future Enhancements

- Role-based authorization (Customer / Owner / Admin).
- Owner dashboard (manage places, pricing, availability).
- Payment gateway integration.
- Notifications (email/SMS), booking reminders.
- Automated tests (domain rules + booking overlap validation).
- CI pipeline (build + test + migrations checks).

---

## Security Notes

- Never commit secrets (DB passwords, API keys).
- Prefer **User Secrets** locally and **Environment Variables** in production.
- Validate all booking inputs on both client and server.
