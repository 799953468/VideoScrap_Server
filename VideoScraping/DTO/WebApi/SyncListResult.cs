using VideoScraping.Entity;

namespace VideoScraping.DTO.WebApi;

public class SyncListResult
{
    public List<SyncEntity> SyncList { get; set; }
    public int Total { get; set; }
}