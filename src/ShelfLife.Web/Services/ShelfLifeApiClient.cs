using System.Net.Http.Json;
using ShelfLife.Shared.DTOs;
using ShelfLife.Shared.Models;

namespace ShelfLife.Web.Services;

public class ShelfLifeApiClient(HttpClient http)
{
    public async Task<List<BookSearchResult>> SearchBooksAsync(string query)
    {
        var results = await http.GetFromJsonAsync<List<BookSearchResult>>(
            $"api/search?q={Uri.EscapeDataString(query)}");
        return results ?? [];
    }

    public async Task<List<BookDto>> GetBooksAsync(ReadingStatus? status = null)
    {
        var url = status.HasValue
            ? $"api/books?status={status.Value}"
            : "api/books";
        var results = await http.GetFromJsonAsync<List<BookDto>>(url);
        return results ?? [];
    }

    public async Task<BookDto?> GetBookAsync(int id)
    {
        return await http.GetFromJsonAsync<BookDto>($"api/books/{id}");
    }

    public async Task<BookDto?> AddBookAsync(AddBookRequest request)
    {
        var response = await http.PostAsJsonAsync("api/books", request);
        if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
            return null; // already on shelf
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<BookDto>();
    }

    public async Task<BookDto?> UpdateBookAsync(int id, UpdateBookRequest request)
    {
        var response = await http.PutAsJsonAsync($"api/books/{id}", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<BookDto>();
    }

    public async Task<bool> DeleteBookAsync(int id)
    {
        var response = await http.DeleteAsync($"api/books/{id}");
        return response.IsSuccessStatusCode;
    }

    public async Task<ReadingStatsDto?> GetStatsAsync()
    {
        return await http.GetFromJsonAsync<ReadingStatsDto>("api/stats");
    }

    public async Task<int> SeedBooksAsync()
    {
        var response = await http.PostAsync("api/books/seed", null);
        if (!response.IsSuccessStatusCode) return 0;
        return await response.Content.ReadFromJsonAsync<int>();
    }
}
