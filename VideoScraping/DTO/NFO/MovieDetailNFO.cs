using System.Xml.Serialization;

namespace VideoScraping.DTO.NFO;

[XmlRoot(ElementName = "movie")]
public class MovieDetailNFO
{
    [XmlElement(ElementName = "dateadded")]
    public DateTime Dateadded { get; set; }

    [XmlElement(ElementName = "tmdbid")]
    public int Tmdbid { get; set; }

    [XmlElement(ElementName = "uniqueid")]
    public List<Uniqueid> Uniqueid { get; set; }

    [XmlElement(ElementName = "imdbid")]
    public string Imdbid { get; set; }

    [XmlElement(ElementName = "plot")]
    public string Plot { get; set; }

    [XmlElement(ElementName = "outline")]
    public string Outline { get; set; }

    [XmlElement(ElementName = "director")]
    public Director Director { get; set; }

    [XmlElement(ElementName = "actor")]
    public List<Actor> Actor { get; set; }

    [XmlElement(ElementName = "genre")]
    public List<string> Genre { get; set; }

    [XmlElement(ElementName = "rating")]
    public DateTime Rating { get; set; }

    [XmlElement(ElementName = "title")]
    public string Title { get; set; }

    [XmlElement(ElementName = "originaltitle")]
    public string Originaltitle { get; set; }

    [XmlElement(ElementName = "premiered")]
    public DateTime Premiered { get; set; }

    [XmlElement(ElementName = "year")]
    public int Year { get; set; }
}