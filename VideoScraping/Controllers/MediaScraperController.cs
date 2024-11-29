using Microsoft.AspNetCore.Mvc;
using VideoScraping.Helper;
using VideoScraping.Sync;

namespace VideoScraping.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class MediaScraperController(IEnumerable<IHostedService> hostedServices) : ControllerBase
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
}