using ShelfLife.Api.Services;

namespace ShelfLife.Api.Endpoints;

public static class SearchEndpoints
{
    public static RouteGroupBuilder MapSearchEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/search");

        group.MapGet("/", async (string? q, OpenLibraryService openLibrary) =>
        {
            if (string.IsNullOrWhiteSpace(q))
                return Results.BadRequest(new { error = "Query parameter 'q' is required." });

            var results = await openLibrary.SearchAsync(q);
            return Results.Ok(results);
        });

        return group;
    }
}
