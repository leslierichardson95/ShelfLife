using Microsoft.EntityFrameworkCore;
using ShelfLife.Api.Data;
using ShelfLife.Shared;
using ShelfLife.Shared.DTOs;
using ShelfLife.Shared.Models;

namespace ShelfLife.Api.Services;

public class StatsService(ShelfContext db)
{
    public async Task<ReadingStatsDto> GetStatsAsync()
    {
        // HACK: Loading all books into memory; fine for demo, use projections in production
        var allBooks = await db.Books.ToListAsync();

        // Count books by status
        var totalBooks = allBooks.Count;
        var booksFinished = allBooks.Count(b => b.Status == ReadingStatus.Finished);
        var currentlyReading = allBooks.Count(b => b.Status == ReadingStatus.Reading);
        var wantToRead = allBooks.Count(b => b.Status == ReadingStatus.WantToRead);
        var abandoned = allBooks.Count(b => b.Status == ReadingStatus.Abandoned);

        // Calculate average rating from books that have been rated
        var ratedBooks = allBooks.Where(b => b.Rating.HasValue).ToList();
        var ratings = ratedBooks.Select(b => b.Rating!.Value).ToList();
        var averageRating = ratings.Count > 0
            ? Math.Round(ratings.Average(), 1)
            : 0.0;

        // Total pages of finished books
        var finishedBooks = allBooks.Where(b => b.Status == ReadingStatus.Finished).ToList();
        var finishedWithPages = finishedBooks.Where(b => b.PageCount.HasValue).ToList();
        var totalPagesRead = finishedWithPages.Sum(b => b.PageCount!.Value);

        // Books grouped by genre
        var booksWithGenre = allBooks.Where(b => !string.IsNullOrEmpty(b.Genre)).ToList();
        var genreGroups = booksWithGenre.GroupBy(b => b.Genre!).ToList();
        var booksByGenre = genreGroups
            .OrderByDescending(g => g.Count())
            .ToDictionary(g => g.Key, g => g.Count());

        // Books finished per month (group by DateFinished year-month)
        var finishedWithDates = finishedBooks.Where(b => b.DateFinished.HasValue).ToList();
        var monthGroups = finishedWithDates
            .GroupBy(b => b.DateFinished!.Value.ToString("yyyy-MM"))
            .ToList();
        var booksPerMonth = monthGroups
            .OrderBy(g => g.Key)
            .ToDictionary(g => g.Key, g => g.Count());

        // Currently reading book (pick the first one)
        var currentBook = allBooks
            .Where(b => b.Status == ReadingStatus.Reading)
            .OrderByDescending(b => b.DateStarted)
            .FirstOrDefault();

        return new ReadingStatsDto
        {
            TotalBooks = totalBooks,
            BooksFinished = booksFinished,
            CurrentlyReading = currentlyReading,
            WantToRead = wantToRead,
            Abandoned = abandoned,
            AverageRating = averageRating,
            TotalPagesRead = totalPagesRead,
            BooksByGenre = booksByGenre,
            BooksPerMonth = booksPerMonth,
            CurrentBook = currentBook?.ToDto()
        };
    }
}
