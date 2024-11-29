using SqlSugar;

namespace VideoScraping.Entity;

[SugarTable]
public class ScraperConfig
{
    /// <summary>
    /// id
    /// </summary>
    [SugarColumn(IsPrimaryKey = true, IsIdentity = false)]
    public long Id { get; set; }
    /// <summary>
    /// 刮捎信息配置
    /// </summary>
    [SugarColumn(IsJson = true)]
    public ScraperInfoConfig ScraperInfoConfig { get; set; }
    
    /// <summary>
    /// 刮捎图片配置
    /// </summary>
    [SugarColumn(IsJson = true)]
    public ScraperImageConfig ScraperImageConfig { get; set; }
    
    /// <summary>
    /// 关联id
    /// </summary>
    [SugarColumn(ColumnDescription = "关联id")]
    public long SyncId { get; set; }
}

public class ScraperInfoConfig
{
    public bool Basic { get; set; }
    public bool Credits { get; set; }
    public bool CreditsChinese { get; set; }
    public bool SeasonBasic { get; set; }
    public bool EpisodeBasic { get; set; }
    public bool EpisodeCredits { get; set; }
}

public class ScraperImageConfig
{
    public bool Poster { get; set; }
    public bool Backdrop { get; set; }
    public bool Background { get; set; }
    public bool Logo { get; set; }
    public bool Disc { get; set; }
    public bool Banner { get; set; }
    public bool Thumb { get; set; }
    public bool SeasonPoster { get; set; }
    public bool SeasonBanner { get; set; }
    public bool SeasonThumb { get; set; }
    public bool EpisodeThumb { get; set; }
}