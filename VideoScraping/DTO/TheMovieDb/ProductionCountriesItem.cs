using System.Text.Json.Serialization;

namespace VideoScraping.DTO.TheMovieDb;

public class ProductionCountriesItem
{
    [JsonPropertyName("iso_3166_1")]
    public string Iso31661 { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; }
}