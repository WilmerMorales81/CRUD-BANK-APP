# 🏦 CRUD Bank Application

A full-stack banking application demonstrating modern web development practices with .NET Core backend and React frontend. This project showcases complete CRUD operations, authentication, payment processing, and cloud deployment.

## 🚀 Live Demo

- **Frontend:** [https://crud-bank-app.vercel.app](https://crud-bank-app.vercel.app)
- **Backend API:** [https://crud-bank-app-production.up.railway.app](https://crud-bank-app-production.up.railway.app)

## 📋 Features

### 🔐 Authentication & Authorization
- JWT token-based authentication
- Role-based access control (Admin/Customer)
- Secure password hashing with ASP.NET Core Identity
- User registration and login system

### 💳 Account Management
- Create and manage bank accounts
- Multiple account types (Checking, Savings)
- Real-time balance tracking
- Account deletion with balance validation
- Account history and transaction logs

### 💰 Payment Processing
- Process payments on accounts
- Payment history tracking
- Multiple payment types support
- Transaction validation and error handling
- Real-time balance updates

### 👥 User Management
- User profile management
- Admin dashboard for customer oversight
- Customer account linking
- Role-based permissions

## 🛠️ Tech Stack

### Backend
- **Framework:** ASP.NET Core 8.0
- **ORM:** Entity Framework Core
- **Database:** PostgreSQL (Neon)
- **Authentication:** ASP.NET Core Identity + JWT
- **Hosting:** Railway
- **API Documentation:** Swagger/OpenAPI
- **Containerization:** Docker

### Frontend
- **Framework:** React 18
- **Build Tool:** Vite
- **Styling:** Bootstrap 5
- **State Management:** React Hooks
- **Routing:** React Router
- **Hosting:** Vercel
- **Package Manager:** npm

### Database
- **Provider:** Neon (PostgreSQL)
- **Migrations:** Entity Framework
- **Connection:** SSL with connection pooling
- **Backup:** Automated backups

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
- Git

### Backend Setup
```bash
# Clone the repository
git clone https://github.com/WilmerMorales81/CRUD-BANK-APP.git
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
│   ├── AuthController.cs # Authentication endpoints
│   ├── AccountsController.cs # Account management
│   └── ...
├── Models/              # Data Models & DTOs
│   ├── UserProfile.cs
│   ├── Account.cs
│   ├── DTOs/           # Data Transfer Objects
│   └── ...
├── Data/                # Database Context
│   └── CrudBankAppDbContext.cs
├── Migrations/          # EF Core Migrations
├── client/              # React Frontend
│   ├── src/
│   │   ├── components/  # React Components
│   │   │   ├── Accounts/
│   │   │   ├── auth/
│   │   │   ├── Payments/
│   │   │   └── ...
│   │   ├── managers/    # API Managers
│   │   ├── config/      # Configuration
│   │   └── ...
│   └── public/          # Static Assets
├── Dockerfile           # Container Configuration
├── .dockerignore        # Docker ignore file
└── README.md           # This File
```

## 🔧 Environment Variables

### Backend (Railway)
```env
CRUD_BANK_CONN=Host=...;Database=...;Username=...;Password=...;SSL Mode=Require
Jwt__Key=your-secret-key
Jwt__Issuer=https://your-backend-url
Jwt__Audience=https://your-frontend-url
AdminEmail=admin@crudbank.com
AdminPassword=Admin123!
```

### Frontend (Vercel)
```env
VITE_API_URL=https://your-backend-url
```

## 🧪 Testing

### API Endpoints
- **Health Check:** `GET /health`
- **Authentication:** `POST /api/auth/login`
- **Registration:** `POST /api/auth/register`
- **User Info:** `GET /api/auth/me`
- **Accounts:** `GET /api/accounts`
- **Create Account:** `POST /api/accounts`
- **Payments:** `POST /api/accounts/pay/{id}`
- **Delete Account:** `DELETE /api/accounts/{id}`

### Default Admin User
- **Email:** admin@crudbank.com
- **Password:** Admin123!

## 🚀 Deployment

### Backend (Railway)
1. Connect GitHub repository to Railway
2. Set environment variables
3. Configure build settings
4. Deploy automatically on push

### Frontend (Vercel)
1. Connect GitHub repository to Vercel
2. Set root directory to `client`
3. Configure environment variables
4. Deploy automatically on push

### Database (Neon)
1. Create PostgreSQL database
2. Configure connection string
3. Run Entity Framework migrations
4. Set up automated backups

## 📊 Performance

- **Backend Response Time:** < 200ms
- **Database Queries:** Optimized with EF Core
- **Frontend Load Time:** < 2s
- **SSL/TLS:** Enabled for all connections
- **CORS:** Configured for production

## 🔒 Security Features

- JWT token authentication
- Password hashing with ASP.NET Core Identity
- CORS configuration for production
- Input validation and sanitization
- SQL injection prevention with EF Core
- HTTPS enforcement
- Secure headers

## 🐳 Docker Support

The application includes Docker configuration for containerized deployment:

```bash
# Build the Docker image
docker build -t crud-bank-app .

# Run the container
docker run -p 8080:80 crud-bank-app
```

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 👨‍💻 Author

**Wilmer Morales**
- **Email:** [moralesw.1981@gmail.com](mailto:moralesw.1981@gmail.com)
- **LinkedIn:** [Wilmer Morales](https://www.linkedin.com/in/wilmermorales)
- **Location:** Franklin, TN

## 🙏 Acknowledgments

- ASP.NET Core team for the excellent framework
- React team for the powerful frontend library
- Railway, Vercel, and Neon for free hosting services
- Bootstrap team for the responsive UI framework

---

⭐ **If you find this project helpful, please give it a star!**

🔗 **Live Demo:** [https://crud-bank-app.vercel.app](https://crud-bank-app.vercel.app)