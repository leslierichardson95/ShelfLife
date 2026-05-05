# 📚 ShelfLife

A personal reading tracker built with .NET 10, Blazor, MudBlazor, Minimal API, and .NET Aspire. Search for books, manage your shelf, and track reading progress.

## Features

- **Search** — Find books via the Open Library API and add them to your shelf
- **Reading Tracker** — Track status (Want to Read, Reading, Finished, Abandoned) with inline quick-actions
- **Ratings & Reviews** — Rate books 1–5 stars and write reviews
- **Stats Dashboard** — Charts showing books read by genre, status breakdown, and reading activity
- **Dark Theme** — Sleek, modern UI with MudBlazor's dark mode

## Tech Stack

| Layer | Technology |
|-------|-----------|
| Frontend | Blazor Web App (InteractiveServer) + MudBlazor 9.x |
| Backend | ASP.NET Core Minimal API |
| Database | EF Core + SQLite |
| Orchestration | .NET Aspire |
| External API | [Open Library](https://openlibrary.org/) |

## Project Structure

```
src/
├── ShelfLife.AppHost/          # Aspire orchestrator (startup project)
├── ShelfLife.ServiceDefaults/  # Shared telemetry, health checks, resilience
├── ShelfLife.Api/              # Minimal API backend
├── ShelfLife.Web/              # Blazor frontend
└── ShelfLife.Shared/           # DTOs and models
```

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [.NET Aspire workload](https://learn.microsoft.com/dotnet/aspire/fundamentals/setup-tooling)

### Run with Aspire

```bash
cd src/ShelfLife.AppHost
dotnet run
```

The Aspire dashboard will open in your browser showing both the API and Web app. The database is auto-created and can be seeded from the home page.

### Run in Visual Studio

1. Open `ShelfLife.slnx`
2. Set **ShelfLife.AppHost** as the startup project
3. Press F5

## Seeding Data

Click the **"Seed Database"** button on the home page to load 15 sample books with real ISBNs and cover images.

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/books` | List all books (optional `?status=` filter) |
| GET | `/api/books/{id}` | Get book by ID |
| POST | `/api/books` | Add a book to your shelf |
| PUT | `/api/books/{id}` | Update book status/rating/review |
| DELETE | `/api/books/{id}` | Remove a book |
| GET | `/api/search?q={query}` | Search Open Library |
| GET | `/api/stats` | Reading statistics |
| POST | `/api/books/seed` | Seed sample data |

## License

MIT