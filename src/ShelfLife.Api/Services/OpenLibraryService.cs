using System.Net.Http.Json;
using System.Text.Json;
using ShelfLife.Shared.DTOs;

namespace ShelfLife.Api.Services;

public class OpenLibraryService(HttpClient httpClient)
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<List<BookSearchResult>> SearchAsync(string query)
    {
        // TODO: Add caching for repeated searches to reduce Open Library API calls
        try
        {
            var encodedQuery = Uri.EscapeDataString(query);
            var response = await httpClient.GetAsync(
                $"https://openlibrary.org/search.json?q={encodedQuery}&limit=20");

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadFromJsonAsync<OpenLibraryResponse>(JsonOptions);

            if (json?.Docs is null)
                return [];

            var results = json.Docs.Select(doc =>
            {
                var isbn = doc.Isbn?.FirstOrDefault();
                string? coverUrl = doc.CoverId.HasValue
                    ? $"https://covers.openlibrary.org/b/id/{doc.CoverId}-M.jpg"
                    : isbn is not null
                        ? $"https://covers.openlibrary.org/b/isbn/{isbn}-M.jpg"
                        : null;
                return new BookSearchResult
                {
                    Title = doc.Title ?? "Unknown Title",
                    Author = doc.AuthorName?.FirstOrDefault() ?? "Unknown Author",
                    Isbn = isbn,
                    CoverUrl = coverUrl,
                    PageCount = doc.NumberOfPagesMedian,
                    Genre = doc.Subject?.FirstOrDefault(),
                    Publisher = doc.Publisher?.FirstOrDefault(),
                    PublishYear = doc.FirstPublishYear,
                    Description = null,
                    OpenLibraryKey = doc.Key
                };
            }).ToList();

            return results;
        }
        catch
        {
            return [];
        }
    }

    public async Task<string?> GetDescriptionAsync(string workKey)
    {
        try
        {
            // workKey is like "/works/OL123W"
            var url = $"https://openlibrary.org{workKey}.json";
            var response = await httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadFromJsonAsync<OpenLibraryWork>(JsonOptions);
            return json?.Description switch
            {
                null => null,
                // Works API returns description as either a plain string or {"type":"...","value":"..."}
                System.Text.Json.JsonElement el when el.ValueKind == System.Text.Json.JsonValueKind.String
                    => el.GetString(),
                System.Text.Json.JsonElement el when el.ValueKind == System.Text.Json.JsonValueKind.Object
                    => el.TryGetProperty("value", out var val) ? val.GetString() : null,
                _ => null
            };
        }
        catch
        {
            return null;
        }
    }

    // Internal types for deserializing Open Library JSON response
    private class OpenLibraryWork
    {
        [System.Text.Json.Serialization.JsonPropertyName("description")]
        public System.Text.Json.JsonElement? Description { get; set; }
    }

    private class OpenLibraryResponse
    {
        public List<OpenLibraryDoc>? Docs { get; set; }
    }

    private class OpenLibraryDoc
    {
        public string? Title { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("author_name")]
        public string[]? AuthorName { get; set; }

        public string[]? Isbn { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("number_of_pages_median")]
        public int? NumberOfPagesMedian { get; set; }

        public string[]? Subject { get; set; }
        public string[]? Publisher { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("first_publish_year")]
        public int? FirstPublishYear { get; set; }

        public string? Key { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("cover_i")]
        public int? CoverId { get; set; }
    }
}
