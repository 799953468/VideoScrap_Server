// using Serilog;
// using VideoScraping.DTO;
//
// namespace VideoScraping.MediaServer.Client;
//
// public class Jellyfin : IMediaClient
// {
//     private readonly HttpClient _httpClient = new HttpClient();
//
//     public Jellyfin()
//     {
//     }
//
//     /// <summary>
//     /// 媒体服务器Id
//     /// </summary>
//     public string ClientId => "Jellyfin";
//
//     /// <summary>
//     /// 媒体服务器类型
//     /// </summary>
//     public MediaClientType ClientType => MediaClientType.Jellyfin;
//
//     /// <summary>
//     /// 媒体服务器名称
//     /// </summary>
//     public string ClientName => "Jellyfin";
//
//     private long _serverId;
//     private string? _apiKey;
//     private string? _host;
//     private string _playHost;
//     private string _user;
//
//     /// <summary>
//     /// 获得电影、电视剧、动漫媒体数量
//     /// </summary>
//     /// <exception cref="NotImplementedException"></exception>
//     public async Task GetMediasCount()
//     {
//         if (string.IsNullOrEmpty(_host) || string.IsNullOrEmpty(_apiKey))
//         {
//             return;
//         }
//
//         var reqUrl = $"{_host}Items/Counts?api_key={_apiKey}";
//         try
//         {
//             var res = await _httpClient.GetAsync(reqUrl);
//             res.EnsureSuccessStatusCode();
//             var content = await res.Content.ReadAsStringAsync();
//         }
//         catch (Exception ex)
//         {
//             Log.Error($"[{ClientName}] 连接Items/Counts出错: {ex}");
//         }
//
//         throw new NotImplementedException();
//     }
// }