# Order Management API

REST API for order management built with .NET 10, following Clean Architecture principles and CQRS.

The application allows authenticated users to create, retrieve, list, and cancel orders.

## Technologies

- .NET 10
- ASP.NET Core Web API
- Entity Framework Core
- SQLite
- MediatR
- FluentValidation
- JWT Bearer Authentication
- Serilog
- OpenTelemetry
- xUnit
- Docker

## Architecture

The solution is organized into four main projects:

```text
src/
├── OrderManagement.Domain
├── OrderManagement.Application
├── OrderManagement.Infrastructure
└── OrderManagement.Api

tests/
├── OrderManagement.UnitTests
└── OrderManagement.IntegrationTests
```

### Domain

Contains the core business entities and domain rules.

Main entities:

- `Order`
- `OrderItem`
- `OrderStatus`

The domain does not depend on infrastructure or external frameworks.

### Application

Contains the application use cases using CQRS with MediatR.

Commands:

- `CreateOrderCommand`
- `CancelOrderCommand`

Queries:

- `GetOrdersQuery`
- `GetOrderByIdQuery`

Cross-cutting concerns such as validation and logging are implemented through MediatR pipeline behaviors.

### Infrastructure

Contains persistence concerns:

- Entity Framework Core
- SQLite
- Entity configurations
- Repository implementation
- Database migrations

### API

Responsible for exposing the application through HTTP.

It contains:

- Controllers
- JWT authentication
- Authorization
- Swagger
- Global exception handling
- Dependency injection configuration

## Authentication

The API uses JWT Bearer authentication.

First obtain a token using:

```http
POST /auth/login
```

Then send the returned token in protected endpoints:

```http
Authorization: Bearer <token>
```

Swagger also supports JWT authentication through the **Authorize** button.

## Orders

The API provides the following operations:

```text
POST   /api/orders
GET    /api/orders
GET    /api/orders/{id}
PUT    /api/orders/{id}/cancel
```

Order listing supports pagination through query parameters:

```text
GET /api/orders?page=1&pageSize=10
```

## Running locally

Restore dependencies:

```bash
dotnet restore
```

Apply migrations:

```bash
dotnet ef database update \
  --project src/OrderManagement.Infrastructure \
  --startup-project src/OrderManagement.Api
```

Run the API:

```bash
dotnet run --project src/OrderManagement.Api
```

In Development, Swagger is available at:

```text
http://localhost:5161/swagger
```

## Running with Docker

Build the image:

```bash
docker build -t order-management-api .
```

Run using Docker Compose:

```bash
docker compose up --build
```

The API will be available at:

```text
http://localhost:8080
```

When running in the Development environment, Swagger is available at:

```text
http://localhost:8080/swagger
```

SQLite data is persisted using a Docker volume.

Stop the containers:

```bash
docker compose down
```

To also remove the persisted database volume:

```bash
docker compose down -v
```

## Tests

Run all tests:

```bash
dotnet test
```

The solution contains unit tests for domain/application behavior and integration tests using `WebApplicationFactory`.

Integration tests execute HTTP requests against the ASP.NET Core application and validate the behavior of the API pipeline.

## Validation

Input validation is implemented with FluentValidation.

Validation is executed through a MediatR pipeline behavior before commands or queries reach their handlers.

This keeps validation separate from controllers and application handlers.

## Error Handling

Exceptions are handled globally using ASP.NET Core exception handling and Problem Details.

This provides consistent HTTP error responses without requiring repetitive `try/catch` blocks in controllers.

## Logging

Structured logging is implemented with Serilog.

HTTP requests are logged using Serilog request logging, while application commands and queries are logged through a MediatR pipeline behavior.

The application logs:

- Request/command name
- Execution time
- Errors

## Observability

Basic distributed tracing is configured with OpenTelemetry.

ASP.NET Core requests are automatically instrumented and traces are exported to the console.

Each trace contains information such as:

- Trace ID
- Span ID
- HTTP route
- HTTP method
- Status code
- Request duration

The console exporter is intentionally used to keep the project infrastructure simple while demonstrating the observability setup.

## Technical Decisions

### Clean Architecture

The solution separates domain rules, application use cases, infrastructure concerns, and the HTTP API.

Dependencies point toward the application core, keeping domain logic independent from persistence and presentation concerns.

### CQRS

Commands and queries are represented explicitly using MediatR.

For the current size of the application, CQRS is not strictly required, but it provides clear separation between operations and allows cross-cutting concerns such as validation and logging to be handled consistently.

### Repository

Persistence access is abstracted behind a repository so application handlers do not depend directly on Entity Framework Core.

### SQLite

SQLite was chosen because it satisfies the requirements while keeping local execution and evaluation simple.

For a production system with higher concurrency or scalability requirements, a database such as PostgreSQL would be more appropriate.

### Docker

The API uses a multi-stage Docker build.

The .NET SDK image is used only for restoring and publishing the application, while the final container uses the smaller ASP.NET Core runtime image.

A Docker volume is used to persist the SQLite database outside the container lifecycle.

## Possible Production Improvements

For a production environment, some additional improvements would include:

- Store JWT secrets outside source control using environment variables or a secret manager
- Use a production-grade relational database such as PostgreSQL
- Export OpenTelemetry data to an observability backend
- Add health checks
- Add rate limiting where appropriate
- Add CI/CD
- Expand integration and edge-case test coverage