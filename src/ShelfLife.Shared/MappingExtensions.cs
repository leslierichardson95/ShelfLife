using ShelfLife.Shared.DTOs;
using ShelfLife.Shared.Models;

namespace ShelfLife.Shared;

public static class MappingExtensions
{
    public static BookDto ToDto(this Book book) => new()
    {
        Id = book.Id,
        Title = book.Title,
        Author = book.Author,
        Isbn = book.Isbn,
        CoverUrl = book.CoverUrl,
        PageCount = book.PageCount,
        Genre = book.Genre,
        Publisher = book.Publisher,
        PublishYear = book.PublishYear,
        Description = book.Description,
        Status = book.Status,
        Rating = book.Rating,
        ReviewText = book.ReviewText,
        DateAdded = book.DateAdded,
        DateStarted = book.DateStarted,
        DateFinished = book.DateFinished,
        OpenLibraryKey = book.OpenLibraryKey
    };

    public static Book ToEntity(this AddBookRequest request) => new()
    {
        Title = request.Title,
        Author = request.Author,
        Isbn = request.Isbn,
        CoverUrl = request.CoverUrl,
        PageCount = request.PageCount,
        Genre = request.Genre,
        Publisher = request.Publisher,
        PublishYear = request.PublishYear,
        Description = request.Description,
        OpenLibraryKey = request.OpenLibraryKey,
        DateAdded = DateTime.UtcNow
    };

    public static List<BookDto> ToDtoList(this IEnumerable<Book> books) =>
        books.Select(b => b.ToDto()).ToList();
}
