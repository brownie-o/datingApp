# Dating App

A full-stack web application built with ASP.NET Core and Angular, enabling users to connect and interact with each other through messaging, likes, and real-time notifications.

**Live Demo:** https://dating-2026.azurewebsites.net/

## 📋 Features

- **User Authentication & Authorization**
  - Secure login and registration
  - JWT token-based authentication
  - Role-based access control (Admin, User)

- **User Profiles**
  - User member profiles with photos
  - Profile customization and updates
  - Photo upload and management

- **Social Interactions**
  - Like/unlike users
  - View who liked you

- **Real-Time Messaging**
  - Direct messaging between users
  - Real-time message notifications via SignalR
  - Message read status tracking
  - Message history

- **Admin Dashboard**
  - User moderation tools
  - Photo approval workflow
  - System administration

- **Real-Time Presence**
  - User online/offline status
  - Last activity tracking
  - Real-time presence updates

## 🛠️ Tech Stack

### Backend
- **Framework:** ASP.NET Core 10.0
- **Database:** SQL Server
- **Authentication:** JWT (JSON Web Tokens)
- **Real-Time:** SignalR
- **ORM:** Entity Framework Core
- **File Storage:** Cloudinary
- **Language:** C#

### Frontend
- **Framework:** Angular 21.0
- **Language:** TypeScript
- **Styling:** CSS, DaisyUi
- **Package Manager:** npm
- **Testing:** Vitest

### Cloud & Deployment
- **Hosting:** Azure App Service
- **Database:** Azure SQL Database

## 📁 Project Structure

```
datingApp/
├── API/                          # ASP.NET Core backend
│   ├── Controllers/              # API endpoints
│   ├── Data/                     # Database context & repositories
│   ├── DTOs/                     # Data transfer objects
│   ├── Entities/                 # Domain models
│   ├── Interfaces/               # Service contracts
│   ├── Services/                 # Business logic
│   ├── SignalR/                  # Real-time hubs
│   ├── Middleware/               # Custom middleware
│   ├── Extensions/               # Extension methods
│   ├── Helpers/                  # Utility classes
│   └── Program.cs                # App configuration
│
├── client/                       # Angular frontend
│   ├── src/
│   │   ├── app/                  # Angular components & modules
│   │   ├── core/                 # Core services
│   │   ├── features/             # Feature modules
│   │   ├── layout/               # Layout components
│   │   ├── shared/               # Shared components & services
│   │   ├── types/                # TypeScript type definitions
│   │   └── environments/         # Environment configurations
│   └── package.json              # Frontend dependencies
│
└── datingApp.sln                 # Solution file

```

## 🚀 Getting Started

### Prerequisites

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download)
- [Node.js 18+](https://nodejs.org/) and npm
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) or Windows Authentication to Azure SQL
- Cloudinary account for image storage

### Local Development Setup

#### 1. Clone the Repository

```bash
git clone https://github.com/yourusername/datingApp.git
cd datingApp
```

#### 2. Backend Setup (API)

```bash
# Navigate to API directory
cd API

# Install dependencies
dotnet restore

# Update database
dotnet ef database update

# Run the API
dotnet run
```

The API will be available at `https://localhost:5001` (or configured port in `launchSettings.json`)

#### 3. Frontend Setup (Angular)

```bash
# Open a new terminal and navigate to client directory
cd client

# Install dependencies
npm install

# Start development server
ng serve
```

The Angular app will be available at `http://localhost:4200/`

### Configuration

#### Backend Configuration

Update `appsettings.Development.json` with your local settings:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "CloudinarySettings": {
    "CloudName": "your-cloud-name",
    "ApiKey": "your-api-key",
    "ApiSecret": "your-api-secret"
  },
  "AllowedHosts": "*"
}
```

#### Frontend Configuration

Update `src/environments/environment.ts`:

```typescript
export const environment = {
  production: false,
  apiUrl: 'https://localhost:5001/api/',
  hubUrl: 'https://localhost:5001/hubs/'
};
```

## 📦 Building for Production

### Backend

```bash
cd API
dotnet publish -c Release -o ./bin/Publish
```

### Frontend

```bash
cd client
ng build
```

The production build will be output to `dist/` directory.

## 🌐 Deployment on Azure

### Prerequisites
- Azure subscription
- SQL Server instance on Azure

### Deployment Steps

1. **Create Azure Resources**
   - App Service (for hosting)
   - Azure SQL Database

2. **Deploy Backend**
   ```bash
   # Publish backend
   dotnet publish -c Release
   ```
   **Continuous Integration to Azure App Service**
    - Link your GitHub with Web App Deployment Source
    - Select Add a workflow for Workflow option
    - Select User-assinged identity for Authentication type

3. **Deploy Frontend**
   ```bash
   # Build Angular app
   ng build
   
   ```

4. **Configure Settings on Azure**
   - Set connection strings
   - Set environment variables
   - Enable Web sockets in Azure Web App

## 📝 API Documentation

### Key Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/account/register` | Register new user |
| POST | `/api/account/login` | Login user |
| GET | `/api/members` | Get list of users |
| GET | `/api/members/{id}` | Get user profile |
| PUT | `/api/members` | Update profile |
| POST | `/api/likes/{id}` | Like a user |
| DELETE | `/api/likes/{id}` | Unlike a user |
| GET | `/api/likes` | Get likes list |
| POST | `/api/messages` | Send message |
| GET | `/api/messages` | Get messages |
| POST | `/api/admin/approve-photo/{photoId}` | Approve photo (admin) |


## 🔐 Security Features

- JWT-based authentication
- HTTPS enforced in production
- SQL injection prevention via parameterized queries
- Role-based authorization
- CORS configuration
- Exception handling middleware

## 📚 Resources

- [ASP.NET Core Documentation](https://docs.microsoft.com/en-us/dotnet/core/)
- [Angular Documentation](https://angular.io/docs)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
- [SignalR Documentation](https://docs.microsoft.com/en-us/aspnet/core/signalr/)
- [Azure App Service](https://azure.microsoft.com/en-us/services/app-service/)

---

**Last Updated:** April 2026

**Live Application:** https://dating-2026.azurewebsites.net/
