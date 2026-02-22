# Hotel Search API

A lightweight Hotel Search backend built with **.NET 9** using **FastEndpoints**.  
The application uses **Entity Framework Core with SQLite** for data persistence and **Serilog** for structured logging. Swagger is enabled in development for API exploration.

This project demonstrates modern .NET API development with clean configuration, minimal setup, and portable database support.

---

## Overview

The Hotel Search API is a RESTful backend service designed to manage and search hotel data.

### Key Characteristics

- Uses **FastEndpoints** instead of traditional Controllers
- Implements **Entity Framework Core** as ORM
- Uses **SQLite** as a file-based database
- Provides structured logging via **Serilog**
- Includes **Swagger (OpenAPI)** documentation in development mode

---

## Tech Stack

| Technology | Purpose |
|------------|----------|
| .NET 9 | Application framework |
| FastEndpoints | Endpoint-based API structure |
| Entity Framework Core | Data access layer |
| SQLite | Lightweight database |
| Serilog | Structured logging |
| Swagger / OpenAPI | API documentation |

---


### Main Components

**Program.cs**
- Registers FastEndpoints
- Configures EF Core with SQLite
- Enables Swagger (Development only)
- Configures Serilog logging
- Enables HTTPS redirection
- Enables CORS

**appsettings.json**
- Contains connection strings
- Contains Serilog configuration

**SQLite Database**
- File-based database (`hotels.db`)
- Created automatically if not present

---

## Application Architecture

The `HotelSearch.API` project follows a clean, minimal **vertical slice architecture** using FastEndpoints.

The project is organized by responsibility:

- **Endpoints** – API route definitions
- **Entities** – Domain models mapped to the database
- **Database** – EF Core `DbContext` configuration
- **Middleware (if present)** – Custom request pipeline logic
- **Program.cs** – Application bootstrap and service registration

This structure keeps features self-contained and avoids unnecessary layering.

---

## FastEndpoints Usage

Instead of traditional Controllers, this project uses FastEndpoints.

Each endpoint:

- Inherits from `Endpoint<TRequest, TResponse>`
- Defines route configuration inside `Configure()`
- Implements business logic inside `HandleAsync()`

### Benefits

- Clear feature separation
- Reduced boilerplate
- Improved readability
- Easier unit testing
- Vertical slice architecture support

### Example (Conceptual)

```csharp
public class SearchHotelsEndpoint : Endpoint<SearchRequest, SearchResponse>
{
    public override void Configure()
    {
        Get("/hotels/search");
        AllowAnonymous();
    }

    public override async Task HandleAsync(SearchRequest req, CancellationToken ct)
    {
        // Business logic here
    }
}
```
