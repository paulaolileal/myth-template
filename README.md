# Myth Template API

[![.NET](https://img.shields.io/badge/.NET-8.0-purple.svg)](https://dotnet.microsoft.com/download/dotnet/8.0)
[![Myth Framework](https://img.shields.io/badge/Myth-blue.svg)](https://github.com/paulaolileal/myth)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)

> 🚀 **Enterprise-grade ASP.NET Core API template showcasing the power of the Myth ecosystem**

A production-ready template project demonstrating advanced architectural patterns, clean code principles, and the capabilities of the **Myth Framework**. This template serves as a blueprint for building scalable, maintainable REST APIs with enterprise-level quality and best practices.

## 📋 Table of Contents

- [Overview](#-overview)
- [Architecture](#-architecture)
- [Key Features](#-key-features)
- [Technology Stack](#-technology-stack)
- [Getting Started](#-getting-started)
- [Project Structure](#-project-structure)
- [Design Patterns](#-design-patterns)
- [Myth Framework Benefits](#-myth-framework-benefits)
- [API Documentation](#-api-documentation)
- [Examples](#-examples)
- [Best Practices Implemented](#-best-practices-implemented)
- [Testing](#-testing)
- [Configuration](#-configuration)
- [Contributing](#-contributing)
- [License](#-license)

## 🎯 Overview

The **Myth Template API** is a comprehensive demonstration of enterprise software architecture using the **Myth Framework**. It implements a Weather Forecast API with full CRUD operations, showcasing:

- **Clean Architecture** with distinct domain, application, and infrastructure layers
- **Domain-Driven Design (DDD)** principles with aggregate roots and value objects
- **CQRS (Command Query Responsibility Segregation)** pattern
- **Event-Driven Architecture** with domain events
- **Repository and Specification patterns**
- **Comprehensive validation** with business rules
- **Type-safe object mapping** and transformations
- **External API integration** with REST clients
- **Production-ready logging, error handling, and configuration**

### Why Use This Template?

- ✅ **Accelerated Development**: Skip months of architectural decisions and setup
- ✅ **Production Ready**: Battle-tested patterns and configurations
- ✅ **Scalable**: Clean separation of concerns enables independent scaling
- ✅ **Maintainable**: SOLID principles and clear structure reduce technical debt
- ✅ **Testable**: Dependency injection and repository patterns enable easy testing
- ✅ **Enterprise Quality**: Comprehensive validation, logging, and error handling

## 🏗️ Architecture

This template follows **Clean Architecture** principles with clear separation of concerns:

```
┌─────────────────────────────────────────────────────────┐
│                     API Layer                           │
│  ┌─────────────────────────────────────────────────┐    │
│  │              Controllers                        │    │
│  │  • HTTP Endpoints                               │    │
│  │  • Request/Response Transformation              │    │
│  │  • Myth.Flow Pipelines                          │    │
│  └─────────────────────────────────────────────────┘    │
└─────────────────────────────────────────────────────────┘
┌─────────────────────────────────────────────────────────┐
│                 Application Layer                       │
│  ┌─────────────────┐ ┌────────────────┐ ┌─────────────┐ │
│  │    Commands     │ │     Queries    │ │   Events    │ │
│  │  • Create       │ │  • GetAll      │ │  • Created  │ │
│  │  • Update       │ │  • GetById     │ │  • Updated  │ │
│  │  • Delete       │ │                │ │  • Deleted  │ │
│  └─────────────────┘ └────────────────┘ └─────────────┘ │
│  ┌────────────────────────────────────────────────────┐ │
│  │                    DTOs & Handlers                 │ │
│  └────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────┘
┌─────────────────────────────────────────────────────────┐
│                   Domain Layer                          │
│  ┌────────────────────────────────────────────────────┐ │
│  │  • Aggregate Roots (WeatherForecast)               │ │
│  │  • Value Objects (Summary, DateOnly)               │ │
│  │  • Domain Events                                   │ │
│  │  • Business Rules & Specifications                 │ │
│  │  • Repository Interfaces                           │ │
│  └────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────┘
┌─────────────────────────────────────────────────────────┐
│                Infrastructure Layer                     │
│  ┌─────────────────┐ ┌─────────────────┐ ┌────────────┐ │
│  │      Data       │ │  External Data  │ │    Tests   │ │
│  │  • EF Context   │ │  • REST Clients │ │  • Unit    │ │
│  │  • Repositories │ │  • External APIs│ │  • Integra-│ │
│  │  • Mappings     │ │  • Adapters	 │ │  tion      │ │
│  └─────────────────┘ └─────────────────┘ └────────────┘ │
└─────────────────────────────────────────────────────────┘
```

### Architectural Benefits

| Benefit | Description |
|---------|-------------|
| **Separation of Concerns** | Each layer has distinct responsibilities and dependencies flow inward |
| **Testability** | Domain logic isolated from infrastructure; easy to mock dependencies |
| **Maintainability** | Changes in one layer don't affect others; clear boundaries |
| **Scalability** | Layers can be scaled independently; clear performance bottlenecks |
| **Flexibility** | Easy to swap implementations (database, external services) |

## ✨ Key Features

### 🎯 Domain-Driven Design (DDD)

- **Aggregate Roots**: `WeatherForecast` with encapsulated business logic
- **Value Objects**: `Summary` enum, `DateOnly` for type safety
- **Domain Events**: Automatic event publishing for business actions
- **Specifications**: Reusable, composable query logic
- **Fluent API**: Intuitive domain model interactions

```csharp
// Example: Creating a weather forecast with business rules
var forecast = new WeatherForecast(date, temperatureC, Summary.Warm)
    .ChangeTemperatureC(25)
    .ChangeSummary(Summary.Mild);
```

### 🔄 CQRS Implementation

**Complete separation of Commands (writes) and Queries (reads):**

| Operation | Type | Handler | Validation | Events |
|-----------|------|---------|------------|--------|
| Create | Command | `CreateWeatherForecastCommandHandler` | ✅ Business Rules | ✅ Created Event |
| Update | Command | `UpdateWeatherForecastCommandHandler` | ✅ Existence Check | ✅ Updated Event |
| Delete | Command | `DeleteWeatherForecastCommandHandler` | ✅ Existence Check | ✅ Deleted Event |
| Get All | Query | `GetAllWeatherForecastsQueryHandler` | ✅ Filter Validation | ❌ Read-only |
| Get By ID | Query | `GetWeatherForecastsByIdQueryHandler` | ✅ ID Validation | ❌ Read-only |

### 📊 Advanced Filtering & Pagination

**Comprehensive query capabilities:**

- 🌡️ **Temperature Range**: Filter by min/max temperature (-100°C to 100°C)
- 📅 **Date Range**: Filter by date range with `DateOnly` precision
- 🌤️ **Weather Summary**: Filter by weather conditions (Freezing, Warm, Hot, etc.)
- 📄 **Pagination**: Page number and size with total count
- 🔄 **Ordering**: Results ordered by date (most recent first)
- 💾 **Caching**: Automatic query result caching

```csharp
// Example: Advanced filtering query
GET /api/v1/weatherforecast?summary=Warm&minimumDate=2024-01-01&maximumDate=2024-12-31&minimumTemperature=15&maximumTemperature=30&pageNumber=1&pageSize=20
```

### 🔔 Event-Driven Architecture

**Automatic domain event handling:**

```csharp
// Event published automatically on forecast creation
public record WeatherForecastCreatedEvent : DomainEvent
{
    public Guid WeatherForecastId { get; init; }
}

// Handler responds to events (loose coupling)
public class WeatherForecastCreatedEventHandler : IEventHandler<WeatherForecastCreatedEvent>
{
    public async Task HandleAsync(WeatherForecastCreatedEvent @event, CancellationToken cancellationToken)
    {
        // Log the creation
        _logger.LogInformation("Weather forecast created: {Id}", @event.WeatherForecastId);

        // Fetch random brewery recommendation 🍺
        var brewery = await _breweryRepository.GetRandomBreweryAsync(cancellationToken);
        _logger.LogInformation("Recommended brewery: {Name}", brewery.Name);
    }
}
```

### 🛡️ Comprehensive Validation

**Multi-layer validation with business rules:**

```csharp
// Fluent validation with async database checks
builder.For(Date, rules => rules
    .Past()                                    // Must be in the past
    .GreaterThan(DateOnly.MinValue)           // Valid date
    .RespectAsync(async (date, ct, sp) => {   // Async business rule
        var repository = sp.GetRequiredService<IWeatherForecastRepository>();
        var spec = SpecBuilder<WeatherForecast>.Create().WithDateNotInUse(date);
        return await repository.AllAsync(spec, ct);
    })
    .WithStatusCode(HttpStatusCode.Conflict)
    .WithMessage("Weather forecast for this date already exists"));

builder.For(TemperatureC, rules => rules
    .Between(-100, 100)                       // Realistic temperature range
    .WithMessage("Temperature must be between -100°C and 100°C"));
```

### 🔗 External API Integration

**Production-ready REST client integration:**

```csharp
// Configured REST client with automatic deserialization
builder.Services.AddRestConfiguration("brewery", conf => conf
    .WithBaseUrl("https://api.openbrewerydb.org/v1/")
    .WithBodyDeserialization(CaseStrategy.SnakeCase));

// Repository using fluent REST API
public async Task<BreweryResponseDto> GetRandomBreweryAsync(CancellationToken cancellationToken)
{
    var request = await _client
        .DoGet("breweries/random")
        .OnResult(res => res.UseTypeForSuccess<IEnumerable<BreweryResponseDto>>())
        .OnError(err => err.ThrowForNonSuccess())
        .BuildAsync(cancellationToken);

    return request.GetAs<IEnumerable<BreweryResponseDto>>().First();
}
```

## 🛠️ Technology Stack

### Core Framework
- **.NET 8.0** - Latest LTS version
- **ASP.NET Core 8.0** - High-performance web framework
- **Entity Framework Core 8.0** - Object-relational mapping

### Myth Framework Ecosystem (v3.0.5-preview.13)

| Package | Purpose | Key Benefits |
|---------|---------|--------------|
| **Myth.Commons** | Common utilities and extensions | Base classes, helper methods |
| **Myth.Flow** | Pipeline orchestration framework | Request/response pipelines, middleware |
| **Myth.Flow.Actions** | CQRS command/query dispatching | Auto-discovery of handlers, type-safe dispatch |
| **Myth.Guard** | Fluent validation library | Business rules, async validation, custom errors |
| **Myth.Morph** | Type-safe object mapping | Schema-based mapping, no reflection overhead |
| **Myth.Rest** | REST client factory | Fluent HTTP clients, configuration management |
| **Myth.Repository.EntityFramework** | Repository pattern with EF | Generic repositories, specifications, unit of work |

### Additional Dependencies
- **Swashbuckle.AspNetCore** - API documentation (Swagger/OpenAPI)
- **Microsoft.EntityFrameworkCore.InMemory** - In-memory database for development/testing

## 🚀 Getting Started

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- IDE: [Visual Studio 2022](https://visualstudio.microsoft.com/vs/), [JetBrains Rider](https://www.jetbrains.com/rider/), or [VS Code](https://code.visualstudio.com/)

### Quick Start

1. **Clone or use as template:**
   ```bash
   git clone https://github.com/your-org/myth-template-api.git
   cd myth-template-api
   ```

2. **Restore packages:**
   ```bash
   dotnet restore
   ```

3. **Run the application:**
   ```bash
   dotnet run --project Myth.Template.API
   ```

4. **Explore the API:**
   - 🌐 **Swagger UI**: [https://localhost:7296/swagger](https://localhost:7296/swagger)
   - 🔍 **Health Check**: [https://localhost:7296/health](https://localhost:7296/health)
   - ⚡ **Sample API**: [https://localhost:7296/api/v1/weatherforecast](https://localhost:7296/api/v1/weatherforecast)

### Development Setup

1. **Development database** (In-memory by default):
   - Automatically seeded with 1000 sample weather forecasts
   - Historical data for the past 1000 days
   - Ready for immediate testing

2. **API Documentation**:
   - Complete OpenAPI/Swagger specification
   - Request/response examples
   - Validation error descriptions

## 📁 Project Structure

```
Myth.Template.API/
├── 🏗️ Myth.Template.API/                    # Web API Layer
│   ├── Controllers/                         # HTTP endpoints
│   ├── Program.cs                           # Application startup
│   └── appsettings.json                     # Configuration
│
├── 🎯 Myth.Template.Domain/                 # Domain Layer (Business Logic)
│   ├── Models/                              # Aggregate roots & value objects
│   │   ├── WeatherForecast.cs              # Main aggregate root
│   │   └── Summary.cs                       # Value object (enum)
│   ├── Interfaces/                          # Repository contracts
│   └── Specifications/                      # Query specifications
│
├── 🔄 Myth.Template.Application/            # Application Layer (Use Cases)
│   ├── WeatherForecasts/
│   │   ├── Commands/                        # Write operations
│   │   │   ├── Create/                      # Create forecast
│   │   │   ├── Update/                      # Update forecast
│   │   │   └── Delete/                      # Delete forecast
│   │   ├── Queries/                         # Read operations
│   │   │   ├── GetAll/                      # List with filters
│   │   │   └── GetById/                     # Single forecast
│   │   ├── Events/                          # Domain events
│   │   └── DTOs/                            # Data transfer objects
│   └── InitializeFakeData.cs               # Development data seeding
│
├── 💾 Myth.Template.Data/                   # Data Access Layer
│   ├── Contexts/                            # Entity Framework contexts
│   ├── Mappings/                            # Entity configurations
│   └── Repositories/                        # Data access implementations
│
├── 🌐 Myth.Template.ExternalData/           # External Integrations
│   └── Breweries/                           # Sample external API integration
│
└── 🧪 Myth.Template.Test/                   # Test Projects
    └── WeatherForecastTests.cs              # Unit tests
```

### Layer Responsibilities

| Layer | Responsibilities | Dependencies |
|-------|-----------------|--------------|
| **API** | HTTP endpoints, request/response handling, authentication | Application |
| **Application** | Use cases, command/query handlers, DTOs, events | Domain |
| **Domain** | Business logic, aggregate roots, domain services, specifications | None |
| **Data** | Entity Framework, repositories, database mappings | Domain, Application |
| **ExternalData** | External service integration, REST clients | Application |
| **Test** | Unit tests, integration tests, test fixtures | All layers |

## 🎨 Design Patterns

This template demonstrates professional implementation of key design patterns:

### 🏛️ Repository Pattern

```csharp
// Generic repository with specifications
public interface IWeatherForecastRepository : IReadWriteRepositoryAsync<WeatherForecast>
{
    // Domain-specific methods can be added here
}

// Implementation with EF Core
public class WeatherForecastRepository : ReadWriteRepositoryAsync<WeatherForecast>, IWeatherForecastRepository
{
    public WeatherForecastRepository(ForecastContext context) : base(context) { }
}

// Usage in handlers
var forecasts = await _repository.SearchPaginatedAsync(specification, cancellationToken);
```

### 📋 Specification Pattern

```csharp
// Composable, reusable query logic
var spec = SpecBuilder<WeatherForecast>
    .Create()
    .WithSummary(query.Summary)                    // Optional filter
    .WithDateGreaterThan(query.MinimumDate)        // Optional filter
    .WithDateLowerThan(query.MaximumDate)          // Optional filter
    .WithTemparatureGreaterThan(query.MinimumTemperature) // Optional filter
    .WithTemparatureLowerThan(query.MaximumTemperature)   // Optional filter
    .OrderDescending(x => x.Date)                  // Consistent ordering
    .WithPagination(query);                        // Pagination

// Execute with type safety
var result = await repository.SearchPaginatedAsync(spec, cancellationToken);
```

### 🔄 Unit of Work Pattern

```csharp
// Transactional consistency across operations
public async Task<WeatherForecastCreatedEvent> HandleAsync(CreateWeatherForecastCommand command, CancellationToken cancellationToken)
{
    var weatherForecast = new WeatherForecast(command.Date, command.TemperatureC, command.Summary);

    await _repository.AddAsync(weatherForecast, cancellationToken);
    await _unitOfWork.SaveChangesAsync(cancellationToken);  // Single transaction

    return new WeatherForecastCreatedEvent { WeatherForecastId = weatherForecast.WeatherForecastId };
}
```

### 🎭 Command Pattern (CQRS)

```csharp
// Commands are immutable DTOs with validation
public record CreateWeatherForecastCommand : ICommand<WeatherForecastCreatedEvent>, IValidatable
{
    public DateOnly Date { get; init; }
    public int TemperatureC { get; init; }
    public Summary Summary { get; init; }

    public void Validate(ValidationBuilder<CreateWeatherForecastCommand> builder, ValidationContextKey? context = null)
    {
        // Fluent validation with business rules
    }
}

// Handlers have single responsibility
public class CreateWeatherForecastCommandHandler : ICommandHandler<CreateWeatherForecastCommand, WeatherForecastCreatedEvent>
{
    public async Task<WeatherForecastCreatedEvent> HandleAsync(CreateWeatherForecastCommand command, CancellationToken cancellationToken)
    {
        // Implementation
    }
}
```

### 🔔 Observer Pattern (Events)

```csharp
// Loose coupling through domain events
public record WeatherForecastCreatedEvent : DomainEvent
{
    public Guid WeatherForecastId { get; init; }
}

// Multiple handlers can respond to same event
public class WeatherForecastCreatedEventHandler : IEventHandler<WeatherForecastCreatedEvent>
{
    public async Task HandleAsync(WeatherForecastCreatedEvent @event, CancellationToken cancellationToken)
    {
        // Side effects: logging, notifications, external API calls
    }
}
```

## 💎 Myth Framework Benefits

The Myth Framework provides significant advantages over traditional ASP.NET Core development:

### 🚀 Development Velocity

| Traditional Approach | With Myth Framework |
|---------------------|-------------------|
| Manual pipeline setup | `PipelineExtensions.Start()` |
| Custom validation framework | `Myth.Guard` with fluent API |
| Manual object mapping | `Myth.Morph` type-safe mapping |
| HttpClient configuration | `Myth.Rest` fluent REST clients |
| Repository boilerplate | Generic repositories with specifications |
| Manual event handling | Automatic event discovery and dispatch |

### 🏗️ Pipeline Architecture

**Traditional Controller:**
```csharp
[HttpPost]
public async Task<IActionResult> CreateAsync([FromBody] CreateWeatherForecastRequest request)
{
    // Manual validation
    if (!ModelState.IsValid)
        return BadRequest(ModelState);

    // Manual mapping
    var command = new CreateWeatherForecastCommand
    {
        Date = request.Date,
        TemperatureC = request.TemperatureC,
        Summary = request.Summary
    };

    // Manual handler invocation
    var result = await _handler.HandleAsync(command);

    // Manual event publishing
    await _eventPublisher.PublishAsync(new WeatherForecastCreatedEvent { Id = result.Id });

    return CreatedAtAction(nameof(GetByIdAsync), new { id = result.Id }, result);
}
```

**With Myth Pipeline:**
```csharp
[HttpPost]
public async Task<IActionResult> CreateAsync([FromBody] CreateWeatherForecastRequest request, CancellationToken cancellationToken)
{
    var result = await PipelineExtensions
        .Start(request.To<CreateWeatherForecastCommand>())                  // Type-safe mapping
        .TapAsync(pipeline => _validator.ValidateAsync(pipeline.CurrentRequest!)) // Automatic validation
        .Tap(pipeline => _logger.LogDebug("Command validated successfully"))      // Side effects
        .Process<CreateWeatherForecastCommand, WeatherForecastCreatedEvent>()     // Handler dispatch
        .Publish()                                                               // Event publishing
        .Tap(pipeline => _logger.LogInformation("Weather forecast created: {Id}",
            pipeline.CurrentRequest!.WeatherForecastId))                        // Success logging
        .ExecuteAsync(cancellationToken);                                       // Async execution

    return result.Match(
        success => CreatedAtAction(nameof(GetByIdAsync),
            new { id = success.WeatherForecastId }, success),
        error => StatusCode((int)error.StatusCode, error.Message));
}
```

### 🎯 Benefits Breakdown

| Feature | Traditional | Myth Framework | Benefit |
|---------|-------------|----------------|---------|
| **Validation** | Manual `ModelState` checks | Automatic with `Myth.Guard` | Type-safe, business rules, async validation |
| **Mapping** | Manual property assignment | `request.To<Command>()` | Zero configuration, type-safe |
| **Logging** | Scattered `_logger` calls | Pipeline `.Tap()` | Consistent, structured logging |
| **Error Handling** | Try-catch blocks | Built-in pipeline error handling | Centralized, consistent responses |
| **Event Publishing** | Manual event dispatcher calls | Automatic with `.Publish()` | Zero configuration, automatic discovery |
| **Caching** | Manual cache implementation | `.UseCache()` configuration | Declarative, configurable |
| **Retries** | Custom retry logic | `.UseRetry(3)` configuration | Exponential backoff, circuit breaker |
| **Telemetry** | Manual performance tracking | `.UseTelemetry()` | Automatic metrics, tracing |

### 🔄 Object Mapping Comparison

**Traditional AutoMapper:**
```csharp
// Configuration required
CreateMap<CreateWeatherForecastRequest, CreateWeatherForecastCommand>();
CreateMap<WeatherForecast, GetWeatherForecastResponse>()
    .ForMember(dest => dest.SummaryDescription, opt => opt.MapFrom(src => Enum.GetName(src.Summary)))
    .ForMember(dest => dest.SummaryId, opt => opt.MapFrom(src => (int)src.Summary));

// Runtime mapping (potential errors)
var command = _mapper.Map<CreateWeatherForecastCommand>(request);
```

**Myth.Morph Schema-Based:**
```csharp
// Type-safe compile-time mapping
public record CreateWeatherForecastRequest : IMorphableTo<CreateWeatherForecastCommand>
{
    public void MorphTo(Schema<CreateWeatherForecastCommand> schema)
    {
        // Automatic property matching, custom logic only when needed
    }
}

public record GetWeatherForecastResponse : IMorphableFrom<WeatherForecast>
{
    public void MorphFrom(Schema<WeatherForecast> schema)
    {
        schema.Bind(() => SummaryDescription, src => Enum.GetName(src.Summary));
        schema.Bind(() => SummaryId, src => (int)src.Summary);
    }
}

// Compile-time safe mapping
var command = request.To<CreateWeatherForecastCommand>();
var response = weatherForecast.To<GetWeatherForecastResponse>();
```

## 📚 API Documentation

### Swagger/OpenAPI Integration

Complete API documentation with:
- 📖 **Interactive Documentation**: Swagger UI with try-it-out functionality
- 🔍 **Schema Definitions**: Request/response models with validation rules
- ✅ **Response Examples**: Sample requests and responses for all endpoints
- ❌ **Error Responses**: Detailed error schemas with status codes

### API Endpoints Overview

| Method | Endpoint | Description | Request | Response |
|--------|----------|-------------|---------|----------|
| **POST** | `/api/v1/weatherforecast` | Create forecast | `CreateWeatherForecastRequest` | `201 Created` + `Location` |
| **GET** | `/api/v1/weatherforecast` | List forecasts | Query parameters | `IPaginated<GetWeatherForecastResponse>` |
| **GET** | `/api/v1/weatherforecast/{id}` | Get forecast | GUID in route | `GetWeatherForecastResponse` |
| **PUT** | `/api/v1/weatherforecast` | Update forecast | `UpdateWeatherForecastRequest` | `204 No Content` |
| **DELETE** | `/api/v1/weatherforecast` | Delete forecast | `DeleteWeatherForecastRequest` | `204 No Content` |

### Response Schema Example

```json
{
  "items": [
    {
      "weatherForecastId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "date": "2024-01-15",
      "temperatureC": 25,
      "temperatureF": 77,
      "summaryId": 6,
      "summaryDescription": "Warm",
      "createdAt": "2024-01-15T10:30:00Z",
      "updatedAt": null
    }
  ],
  "totalCount": 150,
  "pageNumber": 1,
  "pageSize": 20,
  "totalPages": 8,
  "hasPreviousPage": false,
  "hasNextPage": true
}
```

## 💡 Examples

### Creating a Weather Forecast

```bash
curl -X POST "https://localhost:7296/api/v1/weatherforecast" \
  -H "Content-Type: application/json" \
  -d '{
    "date": "2024-01-15",
    "temperatureC": 25,
    "summary": 6
  }'
```

**Response:**
```http
HTTP/1.1 201 Created
Location: /api/v1/weatherforecast/3fa85f64-5717-4562-b3fc-2c963f66afa6
Content-Type: application/json

{
  "weatherForecastId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

### Advanced Filtering Query

```bash
# Get warm weather forecasts from last year, paginated
curl "https://localhost:7296/api/v1/weatherforecast?summary=6&minimumDate=2023-01-01&maximumDate=2023-12-31&minimumTemperature=20&maximumTemperature=30&pageNumber=1&pageSize=10"
```

### Validation Error Example

```bash
curl -X POST "https://localhost:7296/api/v1/weatherforecast" \
  -H "Content-Type: application/json" \
  -d '{
    "date": "2025-01-15",
    "temperatureC": 150,
    "summary": 999
  }'
```

**Response:**
```http
HTTP/1.1 400 Bad Request
Content-Type: application/json

{
  "errors": [
    {
      "field": "Date",
      "message": "Date must be in the past",
      "code": "PAST_DATE_REQUIRED"
    },
    {
      "field": "TemperatureC",
      "message": "Temperature must be between -100°C and 100°C",
      "code": "TEMPERATURE_OUT_OF_RANGE"
    },
    {
      "field": "Summary",
      "message": "Invalid weather summary value",
      "code": "INVALID_ENUM_VALUE"
    }
  ]
}
```

## 🏆 Best Practices Implemented

### SOLID Principles

- ✅ **Single Responsibility**: Each handler, repository, and service has one clear purpose
- ✅ **Open/Closed**: Pipeline extensions allow adding behavior without modifying core logic
- ✅ **Liskov Substitution**: Generic handlers implement standard interfaces consistently
- ✅ **Interface Segregation**: Specific interfaces for each concern (repository, validation, mapping)
- ✅ **Dependency Inversion**: All dependencies injected, depending on abstractions not concretions

### Clean Code Practices

- ✅ **Meaningful Names**: `GetAllWeatherForecastsQueryHandler` clearly describes purpose
- ✅ **Small Functions**: Average method length ~30 lines, single responsibility
- ✅ **No Magic Numbers**: Constants and enums for meaningful values
- ✅ **DRY Principle**: Shared logic in specifications and base classes
- ✅ **Consistent Formatting**: `.editorconfig` enforces team standards

### Enterprise Patterns

- ✅ **Domain-Driven Design**: Aggregate roots, value objects, domain services
- ✅ **CQRS**: Clear separation of read and write operations
- ✅ **Event Sourcing**: Domain events capture business actions
- ✅ **Repository Pattern**: Abstraction over data access with specifications
- ✅ **Unit of Work**: Transactional consistency across operations

### Security & Production Readiness

- ✅ **Input Validation**: Comprehensive validation with business rules
- ✅ **Error Handling**: Consistent error responses with proper HTTP status codes
- ✅ **Logging**: Structured logging with correlation IDs
- ✅ **Health Checks**: Endpoint monitoring for production deployments
- ✅ **Configuration**: Environment-specific settings with validation

## 🧪 Testing

### Test Structure

The template includes a foundation for comprehensive testing:

```
Myth.Template.Test/
├── Unit Tests/
│   ├── Handlers/              # Command and query handler tests
│   ├── Validators/            # Validation logic tests
│   ├── Specifications/        # Query specification tests
│   └── Domain/               # Aggregate root behavior tests
├── Integration Tests/
│   ├── API/                  # End-to-end API tests
│   ├── Database/             # Repository integration tests
│   └── ExternalServices/     # External API integration tests
└── TestFixtures/             # Shared test utilities and data
```

### Testing Benefits of This Architecture

| Component | Testing Approach | Benefits |
|-----------|------------------|----------|
| **Domain Models** | Unit tests with pure functions | No dependencies, fast execution |
| **Handlers** | Unit tests with mocked repositories | Isolated business logic testing |
| **Repositories** | Integration tests with in-memory DB | Real data access testing |
| **Specifications** | Unit tests with sample data | Query logic verification |
| **Validators** | Unit tests with various inputs | Business rule verification |
| **Controllers** | Integration tests with test client | End-to-end API testing |

### Sample Test Examples

```csharp
[Test]
public async Task CreateWeatherForecast_ValidData_ReturnsCreatedEvent()
{
    // Arrange
    var command = new CreateWeatherForecastCommand
    {
        Date = DateOnly.FromDateTime(DateTime.Now.AddDays(-1)),
        TemperatureC = 25,
        Summary = Summary.Warm
    };

    // Act
    var result = await _handler.HandleAsync(command, CancellationToken.None);

    // Assert
    Assert.That(result.WeatherForecastId, Is.Not.EqualTo(Guid.Empty));
}

[Test]
public async Task CreateWeatherForecast_DuplicateDate_ThrowsValidationException()
{
    // Arrange
    var existingDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-1));
    await SeedWeatherForecast(existingDate);

    var command = new CreateWeatherForecastCommand
    {
        Date = existingDate,
        TemperatureC = 20,
        Summary = Summary.Cool
    };

    // Act & Assert
    var exception = await Assert.ThrowsAsync<ValidationException>(() =>
        _handler.HandleAsync(command, CancellationToken.None));

    Assert.That(exception.Errors.First().Code, Is.EqualTo("CONFLICT"));
}
```

## ⚙️ Configuration

### Application Settings

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=weather_forecast.db"
  },
  "ExternalApis": {
    "BreweryApi": {
      "BaseUrl": "https://api.openbrewerydb.org/v1/",
      "Timeout": "00:00:30"
    }
  },
  "Cache": {
    "DefaultExpiration": "00:05:00",
    "QueryCacheExpiration": "00:02:00"
  },
  "Pagination": {
    "DefaultPageSize": 20,
    "MaxPageSize": 100
  }
}
```

### Environment Configuration

| Environment | Database | Logging Level | Cache | External APIs |
|-------------|----------|---------------|-------|---------------|
| **Development** | In-Memory | Debug | Disabled | Real APIs |
| **Testing** | In-Memory | Warning | Disabled | Mocked |
| **Staging** | SQL Server | Information | Redis | Real APIs |
| **Production** | SQL Server | Warning | Redis | Real APIs |

### Startup Configuration

```csharp
// Database configuration
builder.Services.AddDbContext<ForecastContext>(options =>
{
    if (builder.Environment.IsDevelopment())
        options.UseInMemoryDatabase("WeatherForecastDb");
    else
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Myth Framework configuration
builder.Services.AddFlow(config => config
    .UseLogging()                              // Request/response logging
    .UseExceptionFilter<ValidationException>() // Validation error handling
    .UseTelemetry()                           // Performance metrics
    .UseRetry(retryCount: 3)                  // Automatic retry with backoff
    .UseCache(defaultExpiration: TimeSpan.FromMinutes(5)) // Result caching
    .UseActions(x => x
        .UseInMemory()                        // In-memory action store
        .ScanAssemblies(typeof(CreateWeatherForecastCommandHandler).Assembly)));

// REST client configuration
builder.Services.AddRestFactory()
    .AddRestConfiguration("brewery", conf => conf
        .WithBaseUrl(builder.Configuration["ExternalApis:BreweryApi:BaseUrl"])
        .WithTimeout(TimeSpan.Parse(builder.Configuration["ExternalApis:BreweryApi:Timeout"]))
        .WithBodyDeserialization(CaseStrategy.SnakeCase));
```

## 🤝 Contributing

We welcome contributions! This template serves as both a reference implementation and a starting point for your own projects.

### How to Contribute

1. **Fork the repository**
2. **Create a feature branch**: `git checkout -b feature/amazing-feature`
3. **Follow the coding conventions** defined in `.editorconfig`
4. **Add tests** for new functionality
5. **Update documentation** if needed
6. **Submit a pull request**

### Coding Standards

- Follow the **SOLID principles** and **Clean Code** practices
- Use **meaningful variable names** and **single-responsibility functions**
- Add **XML documentation** for all public methods and classes
- Write **comprehensive tests** for new features
- Ensure **proper error handling** and **validation**

### Areas for Contribution

- 🧪 **Additional test examples** and testing utilities
- 📚 **Enhanced documentation** and tutorials
- 🔧 **Additional design pattern implementations**
- 🌐 **More external API integration examples**
- ⚡ **Performance optimizations** and benchmarks
- 🔒 **Security enhancements** and authentication examples

## 📄 License

This project is licensed under the **MIT License** - see the [LICENSE](LICENSE) file for details.

---

## 🎯 Summary

The **Myth Template API** demonstrates enterprise-grade software architecture with:

- ✅ **Clean Architecture** ensuring maintainable, testable code
- ✅ **Domain-Driven Design** with rich domain models and business logic
- ✅ **CQRS Pattern** separating read and write operations
- ✅ **Event-Driven Architecture** enabling loose coupling and extensibility
- ✅ **Comprehensive Validation** with business rules and async checks
- ✅ **Type-Safe Mapping** eliminating runtime mapping errors
- ✅ **Production-Ready Features** including logging, error handling, and monitoring
- ✅ **Myth Framework Integration** accelerating development with proven patterns

**Perfect for:**
- Enterprise API development
- Learning modern .NET patterns
- Team architecture standards
- Production-ready project foundation

**Start building your next enterprise API with the power of the Myth ecosystem! 🚀**

---

*For questions, issues, or contributions, please visit our [GitHub repository](https://github.com/your-org/myth-template-api) or check the [Myth Framework documentation](https://docs.mythframework.io).*
