using System.Text.Json.Serialization;

namespace VideoScraping.DTO.TheMovieDb;

public class ExternalIds
{
    [JsonPropertyName("imdb_id")]
    public string ImdbId { get; set; }

    [JsonPropertyName("freebase_mid")]
    public string FreebaseMid { get; set; }

    [JsonPropertyName("freebase_id")]
    public string FreebaseId { get; set; }

    [JsonPropertyName("tvdb_id")]
    public int TvdbId { get; set; }

    [JsonPropertyName("tvrage_id")]
    public string TvrageId { get; set; }

    [JsonPropertyName("wikidata_id")]
    public string WikidataId { get; set; }

    [JsonPropertyName("facebook_id")]
    public string FacebookId { get; set; }

    [JsonPropertyName("instagram_id")]
    public string InstagramId { get; set; }

    [JsonPropertyName("twitter_id")]
    public string TwitterId { get; set; }
}