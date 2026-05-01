using ShelfLife.Api.Services;

namespace ShelfLife.Api.Endpoints;

public static class StatsEndpoints
{
    public static RouteGroupBuilder MapStatsEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/stats");

        group.MapGet("/", async (StatsService stats) =>
        {
            var result = await stats.GetStatsAsync();
            return Results.Ok(result);
        });

        return group;
    }
}
