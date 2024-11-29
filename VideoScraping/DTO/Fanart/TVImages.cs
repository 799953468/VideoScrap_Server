using System.Text.Json.Serialization;

namespace VideoScraping.DTO.Fanart;

public class TVImages
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("thetvdb_id")]
    public string TheTvdbId { get; set; }

    [JsonPropertyName("tvposter")]
    public List<TvPosterItem> TvPoster { get; set; } = new();

    [JsonPropertyName("hdclearart")]
    public List<HdClearArtItem> Hdclearart { get; set; } = new();

    [JsonPropertyName("showbackground")]
    public List<ShowBackgroundItem> ShowBackground { get; set; } = new();

    [JsonPropertyName("seasonposter")]
    public List<SeasonPosterItem> SeasonPoster { get; set; } = new();

    [JsonPropertyName("seasonthumb")]
    public List<SeasonThumbItem> SeasonThumb { get; set; } = new();

    [JsonPropertyName("hdtvlogo")]
    public List<HdtvLogoItem> HdtvLogo { get; set; } = new();

    [JsonPropertyName("seasonbanner")]
    public List<SeasonBannerItem> SeasonBanner { get; set; } = new();

    [JsonPropertyName("tvbanner")]
    public List<TvBannerItem> TvBanner { get; set; } = new();

    [JsonPropertyName("tvthumb")]
    public List<TvThumbItem> TvThumb { get; set; } = new();
}