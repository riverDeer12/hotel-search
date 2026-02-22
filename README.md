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

## Prerequisites

- .NET 9 SDK installed  
- Optional: EF Core CLI tools  

Install EF Core CLI tools (if needed):

```bash
dotnet tool install --global dotnet-ef
```

---

## Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/riverDeer12/hotel-search.git
cd hotel-search
```

### 2. Restore Dependencies

```bash
dotnet restore
```

### 3. Run the Application

```bash
dotnet run --project ./HotelSearch.API/HotelSearch.API.csproj
```

The application will start locally and automatically create the `hotels.db` SQLite database file if it does not exist.

---

## API Documentation (Swagger)

Swagger is available when running in the **Development** environment.

Open in your browser:

```
https://localhost:<port>/swagger
```

If Swagger does not appear, ensure the environment is set to Development.

### Windows (PowerShell)

```powershell
$env:ASPNETCORE_ENVIRONMENT="Development"
dotnet run --project .\HotelSearch.API\HotelSearch.API.csproj
```

### macOS / Linux

```bash
export ASPNETCORE_ENVIRONMENT=Development
dotnet run --project ./HotelSearch.API/HotelSearch.API.csproj
```

---

## Database Configuration

The connection string is defined in `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Data Source=hotels.db"
}
```

The SQLite database file will be created in the project directory if it does not already exist.

---

## Entity Framework Commands

Apply existing migrations:

```bash
dotnet ef database update --project ./HotelSearch.API/HotelSearch.API.csproj
```

Create a new migration:

```bash
dotnet ef migrations add InitialCreate --project ./HotelSearch.API/HotelSearch.API.csproj
dotnet ef database update --project ./HotelSearch.API/HotelSearch.API.csproj
```

---

## Logging

Serilog is configured for structured logging.

Logs are written to:

- Console output
- Rolling daily log files located at:

```
HotelSearch.API/Logs/log-*.txt
```

---

## Testing

Run tests using:

```bash
dotnet test
```
