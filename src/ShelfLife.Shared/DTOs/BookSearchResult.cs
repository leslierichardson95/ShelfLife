namespace ShelfLife.Shared.DTOs;

public class BookSearchResult
{
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string? Isbn { get; set; }
    public string? CoverUrl { get; set; }
    public int? PageCount { get; set; }
    public string? Genre { get; set; }
    public string? Publisher { get; set; }
    public int? PublishYear { get; set; }
    public string? Description { get; set; }
    public string? OpenLibraryKey { get; set; }
}
