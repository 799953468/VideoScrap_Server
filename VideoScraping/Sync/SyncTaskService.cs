using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using VideoScraping.Entity;
using VideoScraping.Helper;
using VideoScraping.Media;

namespace VideoScraping.Sync;

public class SyncTaskService(ILogger<SyncTaskService> logger) : IHostedService, IDisposable
{
    private readonly Dictionary<SyncEntity, FileSystemWatcher> _syncWatcher = new();
    private readonly BlockingCollection<(FileInfo, SyncEntity)> _syncQueue = new();
    private readonly ConcurrentDictionary<Scraper, SyncEntity> _scrapers = new();

    public async Task StartAsync(CancellationToken stoppingToken)
    {
        ServiceLocator.SyncTaskService = this;
        logger.LogInformation("[Watcher] 启动监控服务...");

        var syncConfigList = await DatabaseHelper.UserDatabase.Queryable<SyncEntity>()
            .Where(s => s.IsEnable)
            .ToListAsync(stoppingToken);
        foreach (var syncEntity in syncConfigList)
        {
            if (!Directory.Exists(syncEntity.ScrapSource))
            {
                logger.LogWarning("Source directory doesn't exist");
                continue;
            }

            if (!Directory.Exists(syncEntity.ScrapDestination))
            {
                Directory.CreateDirectory(syncEntity.ScrapDestination);
            }

            var watcher = new FileSystemWatcher(syncEntity.ScrapSource);
            watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName;
            watcher.Created += (_, e) => { WatcherOnCreated(syncEntity, e); };
            watcher.EnableRaisingEvents = true;
            watcher.IncludeSubdirectories = true;
            _syncWatcher.Add(syncEntity, watcher);
            logger.LogInformation("[Watcher] {Name} 开始监控目录...", syncEntity.Name);
        }

        for (var i = 0; i < 4; i++)
        {
            Task.Run(async () => { await SyncTask(stoppingToken); }, stoppingToken);
        }
    }

    /// <summary>
    /// 文件创建
    /// </summary>
    /// <param name="syncEntity"></param>
    /// <param name="e"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void WatcherOnCreated(SyncEntity syncEntity, FileSystemEventArgs e)
    {
        var fileInfo = new FileInfo(e.FullPath);
        if (!fileInfo.Exists)
        {
            return;
        }

        if (fileInfo.Length < syncEntity.MinFileSize * 1024 * 1024)
        {
            logger.LogInformation("[Watcher] File {path} 大小小于 {minSize} MB, 跳过...", e.FullPath, syncEntity.MinFileSize);
            return;
        }

        logger.LogInformation("[Watcher] File {path} 添加队列", e.FullPath);
        _syncQueue.Add((fileInfo, syncEntity));
    }

    public async Task SyncAll()
    {
        var syncConfigList = await DatabaseHelper.UserDatabase.Queryable<SyncEntity>()
            .Where(s => s.IsEnable)
            .ToListAsync();
        foreach (var syncEntity in syncConfigList)
        {
            var files = Directory.GetFiles(syncEntity.ScrapSource, "*.*", SearchOption.AllDirectories);
            var minSize = syncEntity.MinFileSize * 1024 * 1024;
            foreach (var filePath in files)
            {
                var fileInfo = new FileInfo(filePath);
                if (fileInfo.Length < minSize)
                {
                    logger.LogInformation("[Watcher] File {filePath} 大小小于 {minSize} MB, 跳过...", filePath, minSize);
                    return;
                }

                logger.LogInformation("[Watcher] File {path} 添加队列", filePath);
                _syncQueue.Add((fileInfo, syncEntity));
            }
        }
    }

    public async Task StopAsync(CancellationToken stoppingToken)
    {
        _syncWatcher.Clear();
        logger.LogInformation("[Watcher] 停止监控...");
    }

    public void UpdateWatcher(SyncEntity syncEntity)
    {
        foreach (var syncWatcher in _syncWatcher)
        {
            if (syncWatcher.Key.Id == syncEntity.Id)
            {
                syncWatcher.Value.Dispose();
                _syncWatcher.Remove(syncWatcher.Key);
                logger.LogInformation("[Watcher] 停止监控: {path}...", syncWatcher.Key.ScrapSource);
                break;
            }
        }

        var watcher = new FileSystemWatcher(syncEntity.ScrapSource);
        watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName;
        watcher.Created += (_, e) => { WatcherOnCreated(syncEntity, e); };
        watcher.EnableRaisingEvents = true;
        watcher.IncludeSubdirectories = true;
        _syncWatcher.Add(syncEntity, watcher);
        logger.LogInformation("[Watcher] {Name} 开始监控目录...", syncEntity.Name);
    }

    public List<(FileInfo, SyncEntity)> GetSyncQueue()
    {
        return _syncQueue.ToList();
    }

    public void Dispose()
    {
        _syncWatcher.Clear();
    }

    private async Task SyncTask(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (_syncQueue.IsAddingCompleted)
            {
                break;
            }

            var (fileInfo, syncEntity) = _syncQueue.Take(stoppingToken);
            await StartScrap(fileInfo.FullName, syncEntity);
        }
    }

    private async Task StartScrap(string path, SyncEntity syncEntity)
    {
        string episodeValue;
        if (string.IsNullOrEmpty(syncEntity.GetEpisodeRegular))
        {
            episodeValue = Path.GetFileNameWithoutExtension(path);
        }
        else
        {
            var regex = new Regex(syncEntity.GetEpisodeRegular);
            episodeValue = regex.Match(path).Value;
        }

        if (!int.TryParse(episodeValue, out var episodeNum))
        {
            logger.LogError("Cannot parse episode for {path}", path);
            return;
        }

        if (syncEntity.EpisodeOffset != null)
        {
            episodeNum += syncEntity.EpisodeOffset.Value;
        }

        syncEntity.ScraperConfig = new ScraperConfig();
        syncEntity.ScraperConfig.ScraperImageConfig = new ScraperImageConfig()
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
        syncEntity.ScraperConfig.ScraperInfoConfig = new ScraperInfoConfig()
        {
            Basic = true,
            Credits = true,
            CreditsChinese = true,
            EpisodeCredits = true,
            EpisodeBasic = true,
            SeasonBasic = true
        };
        var scraper = new Scraper(syncEntity, episodeNum);
        _scrapers.TryAdd(scraper, syncEntity);
        await scraper.FileScraper(path);
        _scrapers.TryRemove(scraper, out _);
    }
}