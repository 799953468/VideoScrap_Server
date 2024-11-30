using System.Text.Json.Serialization;
using VideoScraping.Helper;

namespace VideoScraping.DTO.WebApi;

public class QueueInfo
{
    /// <summary>
    /// 文件路径
    /// </summary>
    public string Path { get; set; }

    /// <summary>
    /// 文件大小
    /// </summary>
    [JsonConverter(typeof(LongToStringConverter))]
    public long Size { get; set; }

    /// <summary>
    /// 同步名称
    /// </summary>
    public string SyncName { get; set; }
    
    /// <summary>
    /// TMDB id
    /// </summary>
    public int TmdbId { get; set; }
}