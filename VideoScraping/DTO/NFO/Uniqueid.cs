using System.Xml.Serialization;

namespace VideoScraping.DTO.NFO;

[XmlRoot(ElementName="uniqueid")]
public class Uniqueid { 

    [XmlAttribute(AttributeName="type")] 
    public string Type { get; set; } 

    [XmlAttribute(AttributeName="default")] 
    public bool Default { get; set; } 

    [XmlText] 
    public int Text { get; set; } 
}