using ShelfLife.Api.Services;
using ShelfLife.Shared.DTOs;
using ShelfLife.Shared.Models;

namespace ShelfLife.Api.Endpoints;

public static class BookEndpoints
{
    public static RouteGroupBuilder MapBookEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/books");

        // TODO: Add pagination to book list endpoint
        group.MapGet("/", async (ReadingStatus? status, BookshelfService bookshelf) =>
        {
            var books = await bookshelf.GetAllAsync(status);
            return Results.Ok(books);
        });

        group.MapGet("/{id:int}", async (int id, BookshelfService bookshelf) =>
        {
            var book = await bookshelf.GetByIdAsync(id);
            return book is not null ? Results.Ok(book) : Results.NotFound();
        });

        group.MapPost("/", async (AddBookRequest request, BookshelfService bookshelf, OpenLibraryService openLibrary) =>
        {
            // Fetch description from Works API if not supplied and a key is available
            if (string.IsNullOrEmpty(request.Description) && request.OpenLibraryKey is not null)
                request.Description = await openLibrary.GetDescriptionAsync(request.OpenLibraryKey);

            var book = await bookshelf.AddAsync(request);
            if (book is null)
                return Results.Conflict("This book is already on your shelf.");
            return Results.Created($"/api/books/{book.Id}", book);
        });

        group.MapPut("/{id:int}", async (int id, UpdateBookRequest request, BookshelfService bookshelf) =>
        {
            var book = await bookshelf.UpdateAsync(id, request);
            return book is not null ? Results.Ok(book) : Results.NotFound();
        });

        group.MapDelete("/{id:int}", async (int id, BookshelfService bookshelf) =>
        {
            var deleted = await bookshelf.DeleteAsync(id);
            return deleted ? Results.NoContent() : Results.NotFound();
        });

        group.MapPost("/seed", async (BookshelfService bookshelf) =>
        {
            var count = await bookshelf.SeedAsync();
            return Results.Ok(count);
        });

        return group;
    }
}
