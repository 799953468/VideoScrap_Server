using Microsoft.AspNetCore.Mvc;
using VideoScraping.DTO.WebApi;
using VideoScraping.Helper;

namespace VideoScraping.Controllers;

[ApiController]
[Route("[controller]")]
public class MediaScraperController : ControllerBase
{
    /// <summary>
    /// 扫描所有配置的目录
    /// </summary>
    /// <returns></returns>
    [HttpPost(Name = "ScrapeAll")]
    public ActionResult ScrapeAll()
    {
        ServiceLocator.SyncTaskService.SyncAll();

        return Ok();
    }

    /// <summary>
    /// 获取同步队列信息
    /// </summary>
    /// <returns></returns>
    [HttpGet(Name = "GetSyncQueue")]
    [Route("Queue")]
    public ActionResult<List<QueueInfo>> GetSyncQueue()
    {
        var queue = ServiceLocator.SyncTaskService.GetSyncQueue();
        var result = new List<QueueInfo>();
        foreach (var (fileInfo, syncEntity) in queue)
        {
            result.Add(new QueueInfo()
            {
                Path = fileInfo.FullName,
                Size = fileInfo.Length,
                SyncName = syncEntity.Name,
                TmdbId = syncEntity.TheMovieDbId,
                
            });
        }

        return Ok(result);
    }
}