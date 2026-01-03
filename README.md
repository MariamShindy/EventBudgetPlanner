# Event Budget Planner

A full-stack web application for planning and managing event budgets, built with Angular and .NET 8.

## 🚀 Features

- **Event Management**: Create, edit, and manage events with budgets
- **Expense Tracking**: Track expenses by category and payment status
- **Budget Analysis**: View budget summaries, cashflow, and spending analytics
- **Event Templates**: Use pre-built templates to quickly create events
- **User Authentication**: Secure JWT-based authentication system
- **Profile Management**: Update user profile information
- **Event Sharing**: Share events with others via secure links
- **Reminders**: Set up email reminders for upcoming events

## 🛠️ Tech Stack

### Frontend
- **Angular 20** - Modern web framework
- **TypeScript** - Type-safe JavaScript
- **RxJS** - Reactive programming
- **SCSS** - Styling

### Backend
- **.NET 8** - Modern C# framework
- **Entity Framework Core** - ORM for database operations
- **SQL Server** - Database
- **JWT Authentication** - Secure token-based auth
- **Swagger/OpenAPI** - API documentation

## 📋 Prerequisites

Before you begin, ensure you have the following installed:

- **.NET 8 SDK** - [Download here](https://dotnet.microsoft.com/download/dotnet/8.0)
- **Node.js** (v18 or higher) - [Download here](https://nodejs.org/)
- **npm** (comes with Node.js) or **yarn**
- **SQL Server** or **SQL Server LocalDB** (for Windows)
- **Git** - [Download here](https://git-scm.com/)

## 🔧 Installation & Setup

### 1. Clone the Repository

```bash
git clone https://github.com/MariamShindy/EventBudgetPlanner.git
cd EventBudgetPlanner
```

### 2. Backend Setup

#### Navigate to Backend Directory

```bash
cd Backend/EventBudgetPlannerAPI
```

#### Create appsettings.json

The `appsettings.json` file is in `.gitignore` for security reasons. You need to create it manually.

Create a file named `appsettings.json` in `Backend/EventBudgetPlannerAPI/EventBudgetPlanner.API/` with the following content:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=EventBudgetPlanner;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True"
  },
  "JWT": {
    "Secret": "YOUR_SECRET_KEY_HERE_MINIMUM_64_CHARACTERS_LONG_FOR_SECURITY",
    "Issuer": "https://localhost:7199",
    "Audience": "http://localhost:4200",
    "ExpiryMinutes": "60"
  },
  "AppSettings": {
    "BaseUrl": "https://localhost:7199",
    "FrontUrl": "http://localhost:4200"
  },
  "Email": {
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": "587",
    "SmtpUser": "your-email@gmail.com",
    "SmtpPassword": "your-app-password",
    "FromEmail": "your-email@gmail.com",
    "FromName": "Event Budget Planner"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore.Database.Command": "Information"
    }
  },
  "AllowedHosts": "*"
}
```

**Important Notes:**
- Replace `YOUR_SECRET_KEY_HERE...` with a secure random string (minimum 64 characters). You can generate one using:
  ```bash
  openssl rand -base64 64
  ```
- For **Gmail SMTP**, you'll need to:
  1. Enable 2-Factor Authentication
  2. Generate an App Password: [Google App Passwords](https://myaccount.google.com/apppasswords)
  3. Use the App Password in `SmtpPassword` field
- For **SQL Server LocalDB** (Windows), the connection string should work out of the box
- For **SQL Server Express** or full SQL Server, update the connection string:
  ```
  Server=localhost\\SQLEXPRESS;Database=EventBudgetPlanner;Trusted_Connection=True;TrustServerCertificate=True
  ```

#### Restore Dependencies

```bash
dotnet restore
```

#### Run Database Migrations

The database will be automatically created and migrated when you run the application for the first time. The application includes a database seeder that will populate initial data.

#### Run the Backend

```bash
cd EventBudgetPlanner.API
dotnet run
```

The API will be available at:
- **HTTPS**: `https://localhost:7199`
- **HTTP**: `http://localhost:5281`
- **Swagger UI**: `https://localhost:7199/swagger`

### 3. Frontend Setup

#### Navigate to Frontend Directory

```bash
cd Frontend/EventBudgetPlanner
```

#### Install Dependencies

```bash
npm install
```

#### Configure Environment

The environment files are already configured, but you can modify them if needed:

- `src/app/environments/environment.ts` - Production environment
- `src/app/environments/environment.development.ts` - Development environment

Both point to `https://localhost:7199/api` by default. If your backend runs on a different port, update the `apiBaseUrl` accordingly.

#### Run the Frontend

```bash
npm start
```

The application will be available at `http://localhost:4200`

## 🎯 Usage

1. **Start the Backend**: Run the .NET API first
2. **Start the Frontend**: Run the Angular application
3. **Register/Login**: Create an account or login with existing credentials
4. **Create Events**: Start creating events and managing budgets
5. **Add Expenses**: Track expenses for each event
6. **View Analytics**: Check budget summaries and cashflow

## 📁 Project Structure

```
EventBudgetPlanner/
├── Backend/
│   └── EventBudgetPlannerAPI/
│       ├── EventBudgetPlanner.API/          # Web API layer
│       ├── EventBudgetPlanner.Application/   # Business logic layer
│       ├── EventBudgetPlanner.Domain/         # Domain entities
│       └── EventBudgetPlanner.Infrastructure/ # Data access layer
├── Frontend/
│   └── EventBudgetPlanner/
│       └── src/
│           ├── app/
│           │   ├── core/                     # Core services, guards, interceptors
│           │   ├── features/                  # Feature modules
│           │   └── shared/                    # Shared components
│           └── environments/                  # Environment configuration
```

