namespace VideoScraping.DTO;

public class AppSetting
{
     public TMDB TMDB { get; set; }
}

public class TMDB
{
    public string ApiKey { get; set; }
    public string ApiUrl { get; set; }
    public string ImageUrl { get; set; }
}