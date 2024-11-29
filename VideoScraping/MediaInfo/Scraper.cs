using System.Xml.Serialization;
using Serilog;
using VideoScraping.DTO;
using VideoScraping.DTO.NFO;
using VideoScraping.DTO.TheMovieDb;
using VideoScraping.Entity;
using VideoScraping.Helper;
using VideoScraping.MediaInfo;

namespace VideoScraping.Media;

public class Scraper
{
    private readonly SyncEntity _syncEntity;
    private TheMovieDbServer TheMovieDbServer { get; set; }
    private readonly CacheManager _cacheManager;
    private readonly Fanart _fanart = new Fanart();
    private readonly int _episode;

    public Scraper(SyncEntity syncEntity, int episode)
    {
        _syncEntity = syncEntity;
        _episode = episode;
        TheMovieDbServer = ServiceLocator.Instance.GetRequiredService<TheMovieDbServer>();
        _cacheManager = ServiceLocator.Instance.GetRequiredService<CacheManager>();
    }

    public async Task FileScraper(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Log.Warning("文件不存在");
            return;
        }

        var path = _syncEntity.ScrapDestination;
        Log.Information($"[Scraper] 开始刮捎: {filePath}...");
        try
        {
            var tvShowInfo = _cacheManager.GetOrCreate($"tvshowInfo.{_syncEntity.TheMovieDbId.ToString()}", () => TheMovieDbServer.GetTvShowInfo(_syncEntity.TheMovieDbId).Result);
            if (tvShowInfo == null)
            {
                Log.Warning($"[Scraper] 未找到 {_syncEntity.TheMovieDbId} 的信息");
                return;
            }

            await GenTVInfoFile(tvShowInfo, _syncEntity.ScrapDestination);

            _fanart.GetFanartInfo(MediaType.TV, tvShowInfo.ExternalIds.TvdbId);
            // poster
            if (_syncEntity.ScraperConfig.ScraperImageConfig.Poster)
            {
                var posterImageUrl = TheMovieDbServer.GetPosterImageUrl(tvShowInfo.PosterPath, true);
                await SaveImage(posterImageUrl, path, "poster");
            }

            // backdrop
            if (_syncEntity.ScraperConfig.ScraperImageConfig.Backdrop)
            {
                var backdropImageUrl = TheMovieDbServer.GetBackdropImageUrl(tvShowInfo.BackdropPath, false, true);
                await SaveImage(backdropImageUrl, path, "fanart");
            }

            // background
            if (_syncEntity.ScraperConfig.ScraperImageConfig.Background)
            {
                var backgroundUrl = _fanart.GetBackgroundUrl();
                await SaveImage(backgroundUrl, path, "background");
            }

            // logo
            if (_syncEntity.ScraperConfig.ScraperImageConfig.Logo)
            {
                var logoUrl = _fanart.GetLogoUrl();
                await SaveImage(logoUrl, path, "logo");
            }

            // banner
            if (_syncEntity.ScraperConfig.ScraperImageConfig.Banner)
            {
                var bannerUrl = _fanart.GetBannerUrl();
                await SaveImage(bannerUrl, path, "banner");
            }

            // thumb
            if (_syncEntity.ScraperConfig.ScraperImageConfig.Thumb)
            {
                var thumbUrl = _fanart.GetThumbUrl();
                await SaveImage(thumbUrl, path, "thumb");
            }

            // season poster
            if (_syncEntity.ScraperConfig.ScraperImageConfig.SeasonPoster)
            {
                var seasonPosterUrl = _fanart.GetSeasonPosterUrl(_episode);
                await SaveImage(seasonPosterUrl, path, $"season{_syncEntity.Season:00}-poster");
            }

            // season banner
            if (_syncEntity.ScraperConfig.ScraperImageConfig.SeasonBanner)
            {
                var seasonBannerUrl = _fanart.GetSeasonBannerUrl(_episode);
                await SaveImage(seasonBannerUrl, path, $"season{_syncEntity.Season:00}-banner");
            }

            // season thumb
            if (_syncEntity.ScraperConfig.ScraperImageConfig.SeasonThumb)
            {
                var seasonThumbUrl = _fanart.GetSeasonThumbUrl(_episode);
                await SaveImage(seasonThumbUrl, path, $"season{_syncEntity.Season:00}-thumb");
            }

            path = Path.Combine(path, $"Season {_syncEntity.Season}");
            Directory.CreateDirectory(path);
            // season info
            if (_syncEntity.ScraperConfig.ScraperInfoConfig.SeasonBasic)
            {
                var sessionInfo = _cacheManager.GetOrCreate($"seasonInfo.{tvShowInfo.Id}", () => TheMovieDbServer.GetTvSessionDetail(tvShowInfo.Id, _syncEntity.Season).Result);
                if (sessionInfo != null)
                {
                    await GenTvSessionInfo(sessionInfo, path);
                }
            }

            // episode info
            if (_syncEntity.ScraperConfig.ScraperInfoConfig.EpisodeBasic || _syncEntity.ScraperConfig.ScraperInfoConfig.EpisodeCredits)
            {
                var episodeInfo = await TheMovieDbServer.GetEpisodeDetail(tvShowInfo.Id, _syncEntity.Season, _episode);
                if (episodeInfo != null)
                {
                    var fileName = $"{tvShowInfo.Name} - S{episodeInfo.SeasonNumber:00}E{episodeInfo.EpisodeNumber:00} - 第{episodeInfo.EpisodeNumber}集";
                    await GenTvEpisodeInfo(episodeInfo, path, fileName);
                    if (_syncEntity.ScraperConfig.ScraperImageConfig.EpisodeThumb)
                    {
                        var episodeThumbUrl = TheMovieDbServer.GetEpisodesImageUrl(episodeInfo.StillPath, true);
                        await SaveImage(episodeThumbUrl, path, fileName + "-thumb");
                    }

                    // sync
                    var newFilePath = Path.Combine(path, fileName + Path.GetExtension(filePath));
                    if (_syncEntity.MoveMethod == MoveMethod.Copy)
                    {
                        Log.Information($"[Scrapper] Copy {filePath} to {newFilePath}");
                        File.Copy(filePath, newFilePath, false);
                    }
                    else
                    {
                        Log.Information($"[Scrapper] Move {filePath} to {newFilePath}");
                        File.Move(filePath, newFilePath);
                    }
                }
            }

            Log.Information($"[Scraper] 刮捎完成");
        }
        catch (Exception e)
        {
            Log.Error(e.Message);
        }
    }

    /// <summary>
    /// 生成电视剧的NFO描述文件
    /// </summary>
    /// <param name="tvInfo">TMDB元数据</param>
    /// <param name="outPath">电视剧根目录</param>
    private async Task GenTVInfoFile(TVSeriesDetails tvInfo, string outPath)
    {
        if (File.Exists(Path.Combine(outPath, "tvshow.nfo")))
        {
            return;
        }

        Log.Information($"[Scraper] 正在生成电视剧NFO文件：{outPath}tvshow.nfo");
        var scraperInfo = new TvshowDetailNFO()
        {
            DateAdded = DateTime.Now.ToString("yyyy-M-d HH:mm:ss")
        };
        scraperInfo.Tmdbid = tvInfo.Id;
        scraperInfo.Uniqueid.Add(new Uniqueid()
        {
            Type = "tmdb",
            Default = true,
            Text = tvInfo.Id
        });
        scraperInfo.Plot = tvInfo.Overview ?? "";
        scraperInfo.Outline = tvInfo.Overview ?? "";

        // 标题
        scraperInfo.Title = tvInfo.Name ?? "";
        scraperInfo.OriginalTitle = tvInfo.OriginalName ?? "";

        // 发布日期
        scraperInfo.Premiered = tvInfo.FirstAirDate ?? "";
        scraperInfo.Season = -1;
        scraperInfo.Episode = -1;

        await using var fs = new FileStream(Path.Combine(outPath, "tvshow.nfo"), FileMode.Create);
        var xz = new XmlSerializer(scraperInfo.GetType());
        xz.Serialize(fs, scraperInfo);
    }

    /// <summary>
    /// 生成电视剧季的NFO描述文件
    /// </summary>
    /// <param name="seasonDetails">TMDB季媒体信息</param>
    /// <param name="outPath">电视剧季的目录</param>
    private async Task GenTvSessionInfo(SeasonDetails seasonDetails, string outPath)
    {
        if (File.Exists(Path.Combine(outPath, "tvshow.nfo")))
        {
            return;
        }

        Log.Information($"[Scraper] 正在生成季NFO文件: {outPath}season.nfo");
        var seasonNfo = new SeasonDetailNFO()
        {
            SeasonNumber = seasonDetails.SeasonNumber,
            Overview = seasonDetails.Overview,
            Outline = seasonDetails.Overview,
            Title = seasonDetails.Name,
            Premiered = seasonDetails.AirDate ?? "",
            DateAdded = DateTime.Now.ToString("yyyy-M-d HH:mm:ss"),
            Plot = seasonDetails.Overview,
            ReleaseDate = seasonDetails.AirDate ?? "",
            Year = string.IsNullOrEmpty(seasonDetails.AirDate) ? string.Empty : seasonDetails.AirDate[..4]
        };

        await using var fs = new FileStream(Path.Combine(outPath, "season.nfo"), FileMode.Create);
        var xz = new XmlSerializer(seasonNfo.GetType());
        xz.Serialize(fs, seasonNfo);
    }

    /// <summary>
    /// 生成电视剧集的NFO描述文件
    /// </summary>
    /// <param name="episodesDetails"></param>
    /// <param name="outPath"></param>
    /// <param name="tvShowName">电视剧名称</param>
    private async Task GenTvEpisodeInfo(EpisodesDetails episodesDetails, string outPath, string tvShowName)
    {
        var fileName = tvShowName + ".nfo";
        Log.Information($"[Scraper] 正在生成剧集NFO文件：{fileName}");
        var nfo = new EpisodeDetailsNFO();
        // 添加时间
        nfo.DateAdded = DateTime.Now.ToString("yyyy-M-d HH:mm:ss");
        // tmdb id
        nfo.Tmdbid = _syncEntity.TheMovieDbId;
        nfo.Uniqueid = new Uniqueid()
        {
            Type = "tmdb",
            Default = true,
            Text = nfo.Tmdbid
        };
        // 标题
        nfo.Title = episodesDetails.Name;
        // 简介
        nfo.Plot = episodesDetails.Overview;
        nfo.Outline = episodesDetails.Overview;
        // 发布日期
        nfo.Aired = episodesDetails.AirDate;
        // 年份
        nfo.Year = string.IsNullOrEmpty(episodesDetails.AirDate) ? string.Empty : episodesDetails.AirDate[..4];
        // 季
        nfo.Season = episodesDetails.SeasonNumber;
        // 集
        nfo.Episode = episodesDetails.EpisodeNumber;
        // 评分
        nfo.Rating = episodesDetails.VoteAverage;

        if (_syncEntity.ScraperConfig.ScraperInfoConfig.EpisodeCredits)
        {
            // 导演
            foreach (var crew in episodesDetails.Crew)
            {
                if (crew.KnownForDepartment == "Directing")
                {
                    nfo.Director.Add(new Director() { Tmdbid = crew.Id, Text = crew.Name });
                }
            }

            // 演员
            foreach (var guestStar in episodesDetails.GuestStars)
            {
                if (guestStar.KnownForDepartment == "Acting")
                {
                    nfo.Actor.Add(new Actor() { Name = guestStar.Name, Tmdbid = guestStar.Id, Type = "Actor" });
                }
            }
        }

        await using var fs = new FileStream(Path.Combine(outPath, fileName), FileMode.Create);
        var xz = new XmlSerializer(nfo.GetType());
        xz.Serialize(fs, nfo);
    }

    /// <summary>
    /// 保存图片
    /// </summary>
    /// <param name="url"></param>
    /// <param name="outPath"></param>
    /// <param name="type"></param>
    /// <param name="force"></param>
    private async Task SaveImage(string? url, string outPath, string type = "", bool force = false)
    {
        if (string.IsNullOrEmpty(url))
        {
            return;
        }

        string imagePath;
        if (!string.IsNullOrEmpty(type))
        {
            imagePath = Path.Combine(outPath, $"{type}.{url.Split(".").Last()}");
        }
        else
        {
            imagePath = outPath;
        }

        if (File.Exists(imagePath) && !force)
        {
            return;
        }

        Log.Information($"[Scraper] 正在下载{type}图片: {url}");
        var fileData = await DownloadHelper.DownloadFileAsBinaryAsync(new Uri(url));
        if (fileData == null)
        {
            Log.Error($"[Scraper] {type}图片下载失败，请检查网络连通性");
            return;
        }

        await using var fs = new FileStream(imagePath, FileMode.OpenOrCreate);
        await fs.WriteAsync(fileData);
        Log.Information($"[Scraper] {type}图片已保存：{imagePath}");
    }
}