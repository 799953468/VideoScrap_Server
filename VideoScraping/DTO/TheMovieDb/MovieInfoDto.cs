using System.Text.Json.Serialization;

namespace VideoScraping.DTO.TheMovieDb;

public class MovieInfoDto
{
    public bool Adult { get; set; }
    public string BackdropPath { get; set; }
    public GenresDto[] Genres { get; set; }
    public int Id { get; set; }
    public string ImdbId { get; set; }
    public string[] OriginCountry { get; set; }
    public string OriginalLanguage { get; set; }
    public string OriginalTitle { get; set; }
    public string Overview { get; set; }
    public float Popularity { get; set; }
    public string PosterPath { get; set; }
    public string ReleaseDate { get; set; }
    public int Revenue { get; set; }
    public int Runtime { get; set; }
    public string Status { get; set; }
    public string Tagline { get; set; }
    public string Title { get; set; }
    public bool Video { get; set; }
    public float VoteAverage { get; set; }
    public int VoteCount { get; set; }
}

public class AuthorInfoDto
{
    public int Id { get; set; }
    public string CreaditId { get; set; }
    public string Name { get; set; }
    public string OriginalName { get; set; }
    public int Gender { get; set; }
    public string ProfilePath { get; set; }
}

public class GenresDto
{
    public int Id { get; set; }
    public string Name { get; set; }
}