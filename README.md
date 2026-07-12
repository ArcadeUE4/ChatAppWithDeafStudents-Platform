# Chat App With Deaf Students

A modern, accessible communication platform built specifically for deaf students to connect, collaborate, and communicate effectively. This project features a robust ASP.NET Core API backend with real-time messaging capabilities and a .NET MAUI client application.

## 🌟 Features

- **Real-time Messaging**: Built-in SignalR support for instant message delivery
- **Speech-to-Text (STT)**: Convert audio messages to text for accessibility
- **Text-to-Speech (TTS)**: Convert text messages to speech output
- **Secure Authentication**: JWT-based authentication with BCrypt password hashing
- **User Management**: Comprehensive user profile and role-based access control
- **Accessible Design**: Developed with accessibility in mind for deaf users
- **API Documentation**: Interactive Swagger/OpenAPI documentation
- **Cross-Platform**: .NET MAUI client for multi-platform support
- **Data Validation**: FluentValidation for robust input validation
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
- **FluentValidation** - Input validation
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

*For complete API documentation, visit `/swagger` endpoint when the API is running.*

## 🎤 Speech-to-Text (STT)

Speech-to-Text conversion allows users to dictate messages, which are then transcribed to text. This is especially useful for deaf students who may prefer to use voice input through assistive technologies.

### STT Endpoints

```
POST /api/stt/transcribe - Convert audio to text
GET /api/stt/languages - Get supported languages
POST /api/stt/process-message - Process message with audio attachment
```

### Audio Upload and Transcription

**Request:**
```
POST /api/stt/transcribe
Content-Type: multipart/form-data

Parameter: audio (binary file)
Parameter: language (string, e.g., "en-US", "ru-RU")
Header: Authorization: Bearer JWT_TOKEN
```

**Response:**
```json
{
  "success": true,
  "text": "Hello, how are you?",
  "confidence": 0.95,
  "language": "en-US",
  "duration": 3500,
  "timestamp": "2024-01-15T10:30:00Z"
}
```

### Client Implementation (.NET MAUI)

```csharp
public async Task<string> TranscribeAudioAsync(string audioFilePath, string language = "en-US")
{
    using (var client = new HttpClient())
    {
        client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", _authToken);

        using (var form = new MultipartFormDataContent())
        {
            var fileStream = File.OpenRead(audioFilePath);
            form.Add(new StreamContent(fileStream), "audio", Path.GetFileName(audioFilePath));
            form.Add(new StringContent(language), "language");

            var response = await client.PostAsync("/api/stt/transcribe", form);
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<TranscriptionResult>(jsonResponse);

            return result.Text;
        }
    }
}

public class TranscriptionResult
{
    public bool Success { get; set; }
    public string Text { get; set; }
    public double Confidence { get; set; }
    public string Language { get; set; }
    public int Duration { get; set; }
    public DateTime Timestamp { get; set; }
}
```

### Supported Languages

| Code | Language |
|------|----------|
| en-US | English (US) |
| en-GB | English (UK) |
| ru-RU | Russian |
| fr-FR | French |
| de-DE | German |
| es-ES | Spanish |
| zh-CN | Chinese (Simplified) |
| ja-JP | Japanese |

### STT Configuration

```json
{
  "Stt": {
    "Enabled": true,
    "Provider": "AzureCognitive",
    "ApiKey": "your-stt-api-key",
    "Region": "eastus",
    "SupportedLanguages": ["en-US", "ru-RU", "fr-FR", "de-DE"],
    "MaxAudioDuration": 60000,
    "MaxFileSizeMB": 25
  }
}
```

### Environment Variables for STT
```env
# Azure Cognitive Services (or your STT provider)
STT_PROVIDER=AzureCognitive
STT_API_KEY=your-stt-api-key
STT_REGION=eastus
STT_ENABLED=true
```

### Recording Audio in .NET MAUI

```csharp
public class AudioRecorder
{
    private MediaManager _mediaManager;
    private string _outputPath;

    public async Task StartRecordingAsync()
    {
        _outputPath = Path.Combine(
            FileSystem.AppDataDirectory, 
            $"audio_{DateTime.Now.Ticks}.wav"
        );

        await _mediaManager.StartRecordingAsync(_outputPath);
    }

    public async Task<string> StopRecordingAsync()
    {
        await _mediaManager.StopRecordingAsync();
        return _outputPath;
    }

    public async Task<string> TranscribeRecordingAsync(string authToken)
    {
        var sttService = new SpeechToTextService(authToken);
        return await sttService.TranscribeAudioAsync(_outputPath);
    }
}
```

## 📝 Configuration

### appsettings.json
```json
{
  "Jwt": {
	"Key": "your-secret-key-minimum-32-characters-long",
	"Issuer": "ChatAppWithDeafStudents",
	"Audience": "ChatAppWithDeafStudentsUsers",
	"ExpirationMinutes": 60
  },
  "Cors": {
	"AllowedOrigins": ["http://localhost:3000", "http://localhost:5000"]
  },
  "Stt": {
	"Enabled": true,
	"Provider": "AzureCognitive",
	"SupportedLanguages": ["en-US", "ru-RU", "fr-FR", "de-DE"],
	"MaxAudioDuration": 60000,
	"MaxFileSizeMB": 25
  }
}
```

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

## 👥 Support

For support, please open an issue in the GitHub repository or contact the development team.

---

**Built with ❤️ for Deaf Students Community**

Last Updated: 2024
