using System.Text.Json.Serialization;

namespace VideoScraping.DTO.TheMovieDb;

public class GenresItem
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }
}