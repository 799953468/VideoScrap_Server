using VideoScraping.DTO;

namespace VideoScraping.MediaServer.Client;

public interface IMediaClient
{
    string ClientId { get; }
    MediaClientType ClientType { get; }
    string ClientName { get; }

    /// <summary>
    /// 匹配实例
    /// </summary>
    void Match();
    
    /// <summary>
    /// 获取媒体服务器类型
    /// </summary>
    void GetType();
    
    /// <summary>
    /// 检查连通性
    /// </summary>
    void GetStatus();

    /// <summary>
    /// 获得用户数量
    /// </summary>
    void GetUserCount();

    /// <summary>
    /// 获取活动记录
    /// </summary>
    void GetActivityLog();

    /// <summary>
    /// 获得电影、电视剧、动漫媒体数量
    /// </summary>
    Task GetMediasCount();

    /// <summary>
    /// 根据标题和年份，检查电影是否在存在，存在则返回列表
    /// </summary>
    /// <param name="title">标题</param>
    /// <param name="year">年份，可以为空，为空时不按年份过滤</param>
    void GetMovies(string title, string? year);
    
    /// <summary>
    /// 根据标题、年份、季查询电视剧所有集信息
    /// </summary>
    /// <param name="title">标题</param>
    /// <param name="year">年份，可以为空，为空时不按年份过滤</param>
    /// <param name="season">季号</param>
    void GetDramasEpisodes(string title, string? year, int? season);
}