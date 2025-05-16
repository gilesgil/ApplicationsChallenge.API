# Applications Challenge API

This repository contains a .NET Core Web API for managing applications, authentication, and real-time updates.

## Project Overview

The Applications Challenge API provides a platform for users to submit various types of applications (requests, offers, complaints) and track their status in real-time. The API includes JWT authentication, real-time updates via SignalR, and automatic status transitions through a background service.

## Features

- **User Authentication**: JWT-based authentication system
- **Application Management**: Create, read, update, and delete operations for applications
- **Real-time Updates**: SignalR integration for instant status updates
- **Background Processing**: Automatic status transitions for submitted applications
- **PostgreSQL Database**: Data persistence using Entity Framework Core
- **Swagger Documentation**: Interactive API documentation

## Tech Stack

- **ASP.NET Core 8.0**: Framework for building the API
- **Entity Framework Core**: ORM for database operations
- **PostgreSQL**: Database system
- **JWT**: JSON Web Tokens for authentication
- **SignalR**: Real-time communication
- **Swagger/OpenAPI**: API documentation

## Project Structure

```
ApplicationsChallenge.API/
├── Controllers/             # API endpoints
│   ├── ApplicationsController.cs
│   └── AuthController.cs
├── Data/                   # Database context
│   └── ApplicationDbContext.cs
├── Hubs/                   # SignalR hubs for real-time updates
│   └── ApplicationHub.cs
├── Models/                 # Data models
│   ├── Application.cs
│   └── User.cs
├── Repositories/           # Data access layer
│   ├── ApplicationRepository.cs
│   ├── IApplicationRepository.cs
│   ├── IUserRepository.cs
│   └── UserRepository.cs
├── Services/               # Business logic
│   ├── ApplicationService.cs
│   ├── ApplicationStatusBackgroundService.cs
│   ├── AuthenticationService.cs
│   ├── IApplicationService.cs
│   └── IAuthenticationService.cs
├── Program.cs              # Application startup and configuration
└── appsettings.json        # Application settings
```

## API Endpoints

### Authentication

- `POST /api/Auth/login`: Authenticate user and get JWT token
- `GET /api/Auth/me`: Get current authenticated user information

### Applications

- `GET /api/Applications`: Get all applications
- `GET /api/Applications/{id}`: Get application by ID
- `POST /api/Applications`: Create a new application
- `PUT /api/Applications/{id}/status`: Update application status
- `DELETE /api/Applications/{id}`: Delete an application

## Real-time Updates

The API uses SignalR to provide real-time updates when application statuses change. Clients can subscribe to the `/hubs/applications` endpoint and listen for `ReceiveStatusUpdate` events.

## Background Processing

The application includes a background service that automatically updates application status from "submitted" to "completed" after a configurable time delay (default: 1 minute).

## Setup and Installation

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [PostgreSQL](https://www.postgresql.org/download/)

### Database Configuration

Edit the connection string in `appsettings.json` to point to your PostgreSQL database:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Database=ApplicationsDB;Username=your_username;Password=your_password"
}
```

### Running the Application

1. Clone the repository
2. Navigate to the project directory
3. Run the application:

```bash
cd ApplicationsChallenge.API
dotnet restore
dotnet run
```

4. Access the Swagger documentation at: http://localhost:5298/swagger

## Security Considerations

- The JWT key in `appsettings.json` is for development only. In production, use a secure, environment-specific key.
- User passwords are hashed with a salt before storage.
- The CORS policy in development allows any origin. For production, restrict to specific origins.

## Development Notes

- The database is automatically created and seeded with an admin user when running in development mode.
- For production deployment, apply proper database migrations using Entity Framework Core.

## License

This project is licensed under the MIT License - see the LICENSE file for details.
