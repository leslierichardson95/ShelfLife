using ShelfLife.Shared.Models;

namespace ShelfLife.Shared.DTOs;

public class UpdateBookRequest
{
    public ReadingStatus? Status { get; set; }
    public int? Rating { get; set; }
    public string? ReviewText { get; set; }
    public DateTime? DateStarted { get; set; }
    public DateTime? DateFinished { get; set; }
}
