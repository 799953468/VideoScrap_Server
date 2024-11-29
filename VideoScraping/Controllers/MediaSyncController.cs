using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using VideoScraping.DTO.WebApi;
using VideoScraping.Entity;

namespace VideoScraping.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class MediaSyncController(ILogger<MediaSyncController> logger, ISqlSugarClient databaseClient) : ControllerBase
{
    [HttpGet(Name = "GetSyncList")]
    public async Task<SyncListResult> Get(int pageSize = 20, int page = 1)
    {
        RefAsync<int> total = 0;
        var syncList = await databaseClient.Queryable<SyncEntity>()
            .ToPageListAsync(page, pageSize, total);
        return new SyncListResult() { SyncList = syncList, Total = total };
    }

    [HttpPost(Name = "InsertSync")]
    public async Task<ActionResult> Create([FromBody] CreateSyncDto createSyncDto)
    {
        var sync = new SyncEntity
        {
            Name = createSyncDto.Name,
            MoveMethod = createSyncDto.MoveMethod,
            ScrapDestination = createSyncDto.Destination,
            ScrapSource = createSyncDto.Source,
            TheMovieDbId = createSyncDto.TheMovieDbId,
            Season = createSyncDto.Season,
            GetEpisodeRegular = createSyncDto.GetEpisodeRegular,
            MinFileSize = createSyncDto.MinFileSize
        };
        await databaseClient.Insertable(sync)
            .ExecuteReturnSnowflakeIdAsync();

        return Ok();
    }

    [HttpPost(Name = "UpdateState")]
    public async Task<ActionResult> UpdateState([FromQuery] long? id)
    {
        if (id == null)
        {
            return BadRequest("id cannot be null");
        }

        await databaseClient.Updateable<SyncEntity>()
            .SetColumns(s => s.IsEnable == !s.IsEnable)
            .Where(s => s.Id == id)
            .ExecuteCommandAsync();
        return Ok();
    }

    [HttpDelete(Name = "DeleteSync")]
    public async Task<ActionResult> Delete([FromQuery] long? id)
    {
        if (id == null)
        {
            return BadRequest("id cannot be null");
        }

        await databaseClient.Deleteable<SyncEntity>()
            .In(id)
            .ExecuteCommandAsync();
        return Ok();
    }
}