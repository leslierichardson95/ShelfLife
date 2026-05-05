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
            CurrentBook = currentBook?.ToDto(),
            Summary = BuildSummary(FormatProgress(booksFinished, totalBooks), FormatTopGenre(booksByGenre), FormatPace(booksPerMonth), FormatCurrentRead(currentBook))
        };
    }

    private static string BuildSummary(string progress, string topGenre, string pace, string currentRead)
    {
        return $"{progress} | {topGenre} | {pace} | {currentRead}";
    }

    private static string FormatProgress(int finished, int total)
    {
        var pct = total > 0 ? (int)Math.Round(100.0 * finished / total) : 0;
        return $"{finished} of {total} books finished ({pct}%)";
    }

    private static string FormatTopGenre(Dictionary<string, int> booksByGenre)
    {
        var top = booksByGenre.OrderByDescending(g => g.Value).FirstOrDefault();
        return top.Key is not null ? $"Top genre: {top.Key}" : "No genres yet";
    }

    private static string FormatPace(Dictionary<string, int> booksPerMonth)
    {
        var best = booksPerMonth.OrderByDescending(m => m.Value).FirstOrDefault();
        return best.Key is not null ? $"Best month: {best.Key} ({best.Value} books)" : "No reading history";
    }

    private static string FormatCurrentRead(Book? currentBook)
    {
        return currentBook is not null ? $"Now reading: {currentBook.Title}" : "Nothing in progress";
    }
}
