using VideoScraping.Sync;

namespace VideoScraping.Helper;

public static class ServiceLocator
{
    public static IServiceProvider Instance { get; set; }
    
    public static SyncTaskService SyncTaskService { get; set; }
}