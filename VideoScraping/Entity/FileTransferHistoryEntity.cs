using SqlSugar;
using VideoScraping.DTO;

namespace VideoScraping.Entity;

[SugarTable]
[SugarIndex("index_transfer_history_source", nameof(Source), OrderByType.Asc)]
public class FileTransferHistoryEntity
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = false)]
    public long Id { get; set; }
    
    [SugarColumn(ColumnDescription = "标题")]
    public required string Title { get; set; }
    
    [SugarColumn(ColumnDescription = "The Movie DB id")]
    public required string TheMovieDbId { get; set; }
    
    [SugarColumn(ColumnDescription = "移动方式")]
    public MoveMethod MoveMethod { get; set; }
    
    [SugarColumn(ColumnDescription = "源")]
    public required string Source { get; set; }
    
    [SugarColumn(ColumnDescription = "目的地")]
    public required string Destination { get; set; }
    
    [SugarColumn(ColumnDescription = "时间")]
    public DateTime Date { get; set; }
}