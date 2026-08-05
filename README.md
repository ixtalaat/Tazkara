# Tazkara

Tazkara is a full-stack event discovery and ticket reservation platform built with ASP.NET Core and Angular. Users can browse events, reserve tickets, complete a simulated checkout, and view printable ticket vouchers. Organizers can manage events and track sales, while administrators can review marketplace activity and manage categories.

> **Personal learning project**
>
> Tazkara is a personal project created to learn and practice modern full-stack application development, clean architecture, authentication, role-based authorization, database design, testing, and Angular UI development. It is not currently intended as a production ticketing service.

## Highlights

- Customer registration, login, event browsing, reservations, simulated payments, and ticket vouchers.
- Organizer dashboard with event creation, editing, publishing, cancellation, and sales metrics.
- Administrator dashboard with platform metrics, category management, and event review foundations.
- JWT authentication with Customer, Organizer, and Admin roles.
- SQL Server persistence through Entity Framework Core.
- Environment-driven development data seeding.
- Service-layer unit tests and API authorization integration tests.
- Responsive dark-themed Angular interface.

## Technology stack

### Backend

- .NET 10 / ASP.NET Core Web API
- Entity Framework Core 10 and SQL Server
- ASP.NET Core Identity and JWT bearer authentication
- FluentValidation
- Serilog
- xUnit, Moq, and FluentAssertions

### Frontend

- Angular 22
- TypeScript
- Angular Router and reactive forms
- SweetAlert2 for confirmation and feedback dialogs

## Solution structure

```text
Tazkara.API/                 Web API, middleware, controllers, configuration
Tazkara.Application/         DTOs, interfaces, validators, services, mappings
Tazkara.Domain/              Entities and domain enums
Tazkara.Infrastructure/      EF Core context, repositories, identity, payments
Tazkara.Shared/              Shared response and utility types
Tazkara.Application.Tests/   Unit and API integration tests
Tazkara.Web/                 Angular application
docs/                        Product and integration notes
```

## Prerequisites

- .NET SDK 10
- Node.js and npm
- SQL Server LocalDB or another SQL Server instance
- Optional: Docker Desktop

## Run locally

1. Configure the connection string in `Tazkara.API/appsettings.Development.json` or user secrets.
2. Start the API:

   ```powershell
   dotnet run --project Tazkara.API
   ```

3. In another terminal, install and start the Angular application:

   ```powershell
   cd Tazkara.Web
   npm install
   npm start
   ```

4. Open `http://localhost:4200`.

Development seeding is enabled through `DatabaseSeed` settings in `appsettings.Development.json`. It creates development-only roles, accounts, categories, and sample events. Replace these credentials before using a shared or deployed environment. Production seeding is disabled unless explicitly enabled through configuration.

## Test and build

Run backend tests:

```powershell
dotnet test Tazkara.Application.Tests/Tazkara.Application.Tests.csproj
```

Build the Angular application:

```powershell
cd Tazkara.Web
npm run build
```

## Current scope and future direction

The current implementation uses a simulated payment flow and is intended for learning. Future improvements may include real payment provider integration, email notifications, QR-code validation at entry, richer analytics, pagination and search improvements, production-ready secrets management, CI/CD, and deployment monitoring.

## License

This project is provided for personal learning and experimentation. No production support or warranty is implied.
