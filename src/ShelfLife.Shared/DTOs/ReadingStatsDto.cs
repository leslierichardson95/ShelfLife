namespace ShelfLife.Shared.DTOs;

public class ReadingStatsDto
{
    public int TotalBooks { get; set; }
    public int BooksFinished { get; set; }
    public int CurrentlyReading { get; set; }
    public int WantToRead { get; set; }
    public int Abandoned { get; set; }
    public double AverageRating { get; set; }
    public int TotalPagesRead { get; set; }
    public Dictionary<string, int> BooksByGenre { get; set; } = [];
    public Dictionary<string, int> BooksPerMonth { get; set; } = [];
    public BookDto? CurrentBook { get; set; }
}
