using System.Text.Json.Serialization;

namespace VideoScraping.DTO.Fanart;

public class TvBannerItem
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("url")]
    public string Url { get; set; }

    [JsonPropertyName("lang")]
    public string Lang { get; set; }

    [JsonPropertyName("likes")]
    public string Likes { get; set; }
}