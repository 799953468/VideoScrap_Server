using System.Xml.Serialization;

namespace VideoScraping.DTO.NFO;

[XmlRoot(ElementName = "tvshow")]
public class TvshowDetailNFO
{
    [XmlElement(ElementName = "dateadded")]
    public string DateAdded { get; set; }

    [XmlElement(ElementName = "tmdbid")]
    public int Tmdbid { get; set; }

    [XmlElement(ElementName = "uniqueid")]
    public List<Uniqueid> Uniqueid { get; set; } = new();

    [XmlElement(ElementName = "tvdbid")]
    public int Tvdbid { get; set; }

    [XmlElement(ElementName = "imdbid")]
    public string Imdbid { get; set; }

    [XmlElement(ElementName = "plot")]
    public string Plot { get; set; }

    [XmlElement(ElementName = "outline")]
    public string Outline { get; set; }

    [XmlElement(ElementName = "actor")]
    public List<Actor> Actor { get; set; }

    [XmlElement(ElementName = "genre")]
    public List<string> Genre { get; set; }

    [XmlElement(ElementName = "rating")]
    public DateTime Rating { get; set; }

    [XmlElement(ElementName = "title")]
    public string Title { get; set; }

    [XmlElement(ElementName = "originaltitle")]
    public string OriginalTitle { get; set; }

    [XmlElement(ElementName = "premiered")]
    public string Premiered { get; set; }

    [XmlElement(ElementName = "year")]
    public int Year { get; set; }

    [XmlElement(ElementName = "season")]
    public int Season { get; set; }

    [XmlElement(ElementName = "episode")]
    public int Episode { get; set; }
}