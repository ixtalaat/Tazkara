# Tazkara

Tazkara is a full-stack event marketplace for discovering events, reserving tickets, and managing the event lifecycle.

Customers can browse published events, reserve a ticket, complete a simulated checkout, and access a printable ticket voucher. Organizers can create and manage events, while administrators can monitor platform activity and maintain the event catalog.

> **Personal learning project**
>
> Tazkara is being developed as a personal project to learn and apply full-stack engineering practices: clean architecture, REST APIs, authentication, role-based authorization, relational data modeling, automated testing, and Angular application design. It is a portfolio and learning project, not a production ticketing service.

## What the project demonstrates

- Role-based workflows for customers, organizers, and administrators.
- JWT authentication backed by ASP.NET Core Identity.
- Event creation, editing, publishing, cancellation, and browsing.
- Ticket reservation with availability and duplicate-booking protection.
- Simulated payment completion and ticket voucher generation.
- Organizer sales and reservation metrics.
- Admin category management and event-review foundations.
- Environment-aware development data seeding.
- Unit tests and API authorization integration tests.

## Technology

| Area | Technologies |
| --- | --- |
| Frontend | Angular 22, TypeScript, Angular Router, reactive forms, SweetAlert2 |
| API | ASP.NET Core Web API on .NET 10 |
| Application architecture | Clean separation of API, application, domain, infrastructure, and shared layers |
| Data | Entity Framework Core 10, SQL Server, ASP.NET Core Identity |
| Security | JWT bearer authentication, role-based authorization, validation, security headers, rate limiting |
| Observability | Serilog and health checks |
| Testing | xUnit, Moq, FluentAssertions, ASP.NET Core integration testing |

## Repository layout

```text
Tazkara.API/                 HTTP API, controllers, middleware, configuration
Tazkara.Application/         Use cases, DTOs, validators, interfaces, mappings
Tazkara.Domain/              Domain entities and enums
Tazkara.Infrastructure/      EF Core, repositories, identity, payment services
Tazkara.Shared/              Shared response and utility types
Tazkara.Application.Tests/   Application unit tests and API integration tests
Tazkara.Web/                 Angular client application
docs/                        Product requirements and integration notes
```

## Requirements

- .NET SDK 10
- Node.js and npm
- SQL Server LocalDB or another SQL Server instance
- Optional: Docker Desktop

## Quick start

### 1. Configure the API

Set `ConnectionStrings:DefaultConnection` in `Tazkara.API/appsettings.Development.json` or use .NET user secrets. Keep JWT keys and database credentials out of source control for shared or deployed environments.

Development seeding is controlled by the `DatabaseSeed` section in `appsettings.Development.json`. When enabled, it creates development roles, users, categories, and sample events. Seeding is disabled by default outside Development.

### 2. Start the API

```powershell
dotnet run --project Tazkara.API
```

### 3. Start the Angular client

```powershell
cd Tazkara.Web
npm install
npm start
```

Then open [http://localhost:4200](http://localhost:4200).

## Verification commands

Run the backend test suite:

```powershell
dotnet test Tazkara.Application.Tests/Tazkara.Application.Tests.csproj
```

Build the frontend:

```powershell
cd Tazkara.Web
npm run build
```

## Project status

The core customer, organizer, and admin workflows are implemented. Payment processing is intentionally simulated, and the project is still evolving as part of the learning process. Some production concerns—such as real payment integration, transactional email, deployment secrets, and operational monitoring—remain future work.

## Roadmap

- Add a real payment provider behind the existing payment abstraction.
- Add admin approval and rejection for submitted events.
- Add QR-code scanning and ticket validation at event entry.
- Expand analytics, reporting, filtering, search, and pagination.
- Add email notifications for reservations, payments, and event changes.
- Improve end-to-end coverage and CI/CD automation.
- Prepare a production deployment with managed secrets and monitoring.

## License

Tazkara is provided for personal learning and experimentation. It is not offered as a production service and comes without warranty.
