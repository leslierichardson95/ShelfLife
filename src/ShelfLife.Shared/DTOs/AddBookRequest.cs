namespace ShelfLife.Shared.DTOs;

public class AddBookRequest
{
    public required string Title { get; set; }
    public required string Author { get; set; }
    public string? Isbn { get; set; }
    public string? CoverUrl { get; set; }
    public int? PageCount { get; set; }
    public string? Genre { get; set; }
    public string? Publisher { get; set; }
    public int? PublishYear { get; set; }
    public string? Description { get; set; }
    public string? OpenLibraryKey { get; set; }
}
