using ShelfLife.Shared.Models;

namespace ShelfLife.Shared.DTOs;

public class BookDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string? Isbn { get; set; }
    public string? CoverUrl { get; set; }
    public int? PageCount { get; set; }
    public string? Genre { get; set; }
    public string? Publisher { get; set; }
    public int? PublishYear { get; set; }
    public string? Description { get; set; }
    public ReadingStatus Status { get; set; }
    public int? Rating { get; set; }
    public string? ReviewText { get; set; }
    public DateTime DateAdded { get; set; }
    public DateTime? DateStarted { get; set; }
    public DateTime? DateFinished { get; set; }
    public string? OpenLibraryKey { get; set; }
}
