using ShelfLife.Shared.Models;
using System.Text.Json;
using System.IO;
using ShelfLife.Api.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using ShelfLife.Shared;
using ShelfLife.Shared.DTOs;
using System.Threading.Tasks;

namespace ShelfLife.Api.Services;

// DEMO: Badly formatted file for Code Cleanup / .editorconfig
public class BookshelfService(ShelfContext db)
{
    public async Task<List<BookDto>> GetAllAsync(ReadingStatus? statusFilter)
    {
        // TODO: Add pagination support for large book collections
        IQueryable<Book> query = db.Books;

        if (statusFilter.HasValue)
            query = query.Where(b => b.Status == statusFilter.Value);

        List<Book> books = await query.OrderBy(b => b.Title).ToListAsync();
        return books.ToDtoList();
    }

    public async Task<BookDto?> GetByIdAsync(int id)
    {
        Book? book = await db.Books.FindAsync(id);
        return book?.ToDto();
    }

    public async Task<BookDto?> AddAsync(AddBookRequest request)
    {
        // Prevent duplicates by ISBN or OpenLibrary key
        Boolean isDuplicate = await db.Books.AnyAsync(b =>
            (request.Isbn != null && b.Isbn == request.Isbn) ||
            (request.OpenLibraryKey != null && b.OpenLibraryKey == request.OpenLibraryKey));

        if (isDuplicate)
            return null;

        Book book = request.ToEntity();
        db.Books.Add(book);
        await db.SaveChangesAsync();
        return book.ToDto();
    }

    public async Task<BookDto?> UpdateAsync(int id, UpdateBookRequest request)
    {
        Book? book = await db.Books.FindAsync(id);
        if (book is null) return null;

        if (request.Status.HasValue)
        {
            book.Status = request.Status.Value;
        }

        if (request.Rating.HasValue)
            book.Rating = request.Rating.Value;

        if (request.ReviewText is not null)
        {
            book.ReviewText = request.ReviewText;
        }

        if (request.DateStarted.HasValue)
            book.DateStarted = request.DateStarted.Value;

        if (request.DateFinished.HasValue)
        {
            book.DateFinished = request.DateFinished.Value;
        }

        await db.SaveChangesAsync();
        return book.ToDto();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        Book? book = await db.Books.FindAsync(id);
        if (book is null)
        {
            return false;
        }

        db.Books.Remove(book);
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<int> SeedAsync()
    {
        if (await db.Books.AnyAsync())
        {
            return 0;
        }

        String seedPath = Path.Combine(AppContext.BaseDirectory, "Data", "seed-data.json");
        if (!File.Exists(seedPath))
            return 0;

        String json = await File.ReadAllTextAsync(seedPath);
        JsonSerializerOptions options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        List<SeedBook>? seedBooks = JsonSerializer.Deserialize<List<SeedBook>>(json, options);

        if (seedBooks is null || seedBooks.Count == 0)
        {
            return 0;
        }

        List<Book> books = seedBooks.Select(s => new Book
        {
            Title = s.Title,
            Author = s.Author,
            Isbn = s.Isbn,
            CoverUrl = s.Isbn is not null
                ? $"https://covers.openlibrary.org/b/isbn/{s.Isbn}-M.jpg"
                : null,
            PageCount = s.PageCount,
            Genre = s.Genre,
            Publisher = s.Publisher,
            PublishYear = s.PublishYear,
            Description = s.Description,
            Status = Enum.Parse<ReadingStatus>(s.Status),
            Rating = s.Rating,
            ReviewText = s.ReviewText,
            DateAdded = s.DateAdded,
            DateStarted = s.DateStarted,
            DateFinished = s.DateFinished,
            OpenLibraryKey = s.OpenLibraryKey
        }).ToList();

        db.Books.AddRange(books);
        await db.SaveChangesAsync();
        return books.Count;
    }

    private class SeedBook
    {
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string? Isbn { get; set; }
        public int? PageCount { get; set; }
        public string? Genre { get; set; }
        public string? Publisher { get; set; }
        public int? PublishYear { get; set; }
        public string? Description { get; set; }
        public string Status { get; set; } = "WantToRead";
        public int? Rating { get; set; }
        public string? ReviewText { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime? DateStarted { get; set; }
        public DateTime? DateFinished { get; set; }
        public string? OpenLibraryKey { get; set; }
    }
}
