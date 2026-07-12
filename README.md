
# Chat App With Deaf Students

A modern, accessible communication platform built specifically for deaf students to connect, collaborate, and communicate effectively. This project features a robust ASP.NET Core API backend with real-time messaging capabilities and a .NET MAUI client application.

## 🌟 Features

- **Real-time Messaging**: Built-in SignalR support for instant message delivery
- **Speech-to-Text (STT)**: Convert audio messages to text for accessibility
- **Secure Authentication**: JWT-based authentication with BCrypt password hashing
- **User Management**: Comprehensive user profile and role-based access control
- **Accessible Design**: Developed with accessibility in mind for deaf users
- **API Documentation**: Interactive Swagger/OpenAPI documentation
- **Database**: SQL Server with Entity Framework Core ORM

## 🛠️ Tech Stack

### Backend (API)
- **.NET 10** - Latest .NET framework
- **ASP.NET Core** - Web API framework
- **Entity Framework Core 10.0.9** - ORM for database access
- **SQL Server** - Primary database
- **SignalR** - Real-time communication
- **JWT Authentication** - Secure API authentication
- **Swagger/OpenAPI** - API documentation
- **BCrypt.Net-Core** - Password hashing

### Client
- **.NET MAUI** - Cross-platform UI framework
- **Reactive UI patterns** - Modern async/await patterns

## 📋 Prerequisites

- **.NET 10 SDK** or later
- **SQL Server** 2019 or later
- **Visual Studio 2026** (or compatible IDE)
- **PowerShell** (for running scripts)

## 🚀 Quick Start

### 1. Clone Repository
```bash
git clone https://github.com/ArcadeUE4/ChatAppWithDeafStudents.git
cd ChatAppWithDeafStudents.API
```

### 2. Configure Environment Variables

Create/update the `.env` file with your configuration:

```env
# SQL Server Configuration
MSSQL_SA_PASSWORD=YourStrong@Password123
SQL_PORT=1433

# API Configuration
API_PORT=5000
ASPNETCORE_ENVIRONMENT=Development
LOG_LEVEL=Information

# JWT Configuration
JWT_KEY=dev-key-minimum-32-characters-long-change-this-in-production

# CORS Configuration
CORS_ORIGINS=http://localhost:3000,http://localhost:5000

# Nginx Configuration
NGINX_PORT=80
NGINX_SSL_PORT=443
```

### 3. Database Setup

```bash
# Apply migrations
dotnet ef database update --project ChatAppWithDeafStudents

# Or using Package Manager Console in Visual Studio
Update-Database
```

### 4. Build & Run

**API Backend:**
```bash
cd ChatAppWithDeafStudents
dotnet run
```

**Client Application:**
```bash
cd ChatAppWithDeafStudents.Client
dotnet run
```

The API will be available at `http://localhost:5000` with Swagger UI at `http://localhost:5000/swagger`

## 📁 Project Structure

```
ChatAppWithDeafStudents.API/
├── ChatAppWithDeafStudents/          # API Project (.NET 10)
│   ├── Controllers/                  # API endpoints
│   ├── Services/                     # Business logic
│   ├── Models/                       # Data models
│   ├── Data/                         # Database context
│   ├── Validators/                   # FluentValidation rules
│   ├── Middleware/                   # Custom middleware
│   └── Program.cs                    # Startup configuration
│
├── ChatAppWithDeafStudents.Client/   # MAUI Client Application
│   ├── Views/                        # UI pages
│   ├── ViewModels/                   # MVVM view models
│   ├── Services/                     # Client services
│   └── App.xaml                      # Application resources
│
├── docker-compose.yml                # Docker services configuration
├── .env                              # Environment variables
├── .gitignore                        # Git ignore rules
└── README.md                         # This file
```

## 🔐 Security

- **Password Hashing**: All passwords are hashed using BCrypt
- **JWT Tokens**: Secure token-based authentication
- **CORS Configuration**: Restricted to configured origins
- **Input Validation**: All inputs validated before processing
- **HTTPS**: Recommended for production deployments

## 🗄️ Database

### SQL Server Setup

**Connection String Format:**
```
Server=localhost,1433;Database=ChatAppWithDeafStudents;User Id=sa;Password=YourStrong@Password123;TrustServerCertificate=True;
```

**Using Docker:**
```bash
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=YourStrong@Password123" -p 1433:1433 --name mssql -d mcr.microsoft.com/mssql/server:2022-latest
```

## 🔧 API Endpoints

### Authentication
- `POST /api/auth/register` - Register new user
- `POST /api/auth/login` - User login
- `POST /api/auth/refresh` - Refresh JWT token

### Users
- `GET /api/users` - Get all users
- `GET /api/users/{id}` - Get user by ID
- `PUT /api/users/{id}` - Update user profile
- `DELETE /api/users/{id}` - Delete user

### Messages
- `GET /api/messages` - Get messages
- `POST /api/messages` - Send message
- `GET /api/messages/{id}` - Get message by ID
- `DELETE /api/messages/{id}` - Delete message


## 🧪 Testing

```bash
# Run tests
dotnet test

# Run tests with coverage
dotnet test /p:CollectCoverage=true
```

## 🐛 Troubleshooting

### Database Connection Issues
- Verify SQL Server is running
- Check connection string in `appsettings.json`
- Ensure SA password matches `.env` configuration

### Port Already in Use
```bash
# Find process using port 5000
netstat -ano | findstr :5000

# Kill process (replace PID with actual process ID)
taskkill /PID <PID> /F
```

### SignalR Connection Issues
- Verify CORS configuration includes your client URL
- Check WebSocket is enabled in IIS (if using IIS)
- Ensure firewall allows WebSocket connections

## 🚢 Deployment

### Docker Deployment
```bash
docker-compose up -d
```

## 📚 Documentation

- **Swagger UI**: Available at `/swagger` when API is running
- **OpenAPI Specification**: Available at `/openapi/v1.json`

## 🤝 Contributing

1. Fork the repository
2. Create feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to branch (`git push origin feature/AmazingFeature`)
5. Open Pull Request

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

Last Updated: 2024
