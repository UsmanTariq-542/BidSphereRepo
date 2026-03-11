# 🏷️ BidSphere

A full-stack ASP.NET Core web application for online auctions — users can list products, place bids in real-time, and manage their auction activity.

🔗 **Live Demo:** [https://bidsphereapp.runasp.net]

## ✨ Features

- 🔐 User authentication & registration via ASP.NET Identity
- 🔑 Google OAuth login
- 🏷️ Create and manage auction listings
- ⚡ Real-time bidding using **SignalR**
- 🛡️ Role-based authorization (Admin panel)
- 📦 Category-based product management
- 📬 Contact form with SMTP email
- 📱 Responsive UI

---

## 🛠️ Tech Stack

| Layer | Technology |
|---|---|
| Framework | ASP.NET Core 9 (Razor Pages + MVC) |
| Database | SQL Server + Entity Framework Core |
| Auth | ASP.NET Identity + Google OAuth |
| Real-time | SignalR |
| Hosting | MonsterASP.NET |

---

## 🚀 Quick Start (Local)

### Prerequisites
- .NET 9 SDK
- SQL Server
- Git

### Steps

1. **Clone the repository:**
```bash
git clone https://github.com/UsmanTariq-542/BidSphereRepo.git
cd BidSphereRepo
```

2. **Set up your connection string in `appsettings.Development.json`:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(local);Database=BidSphere;Trusted_Connection=True;"
  }
}
```

3. **Restore and run:**
```bash
dotnet restore
dotnet build
dotnet run --project BidSphereProject
```

4. Open `https://localhost:5001` in your browser.

---

## ⚙️ Configuration

Use `dotnet user-secrets` or environment variables for sensitive values. Never commit secrets to source control.

Required settings:
- `ConnectionStrings:DefaultConnection`
- Google OAuth `ClientId` and `ClientSecret`
- SMTP credentials for contact form

---

## 🗄️ Database

- Apply `DatabaseScript.sql` to your SQL Server instance
- Or let `db.Database.EnsureCreated()` auto-generate tables on first run

---

## 🌐 Deployment (MonsterASP.NET)

```bash
dotnet publish -c Release -o publish
```
Upload the `publish` folder contents via MonsterASP.NET file manager and set environment variables for connection string and SMTP credentials.

---

## 📁 Project Structure

```
BidSphereProject/
├── Controllers/        # MVC Controllers
├── Data/               # DbContext
├── Hubs/               # SignalR Hubs
├── Interfaces/         # Repository Interfaces
├── Repositories/       # Data Access Layer
├── Services/           # Business Logic
├── Views/              # Razor Views
├── Areas/Identity/     # Auth Pages
└── wwwroot/            # Static Files
```

---

## 🔒 Security Notes

- Credentials are managed via environment variables — not hardcoded
- HTTPS enforced in production
- Cookie policies set to `Always` secure in production

---

## 📬 Contact

For bugs or feature requests, open an issue on the repository.
