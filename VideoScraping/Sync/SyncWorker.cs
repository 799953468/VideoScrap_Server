using System.Text.RegularExpressions;
using Serilog;
using VideoScraping.Entity;
using VideoScraping.Helper;
using VideoScraping.Media;

namespace VideoScraping.Sync;

public class SyncWorker
{
    private readonly FileSystemWatcher _watcher;

    public SyncWorker(SyncEntity scraperEntity)
    {
        SyncEntity = scraperEntity;
        minSize = SyncEntity.MinFileSize != null ? SyncEntity.MinFileSize.Value * 1024 * 1024 : 200 * 1024 * 1024;
        if (!Directory.Exists(scraperEntity.ScrapSource))
        {
            Log.Warning("Source directory doesn't exist");
        }

        if (!Directory.Exists(scraperEntity.ScrapDestination))
        {
            Directory.CreateDirectory(scraperEntity.ScrapDestination);
        }

        _watcher = new FileSystemWatcher(scraperEntity.ScrapSource);
        _watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName;
        _watcher.Created += WatcherOnCreated;
        _watcher.EnableRaisingEvents = true;
        _watcher.IncludeSubdirectories = true;
        Log.Information($"Watcher Name: {scraperEntity.Name} started");
    }

    public async Task SyncAll()
    {
        var files = Directory.GetFiles(SyncEntity.ScrapSource, "*.*", SearchOption.AllDirectories);
        foreach (var file in files)
        {
            await StartScrap(file);
        }
    }

    public void Stop()
    {
        _watcher.Dispose();
    }

    private SyncEntity SyncEntity { get; }
    private long minSize;

    /// <summary>
    /// 文件创建
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void WatcherOnCreated(object sender, FileSystemEventArgs e)
    {
        Task.Run(async () => { await StartScrap(e.FullPath); });
    }

    private async Task StartScrap(string path)
    {
        var fileInfo = new FileInfo(path);
        if (fileInfo.Length < minSize)
        {
            Log.Information($"File {path} is less than {minSize} bytes");
            return;
        }

        Log.Information($"File {path} added start sync");
        string episodeValue;
        if (string.IsNullOrEmpty(SyncEntity.GetEpisodeRegular))
        {
            episodeValue = Path.GetFileNameWithoutExtension(path);
        }
        else
        {
            var regex = new Regex(SyncEntity.GetEpisodeRegular);
            episodeValue = regex.Match(path).Value;
        }

        if (!int.TryParse(episodeValue, out var episodeNum))
        {
            Log.Error($"Cannot parse episode for {path}");
        }

        SyncEntity.ScraperConfig = new ScraperConfig();
        SyncEntity.ScraperConfig.ScraperImageConfig = new ScraperImageConfig()
        {
            Background = true,
            Banner = true,
            Backdrop = true,
            Disc = true,
            EpisodeThumb = true,
            Logo = true,
            Poster = true,
            SeasonBanner = true,
            SeasonPoster = true,
            SeasonThumb = true
        };
        SyncEntity.ScraperConfig.ScraperInfoConfig = new ScraperInfoConfig()
        {
            Basic = true,
            Credits = true,
            CreditsChinese = true,
            EpisodeCredits = true,
            EpisodeBasic = true,
            SeasonBasic = true
        };
        var scraper = new Scraper(SyncEntity, episodeNum);
        await scraper.FileScraper(path);
    }
}