using Serilog;
using VideoScraping.DTO;
using VideoScraping.DTO.Fanart;
using VideoScraping.Helper;

namespace VideoScraping.MediaInfo;

public class Fanart
{
    private readonly HttpClient _httpClient = new HttpClient();
    private readonly CacheManager _cacheManager;
    private const string ApiKey = "d2d31f9ecabea050fc7d68aa3146015f";
    private const string FanartMovieApiUrl = "https://webservice.fanart.tv/v3/movies/";
    private const string FanartTvApiUrl = "https://webservice.fanart.tv/v3/tv/";
    private TVImages _tvImages = new();

    public Fanart()
    {
        _cacheManager = ServiceLocator.Instance.GetRequiredService<CacheManager>();
    }

    public void GetFanartInfo(MediaType mediaType, int id)
    {
        string requestUrl;
        if (mediaType == MediaType.Movie)
        {
            requestUrl = $"{FanartMovieApiUrl}{id}?api_key={ApiKey}";
        }
        else
        {
            requestUrl = $"{FanartTvApiUrl}{id}?api_key={ApiKey}";
        }

        Log.Information(requestUrl);

        _tvImages = _cacheManager.GetOrCreate($"{mediaType.ToString()}.{id}", () =>
        {
            return Task.Run(async () =>
            {
                var response = await _httpClient.GetAsync(requestUrl);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<TVImages>() ?? new TVImages();
            }).Result;
        });
    }

    public string GetBackgroundUrl()
    {
        foreach (var backgroundItem in _tvImages.ShowBackground)
        {
            return backgroundItem.Url;
        }

        return string.Empty;
    }

    public string GetLogoUrl()
    {
        foreach (var hdtvLogoItem in _tvImages.HdtvLogo)
        {
            return hdtvLogoItem.Url;
        }

        return string.Empty;
    }

    public string GetBannerUrl()
    {
        foreach (var tvBannerItem in _tvImages.TvBanner)
        {
            return tvBannerItem.Url;
        }

        return string.Empty;
    }

    public string GetSeasonBannerUrl(int season)
    {
        foreach (var seasonBannerItem in _tvImages.SeasonBanner)
        {
            if (seasonBannerItem.Season == season.ToString())
            {
                return seasonBannerItem.Url;
            }
        }

        var seasonBanner = _tvImages.SeasonBanner.FirstOrDefault(s => s.Season == "all");
        if (seasonBanner != null)
        {
            return seasonBanner.Url;
        }

        return string.Empty;
    }

    public string GetThumbUrl()
    {
        foreach (var tvThumbItem in _tvImages.TvThumb)
        {
            return tvThumbItem.Url;
        }

        return string.Empty;
    }

    public string GetSeasonThumbUrl(int season)
    {
        foreach (var seasonThumbItem in _tvImages.SeasonThumb)
        {
            if (seasonThumbItem.Season == season.ToString())
            {
                return seasonThumbItem.Url;
            }
        }

        var seasonThumb = _tvImages.SeasonThumb.FirstOrDefault(s => s.Season == "all");
        if (seasonThumb != null)
        {
            return seasonThumb.Url;
        }

        return string.Empty;
    }

    public string GetSeasonPosterUrl(int season)
    {
        foreach (var seasonPosterItem in _tvImages.SeasonPoster)
        {
            if (seasonPosterItem.Season == season.ToString())
            {
                return seasonPosterItem.Url;
            }
        }

        var seasonPoster = _tvImages.SeasonPoster.FirstOrDefault(s => s.Season == "all");
        if (seasonPoster != null)
        {
            return seasonPoster.Url;
        }

        return string.Empty;
    }
}