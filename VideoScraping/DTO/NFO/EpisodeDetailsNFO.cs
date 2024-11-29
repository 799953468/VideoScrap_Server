using System.Xml.Serialization;

namespace VideoScraping.DTO.NFO;

[XmlRoot(ElementName = "episodedetails")]
public class EpisodeDetailsNFO
{
    [XmlElement(ElementName = "dateadded")]
    public string DateAdded { get; set; }

    [XmlElement(ElementName = "uniqueid")]
    public Uniqueid Uniqueid { get; set; }

    [XmlElement(ElementName = "tmdbid")]
    public int Tmdbid { get; set; }

    [XmlElement(ElementName = "title")]
    public string Title { get; set; }

    [XmlElement(ElementName = "originaltitle")]
    public string OriginalTitle { get; set; }

    [XmlElement(ElementName = "plot")]
    public string Plot { get; set; }

    [XmlElement(ElementName = "outline")]
    public string Outline { get; set; }

    [XmlElement(ElementName = "aired")]
    public string Aired { get; set; }

    [XmlElement(ElementName = "premiered")]
    public string Premiered { get; set; }

    [XmlElement(ElementName = "year")]
    public string Year { get; set; }

    [XmlElement(ElementName = "season")]
    public int Season { get; set; }

    [XmlElement(ElementName = "episode")]
    public int Episode { get; set; }

    [XmlElement(ElementName = "rating")]
    public double Rating { get; set; }

    [XmlElement(ElementName = "director")]
    public List<Director> Director { get; set; } = new();

    [XmlElement(ElementName = "actor")]
    public List<Actor> Actor { get; set; } = new();
}