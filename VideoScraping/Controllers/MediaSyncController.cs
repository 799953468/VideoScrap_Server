using Microsoft.AspNetCore.Mvc;
using Serilog;
using SqlSugar;
using VideoScraping.DTO.WebApi;
using VideoScraping.Entity;
using VideoScraping.Helper;

namespace VideoScraping.Controllers;

[ApiController]
[Route("[controller]")]
public class MediaSyncController(ILogger<MediaSyncController> logger, ISqlSugarClient databaseClient) : ControllerBase
{
    [HttpGet(Name = "GetSyncList")]
    public async Task<SyncListResult> Get(int pageSize = 20, int page = 1)
    {
        RefAsync<int> total = 0;
        var syncList = await databaseClient.Queryable<SyncEntity>()
            .WithCache()
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
            ScrapDestination = createSyncDto.ScrapSource,
            ScrapSource = createSyncDto.ScrapDestination,
            TheMovieDbId = createSyncDto.TheMovieDbId,
            Season = createSyncDto.Season,
            GetEpisodeRegular = createSyncDto.GetEpisodeRegular,
            MinFileSize = createSyncDto.MinFileSize,
        };
        await databaseClient.Insertable(sync)
            .ExecuteReturnSnowflakeIdAsync();
        return Ok();
    }

    [HttpPut(Name = "UpdateSync")]
    public async Task<ActionResult> Update([FromBody] CreateSyncDto createSyncDto)
    {
        try
        {
            var sync = new SyncEntity()
            {
                Id = createSyncDto.Id,
                Name = createSyncDto.Name,
                MinFileSize = createSyncDto.MinFileSize,
                TheMovieDbId = createSyncDto.TheMovieDbId,
                Season = createSyncDto.Season,
                GetEpisodeRegular = createSyncDto.GetEpisodeRegular,
                ScrapSource = createSyncDto.ScrapSource,
                ScrapDestination = createSyncDto.ScrapDestination,
                IsEnable = createSyncDto.IsEnable,
                MoveMethod = createSyncDto.MoveMethod,
                Ver = createSyncDto.Ver
            };
            await databaseClient.Updateable(sync)
                .ExecuteCommandWithOptLockAsync(true);
            await ServiceLocator.SyncTaskService.UpdateWatcher(sync);
        }
        catch (Exception ex)
        {
            Log.Error(ex.Message);
            return BadRequest(ex.Message);
        }

        return Ok();
    }

    [HttpDelete(Name = "DeleteSync")]
    [Route("{id:long}")]
    public async Task<ActionResult> Delete(long id)
    {
        await databaseClient.Deleteable<SyncEntity>()
            .In(id)
            .ExecuteCommandAsync();
        return Ok();
    }
}