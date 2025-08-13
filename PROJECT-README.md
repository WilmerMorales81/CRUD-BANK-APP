# 🏦 CRUD Bank Application

A full-stack banking application demonstrating modern web development practices with .NET Core backend and React frontend.

## 🚀 Live Demo

- **Frontend:** [https://crud-bank-app.vercel.app](https://crud-bank-app.vercel.app)
- **Backend API:** [https://crud-bank-app-production.up.railway.app](https://crud-bank-app-production.up.railway.app)

## 📋 Features

### 🔐 Authentication & Authorization
- JWT token-based authentication
- Role-based access control (Admin/Customer)
- Secure password hashing
- User registration and login

### 💳 Account Management
- Create and manage bank accounts
- Multiple account types (Checking, Savings)
- Real-time balance tracking
- Account deletion with balance validation

### 💰 Payment Processing
- Process payments on accounts
- Payment history tracking
- Multiple payment types support
- Transaction validation

### 👥 User Management
- User profile management
- Admin dashboard for customer oversight
- Customer account linking

## 🛠️ Tech Stack

### Backend
- **Framework:** ASP.NET Core 8.0
- **ORM:** Entity Framework Core
- **Database:** PostgreSQL (Neon)
- **Authentication:** ASP.NET Core Identity + JWT
- **Hosting:** Railway
- **API Documentation:** Swagger/OpenAPI

### Frontend
- **Framework:** React 18
- **Build Tool:** Vite
- **Styling:** Bootstrap 5
- **State Management:** React Hooks
- **Routing:** React Router
- **Hosting:** Vercel

### Database
- **Provider:** Neon (PostgreSQL)
- **Migrations:** Entity Framework
- **Connection:** SSL with connection pooling

## 🏗️ Architecture

```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   React App     │    │  ASP.NET Core   │    │   PostgreSQL    │
│   (Vercel)      │◄──►│   API (Railway) │◄──►│   (Neon)        │
│                 │    │                 │    │                 │
│ - User Interface│    │ - REST API      │    │ - User Data     │
│ - State Mgmt    │    │ - Authentication│    │ - Accounts      │
│ - Routing       │    │ - Business Logic│    │ - Transactions  │
└─────────────────┘    └─────────────────┘    └─────────────────┘
```

## 🚀 Getting Started

### Prerequisites
- .NET 8.0 SDK
- Node.js 18+
- PostgreSQL database

### Backend Setup
```bash
# Clone the repository
git clone https://github.com/your-username/CRUD-BANK-APP.git
cd CRUD-BANK-APP

# Restore dependencies
dotnet restore

# Update connection string in appsettings.json
# Run migrations
dotnet ef database update

# Start the application
dotnet run
```

### Frontend Setup
```bash
cd client

# Install dependencies
npm install

# Start development server
npm run dev
```

## 📁 Project Structure

```
CRUD-BANK-APP/
├── Controllers/          # API Controllers
├── Models/              # Data Models & DTOs
├── Data/                # Database Context
├── Migrations/          # EF Core Migrations
├── client/              # React Frontend
│   ├── src/
│   │   ├── components/  # React Components
│   │   ├── managers/    # API Managers
│   │   └── config/      # Configuration
│   └── public/          # Static Assets
├── Dockerfile           # Container Configuration
└── README.md           # This File
```

## 🔧 Environment Variables

### Backend (Railway)
```
CRUD_BANK_CONN=Host=...;Database=...;Username=...;Password=...;SSL Mode=Require
Jwt__Key=your-secret-key
Jwt__Issuer=https://your-backend-url
Jwt__Audience=https://your-frontend-url
```

### Frontend (Vercel)
```
VITE_API_URL=https://your-backend-url
```

## 🧪 Testing

### API Endpoints
- **Health Check:** `GET /health`
- **Authentication:** `POST /api/auth/login`
- **Accounts:** `GET /api/accounts`
- **Payments:** `POST /api/accounts/pay/{id}`

### Default Admin User
- **Email:** admin@crudbank.com
- **Password:** Admin123!

## 🚀 Deployment

### Backend (Railway)
1. Connect GitHub repository to Railway
2. Set environment variables
3. Deploy automatically on push

### Frontend (Vercel)
1. Connect GitHub repository to Vercel
2. Set root directory to `client`
3. Configure environment variables
4. Deploy automatically on push

## 📊 Performance

- **Backend Response Time:** < 200ms
- **Database Queries:** Optimized with EF Core
- **Frontend Load Time:** < 2s
- **SSL/TLS:** Enabled for all connections

## 🔒 Security Features

- JWT token authentication
- Password hashing with ASP.NET Core Identity
- CORS configuration for production
- Input validation and sanitization
- SQL injection prevention with EF Core

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Test thoroughly
5. Submit a pull request

## 📝 License

This project is licensed under the MIT License.

## 👨‍💻 Author

[Your Name] - [your.email@example.com]

---

⭐ **If you find this project helpful, please give it a star!** 