using System.Xml.Serialization;

namespace VideoScraping.DTO.NFO;

[XmlRoot(ElementName="director")]
public class Director
{
    [XmlAttribute(AttributeName="tmdbid")] 
    public int Tmdbid { get; set; } 

    [XmlText] 
    public string Text { get; set; } 
}