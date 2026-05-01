namespace ShelfLife.Shared.Models;

public class Book
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Author { get; set; }
    public string? Isbn { get; set; }
    public string? CoverUrl { get; set; }
    public int? PageCount { get; set; }
    public string? Genre { get; set; }
    public string? Publisher { get; set; }
    public int? PublishYear { get; set; }
    public string? Description { get; set; }
    public ReadingStatus Status { get; set; } = ReadingStatus.WantToRead;
    public int? Rating { get; set; }
    public string? ReviewText { get; set; }
    public DateTime DateAdded { get; set; } = DateTime.UtcNow;
    public DateTime? DateStarted { get; set; }
    public DateTime? DateFinished { get; set; }
    public string? OpenLibraryKey { get; set; }
}
