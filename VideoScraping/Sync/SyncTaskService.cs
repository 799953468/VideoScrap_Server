using SqlSugar;
using VideoScraping.Entity;
using VideoScraping.Helper;

namespace VideoScraping.Sync;

public class SyncTaskService(ILogger<SyncTaskService> logger, IServiceScopeFactory factory) : IHostedService, IDisposable
{
    private readonly List<SyncWorker> _syncWorkers = [];

    public async Task StartAsync(CancellationToken stoppingToken)
    {
        ServiceLocator.SyncTaskService = this;
        logger.LogInformation("Sync task service started");
        var db = factory.CreateScope().ServiceProvider.GetRequiredService<ISqlSugarClient>();
        var syncConfigList = await db.Queryable<SyncEntity>()
            .Where(s => s.IsEnable)
            .ToListAsync(stoppingToken);
        foreach (var syncEntity in syncConfigList)
        {
            var syncWorker = new SyncWorker(syncEntity);
            _syncWorkers.Add(syncWorker);
        }
    }

    public void SyncAll()
    {
        Task.Run(async () =>
        {
            foreach (var syncWorker in _syncWorkers)
            {
                await syncWorker.SyncAll();
            }
        });
    }

    public async Task StopAsync(CancellationToken stoppingToken)
    {
        foreach (var syncWorker in _syncWorkers)
        {
            syncWorker.Stop();
        }

        _syncWorkers.Clear();
        logger.LogInformation("Sync task service stopped");
    }

    public void Dispose()
    {
    }
}