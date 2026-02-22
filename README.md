# Hotel Search API

A lightweight Hotel Search backend built with .NET 9 using FastEndpoints.  
The application uses Entity Framework Core with SQLite for data persistence and Serilog for structured logging. Swagger is enabled in development for API exploration.

This project demonstrates modern .NET API development with clean configuration, minimal setup, and portable database support.

---

## Overview

The Hotel Search API is a RESTful backend service designed to manage and search hotel data.

The solution uses:

- FastEndpoints instead of traditional Controllers
- Entity Framework Core as ORM
- SQLite as a file-based database
- Serilog for structured logging
- Swagger (OpenAPI) for documentation
- 
---

## Tech Stack

- .NET 9
- FastEndpoints
- Entity Framework Core
- SQLite
- Serilog (Console + Rolling File Logging)
- Swagger / OpenAPI

---

## Project Structure

### Main Components

---

## Application Architecture

The `HotelSearch.API` project follows a clean and minimal vertical-slice structure using FastEndpoints.

The project is organized by responsibility:

- **Endpoints** – API route definitions (FastEndpoints)
- **Entities** – Domain models mapped to the database
- **Database** – EF Core DbContext configuration
- **Middleware (if present)** – Custom request pipeline logic
- **Program.cs** – Application bootstrap and service registration

This structure keeps each feature self-contained and avoids unnecessary layering.

---

## FastEndpoints Usage

Instead of traditional Controllers, this project uses FastEndpoints.

Each endpoint:

- Inherits from `Endpoint<TRequest, TResponse>` (or similar base)
- Defines route configuration inside `Configure()`
- Implements business logic inside `HandleAsync()`

Benefits of this approach:

- Clear separation per feature
- Reduced boilerplate
- Better readability
- Easier unit testing
- Vertical slice architecture

Example structure (conceptual):

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


Program.cs
- Registers FastEndpoints
- Configures EF Core with SQLite
- Enables Swagger (Development only)
- Configures Serilog logging
- Enables HTTPS redirection
- Enables CORS

appsettings.json
- Contains connection strings
- Contains Serilog configuration

SQLite Database
- File-based database (hotels.db)
- Created automatically if not present

---

## Prerequisites

- .NET 9 SDK installed
- Optional: EF Core CLI tools

To install EF CLI tools:

```bash
dotnet tool install --global dotnet-ef
```
## Getting Started

### 1. Clone the repository

```bash
git clone https://github.com/riverDeer12/hotel-search.git
cd hotel-search
```

### 2. Restore the dependencies

```bash
dotnet restore
```

### 3. Run the application

```bash
dotnet run --project ./HotelSearch.API/HotelSearch.API.csproj
```


### 4. Swagger / API Documentation

Open in your browser: https://localhost:<port>/swagger

### 5. Logging

Logs are written to:

- Console output

- Rolling daily log files located at: HotelSearch.API/Logs/log-*.txt


### 6. Testing

```bash
dotnet test
```




















