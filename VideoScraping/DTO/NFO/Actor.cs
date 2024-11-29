using System.Xml.Serialization;

namespace VideoScraping.DTO.NFO;

[XmlRoot("actor")]
public class Actor
{
    /// <summary>
    /// name
    /// </summary>
    [XmlElement(ElementName = "name")]
    public string Name { get; set; }

    [XmlElement(ElementName = "type")]
    public string Type { get; set; }

    [XmlElement(ElementName = "role")]
    public string Role { get; set; }

    [XmlElement(ElementName = "order")]
    public int Order { get; set; }

    [XmlElement(ElementName = "tmdbid")]
    public int Tmdbid { get; set; }

    [XmlElement(ElementName = "thumb")]
    public string Thumb { get; set; }

    [XmlElement(ElementName = "profile")]
    public string Profile { get; set; }
}