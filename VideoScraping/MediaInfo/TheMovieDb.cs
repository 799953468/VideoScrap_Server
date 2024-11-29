using Microsoft.Extensions.Options;
using Serilog;
using VideoScraping.DTO;
using VideoScraping.DTO.TheMovieDb;

namespace VideoScraping.MediaInfo;

public class TheMovieDbServer
{
    private readonly HttpClient _httpClient = new HttpClient();
    private readonly AppSetting _appSetting;

    public TheMovieDbServer(IOptions<AppSetting> options)
    {
        _appSetting = options.Value;
    }

    public string ApiKey => _appSetting.TMDB.ApiKey;
    public string Language { get; set; } = "zh-CN";
    public bool DebugEnabled { get; set; }
    public string Domain => $"https://{_appSetting.TMDB.ApiUrl}/3";
    public string ImageUrl => $"https://{_appSetting.TMDB.ImageUrl}";
    public bool IncludeAdult { get; set; }
    public int Remaining { get; private set; }
    public int Reset { get; private set; }

    public async Task GetMovieInfo(int id)
    {
        if (string.IsNullOrEmpty(ApiKey))
        {
            throw new ArgumentNullException($"No API key found.");
        }

        var requestUrl = $"{Domain}/movie/{id}?api_key={ApiKey}&include_adult={IncludeAdult}&language={Language}";
        var response = await _httpClient.GetAsync(requestUrl);
        if (response.Headers.TryGetValues("X-RateLimit-Remaining", out var remainingValues))
        {
            var remainingStr = string.Join(", ", remainingValues);
            // 尝试将字符串转换为整数
            if (int.TryParse(remainingStr, out var remaining))
            {
                Remaining = remaining;
                Log.Debug($"剩余请求次数（整数）：{remaining}");
            }
        }

        if (response.Headers.TryGetValues("X-RateLimit-Reset", out var rateLimitResetValues))
        {
            var rateLimitResetStr = string.Join(", ", rateLimitResetValues);
            // 尝试将字符串转换为整数
            if (int.TryParse(rateLimitResetStr, out var rateLimitReset))
            {
                Reset = rateLimitReset;
            }
        }

        if (Remaining < 1)
        {
        }
    }

    /// <summary>
    /// 获取电视剧详情
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public async Task<TVSeriesDetails?> GetTvShowInfo(int id)
    {
        if (string.IsNullOrEmpty(ApiKey))
        {
            throw new ArgumentNullException($"No API key found.");
        }

        var requestUrl = $"{Domain}/tv/{id}?api_key={ApiKey}&include_adult={IncludeAdult}&language={Language}&append_to_response=external_ids";
        var response = await _httpClient.GetAsync(requestUrl);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<TVSeriesDetails>();
        return result;
    }

    /// <summary>
    /// 获取电视剧季的详情
    /// </summary>
    /// <param name="id">tmdb id</param>
    /// <param name="season">季</param>
    /// <returns></returns>
    public async Task<SeasonDetails?> GetTvSessionDetail(int id, int season)
    {
        if (string.IsNullOrEmpty(ApiKey))
        {
            throw new ArgumentNullException($"No API key found.");
        }

        Log.Information($"[TheMovieDb] 正在查询TMDB电视剧：{id}，季：{season} ...");
        var requestUrl = $"{Domain}/tv/{id}/season/{season}?api_key={ApiKey}&language={Language}";
        var response = await _httpClient.GetAsync(requestUrl);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<SeasonDetails>();
        if (result != null)
        {
            Log.Information($"[TheMovieDb] {id} 查询结果：{result.Name}");
        }

        return result;
    }

    /// <summary>
    /// 获取剧集信息
    /// </summary>
    /// <param name="seriesId"></param>
    /// <param name="season"></param>
    /// <param name="episode"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public async Task<EpisodesDetails?> GetEpisodeDetail(int seriesId, int season, int episode)
    {
        if (string.IsNullOrEmpty(ApiKey))
        {
            throw new ArgumentNullException($"No API key found.");
        }

        Log.Information($"[TheMovieDb] 正在查询TMDB电视剧：{seriesId}，季：{season}, 集: {episode} ...");
        var requestUrl = $"{Domain}/tv/{seriesId}/season/{season}/episode/{episode}?api_key={ApiKey}&language={Language}";
        var response = await _httpClient.GetAsync(requestUrl);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<EpisodesDetails>();
        if (result != null)
        {
            Log.Information($"[TheMovieDb] {seriesId} 查询结果：{result.Name}");
        }

        return result;
    }

    /// <summary>
    /// 获取图片地址
    /// </summary>
    /// <param name="path"></param>
    /// <param name="original"></param>
    /// <returns></returns>
    public string GetPosterImageUrl(string? path, bool original = false)
    {
        if (string.IsNullOrEmpty(path))
        {
            return string.Empty;
        }

        var tmdbUrl = ImageUrl + "/t/p/w500" + path;
        if (original)
        {
            return tmdbUrl.Replace("/w500", "/original");
        }
        else
        {
            return tmdbUrl;
        }
    }

    public string GetBackdropImageUrl(string? path, bool @default = true, bool original = false)
    {
        if (string.IsNullOrEmpty(path))
        {
            return string.Empty;
        }

        var tmdbUrl = ImageUrl + "/t/p/w500" + path;
        if (original)
        {
            return tmdbUrl.Replace("/w500", "/original");
        }
        else
        {
            return tmdbUrl;
        }
    }

    /// <summary>
    /// 获取剧集中某一集封面
    /// </summary>
    /// <param name="path"></param>
    /// <param name="original"></param>
    /// <returns></returns>
    public string GetEpisodesImageUrl(string? path, bool original = false)
    {
        if (string.IsNullOrEmpty(path))
        {
            return string.Empty;
        }

        var tmdbUrl = ImageUrl + "/t/p/w500" + path;
        if (original)
        {
            return tmdbUrl.Replace("/w500", "/original");
        }
        else
        {
            return tmdbUrl;
        }
    }
}