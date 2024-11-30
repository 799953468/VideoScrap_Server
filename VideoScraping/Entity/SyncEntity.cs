using System.Text.Json.Serialization;
using SqlSugar;
using VideoScraping.DTO;
using VideoScraping.Helper;

namespace VideoScraping.Entity;

[SugarTable]
public class SyncEntity
{
    /// <summary>
    /// id
    /// </summary>
    [SugarColumn(IsPrimaryKey = true, IsIdentity = false)]
    [JsonConverter(typeof(LongToStringConverter))]
    public long Id { get; set; }

    /// <summary>
    /// 名称
    /// </summary>
    [SugarColumn(ColumnDescription = "名称")]
    public string Name { get; set; }

    /// <summary>
    /// 刮捎路径
    /// </summary>
    [SugarColumn(ColumnDescription = "刮捎路径")]
    public string ScrapSource { get; set; }

    /// <summary>
    /// 目的地
    /// </summary>
    [SugarColumn(ColumnDescription = "目的地")]
    public string ScrapDestination { get; set; }

    /// <summary>
    /// 移动方式
    /// </summary>
    [SugarColumn(ColumnDescription = "移动方式")]
    public MoveMethod MoveMethod { get; set; }

    /// <summary>
    /// The Movie DB id
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "The Movie DB id")]
    public int TheMovieDbId { get; set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    [SugarColumn(ColumnDescription = "是否启用")]
    public bool IsEnable { get; set; }

    /// <summary>
    /// 季
    /// </summary>
    [SugarColumn(ColumnDescription = "季")]
    public int Season { get; set; } = 1;
    
    /// <summary>
    /// 获取集数正则，为空直接读取集数
    /// </summary>
    [SugarColumn(ColumnDescription = "获取集数正则，为空直接读取集数", IsNullable = true)]
    public string? GetEpisodeRegular { get; set; }
    
    /// <summary>
    /// 集偏移
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "集偏移")]
    public int? EpisodeOffset { get; set; }

    /// <summary>
    /// 最小文件大小(MB)
    /// </summary>
    [SugarColumn(ColumnDescription = "最小文件大小(MB)")]
    public int MinFileSize { get; set; }
    
    [JsonConverter(typeof(LongToStringConverter))]
    [SugarColumn(IsEnableUpdateVersionValidation = true)]//标识版本字段
    public long Ver { get; set; } 

    /// <summary>
    /// 刮捎配置
    /// </summary>
    [Navigate(NavigateType.OneToOne, nameof(ScraperConfig.SyncId), nameof(Id))]
    public ScraperConfig ScraperConfig { get; set; }
}