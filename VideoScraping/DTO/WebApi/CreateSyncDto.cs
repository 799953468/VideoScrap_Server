namespace VideoScraping.DTO.WebApi;

public class CreateSyncDto
{
    public long Id { get; set; }
    /// <summary>
    ///  名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 路径
    /// </summary>
    public string ScrapSource { get; set; }

    /// <summary>
    /// 目的地
    /// </summary>
    public string ScrapDestination { get; set; }

    /// <summary>
    /// 移动模式
    /// </summary>
    public MoveMethod MoveMethod { get; set; }

    /// <summary>
    /// ID
    /// </summary>
    public int TheMovieDbId { get; set; }

    /// <summary>
    /// 季
    /// </summary>
    public int Season { get; set; }
    
    /// <summary>
    /// 集偏移
    /// </summary>
    public int? EpisodeOffset { get; set; }
    
    /// <summary>
    /// 集数正则
    /// </summary>
    public string? GetEpisodeRegular { get; set; }

    /// <summary>
    /// 最小文件大小(MB)
    /// </summary>
    public int MinFileSize { get; set; } = 200;
    
    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsEnable { get; set; } = true;
    
    /// <summary>
    /// 版本
    /// </summary>
    public long Ver { get; set; }
}