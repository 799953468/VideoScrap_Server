namespace VideoScraping.Helper;

public class DownloadHelper
{
    public static async Task<byte[]?> DownloadFileAsBinaryAsync(Uri url)
    {
        try
        {
            using var client = new HttpClient();
            // 发送 GET 请求并获取文件内容
            var data = await client.GetByteArrayAsync(url);
            return data;
        }
        catch
        {
            return null;
        }
    }
}