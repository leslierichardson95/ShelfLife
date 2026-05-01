using Microsoft.EntityFrameworkCore;
using ShelfLife.Api.Data;
using ShelfLife.Api.Endpoints;
using ShelfLife.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container
builder.Services.AddOpenApi();

builder.Services.AddDbContext<ShelfContext>(options =>
    options.UseSqlite("Data Source=shelflife.db"));

builder.Services.AddHttpClient<OpenLibraryService>();
builder.Services.AddScoped<BookshelfService>();
builder.Services.AddScoped<StatsService>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// HACK: Using EnsureCreated instead of migrations for demo simplicity
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ShelfContext>();
    db.Database.EnsureDeleted();  // Drop & recreate so new columns (e.g. CoverUrl) are picked up
    db.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors();

app.MapBookEndpoints();
app.MapSearchEndpoints();
app.MapStatsEndpoints();

app.MapDefaultEndpoints();

app.Run();
